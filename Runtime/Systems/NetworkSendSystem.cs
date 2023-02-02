using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Profiling;
using UnityGameLoop;
using UnityUtility;

namespace UnityNetworkLoop
{
    struct SendItem
    {
        public int LastSyncTime;
        public SendData Data;
    }

    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    public partial class NetworkSendSystem : GameLoopSystem<NetworkLoop>
    {
        /// <summary>
        /// Send 'NetworkMessage' and 'SendData' to 'ReadyConnections'
        /// </summary>

        public NetworkSendSystem() { }

        List<SendItem> SendItems = new List<SendItem>();
        protected override GameLoopFuncList UpdateList => Loop.SyncUpdate;

        protected override void OnUpdate()
        {
            var driver = Loop.Net.Driver;
            var connections = Loop.ReadyConnections;
            //var sendCount = 0;

            //Log.InfoEditor(
            //    "[NetworkSendSystem] sendCount: {0} ReadyConnections: {1} ReliableMessages: {2} UnreliableMessages: {3}",
            //    sendCount,
            //    Loop.ReadyConnections.Count,
            //    Loop.ReliableMessages.Count,
            //    Loop.UnreliableMessages.Count);

            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];

                if (!connection.IsCreated)
                    continue;

                if (connection.GetState(driver) != NetworkConnection.State.Connected)
                    continue;

                SortSendData(connection);

                var writer = default(DataStreamWriter);

                Send(driver, connection, Loop.Net.ReliablePipeline, Loop.ReliableMessages, ref writer);
                Send(driver, connection, Loop.Net.UnreliablePipeline, Loop.UnreliableMessages, ref writer);
                Send(driver, connection, Loop.Net.UnreliablePipeline, ref writer);

                //sendCount++;
            }

            //if (sendCount == 0 &&
            //    (Loop.UnreliableMessages.Count > 0 ||
            //    Loop.ReliableMessages.Count > 0))
            //{
                //Log.Error(
                //    "[NetworkSendSystem] sendCount: {0} ReadyConnections: {1} ReliableMessages: {2} UnreliableMessages: {3}",
                //    sendCount,
                //    Loop.ReadyConnections.Count,
                //    Loop.ReliableMessages.Count,
                //    Loop.UnreliableMessages.Count);
            //}

            ClearMessages(Loop.UnreliableMessages);
            ClearMessages(Loop.ReliableMessages);
        }

        void ClearMessages(NetworkMessageList messages)
        {
            for (var i = 0; i < messages.Count; i++)
                messages[i].Data.Dispose();

            messages.Clear();
        }

        void Send(
            NetworkDriver driver, 
            NetworkConnection connection,
            NetworkPipeline pipeline,
            NetworkMessageList messages,
            ref DataStreamWriter writer)
        {
            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];

                if (message.Connection.IsCreated &&
                    message.Connection != connection)
                    continue;

                //if (!writer.IsCreated)
                //{
                //    Profiler.BeginSample("NetworkDriver.BeginSend");
                //    driver.BeginSend(pipeline, connection, out writer);
                //    Profiler.EndSample();
                //}

                CheckAndInitWriter(
                    message.Length,
                    driver,
                    connection,
                    pipeline,
                    ref writer);

                //if (writer.IsCreated && message.EventID == 14)
                //    Log.InfoEditor("[NetworkSendSystem] EventID {0} 'PlayerSpawn' {1}", message.EventID, writer.Length);

                Profiler.BeginSample("NativeArray<byte>.GetSubArray");
                var sub_array = message.Data.GetSubArray(0, message.Length);
                Profiler.EndSample();

                Profiler.BeginSample("DataStreamWriter.WriteBytes");
                writer.WriteBytes(sub_array);
                Profiler.EndSample();
            }
        }

        void Send(
            NetworkDriver driver,
            NetworkConnection connection,
            NetworkPipeline pipeline,
            ref DataStreamWriter writer)
        {
            for (var i = 0; i < SendItems.Count; i++)
            {
                var send = SendItems[i];

                CheckAndInitWriter(
                    send.Data.Length,
                    driver,
                    connection,
                    pipeline,
                    ref writer);

                Profiler.BeginSample("NativeArray<byte>.GetSubArray");
                var sub_array = send.Data.Data.GetSubArray(0, send.Data.Length);
                Profiler.EndSample();

                Profiler.BeginSample("DataStreamWriter.WriteBytes");
                writer.WriteBytes(sub_array);
                Profiler.EndSample();

                if (!send.Data.SyncTickByConnection.ContainsKey(connection.InternalId))
                    send.Data.SyncTickByConnection.Add(connection.InternalId, Loop.Tick);
                else
                    send.Data.SyncTickByConnection[connection.InternalId] = Loop.Tick;
            }

            if (writer.IsCreated)
            {
                Profiler.BeginSample("NetworkDriver.EndSend");
                driver.EndSend(writer);
                Profiler.EndSample(); 
            }
        }

        void CheckAndInitWriter(
            int message_lenght,
            NetworkDriver driver,
            NetworkConnection connection,
            NetworkPipeline pipeline,
            ref DataStreamWriter writer)
        {
            if (!writer.IsCreated)
            {
                Profiler.BeginSample("NetworkDriver.BeginSend");
                driver.BeginSend(pipeline, connection, out writer);
                Profiler.EndSample();
                return;
            }

            if (message_lenght > writer.Capacity - writer.Length)
            {
                Log.InfoEditor(
                    "[NetworkSendSystem] Recreate Writer {0} + {1} > {2}",
                    writer.Length,
                    message_lenght,
                    writer.Capacity);

                //break;

                Profiler.BeginSample("NetworkDriver.EndSend");
                driver.EndSend(writer);
                Profiler.EndSample();

                Profiler.BeginSample("NetworkDriver.BeginSend");
                driver.BeginSend(pipeline, connection, out writer);
                Profiler.EndSample();
            }
        }

        void SortSendData(NetworkConnection connection)
        {
            SendItems.Clear();

            Entities.ForEach((SendData send) =>
            {
                if (send.Length == 0)
                    return;

                send.SyncTickByConnection.TryGetValue(connection.InternalId, out var sync_time);

                if (send.UpdateTick <= sync_time)
                    return;

                SendItems.Add(new SendItem()
                {
                    LastSyncTime = sync_time,
                    Data = send
                });                

            }).WithoutBurst().Run();

            Profiler.BeginSample("SendItems.Sort");
            SendItems.Sort(Compare);
            Profiler.EndSample();
        }

        int Compare(SendItem item1, SendItem item2)
        {
            return item1.LastSyncTime.CompareTo(item2.LastSyncTime);
        }

        //protected override void OnUpdate()
        //{
        //    Profiler.BeginSample("NetworkSendSystem.OnUpdate");

        //    Entities.ForEach((NetworkDriverData driver) =>
        //    {
        //        Send(driver.Driver);

        //    }).WithoutBurst().Run();

        //    Profiler.EndSample();
        //}

        //void Send(NetworkDriver driver)
        //{
        //    Entities.ForEach((NetworkConnectionData connection) =>
        //    {

        //        Send(driver, connection.Connection);

        //    }).WithoutBurst().Run();
        //}

        //void Send(NetworkDriver driver, NetworkConnection connection)
        //{
        //    if (!connection.IsCreated)
        //        return;

        //    if (connection.GetState(driver) != NetworkConnection.State.Connected)
        //        return;

        //    var writer = default(DataStreamWriter);

        //    Entities.ForEach((SendData send) =>
        //    {
        //        if (send.Length == 0)
        //            return;

        //        if (!writer.IsCreated)
        //        {
        //            Profiler.BeginSample("NetworkDriver.BeginSend");
        //            driver.BeginSend(connection, out writer);
        //            Profiler.EndSample();
        //        }

        //        if (send.Length > writer.Capacity - writer.Length)
        //        {
        //            Profiler.BeginSample("NetworkDriver.EndSend");
        //            driver.EndSend(writer);
        //            Profiler.EndSample();

        //            Profiler.BeginSample("NetworkDriver.BeginSend");
        //            driver.BeginSend(connection, out writer);
        //            Profiler.EndSample();
        //        }

        //        Profiler.BeginSample("NativeArray<byte>.GetSubArray");
        //        var sub_array = send.Data.GetSubArray(0, send.Length);
        //        Profiler.EndSample();

        //        Profiler.BeginSample("DataStreamWriter.WriteBytes");
        //        writer.WriteBytes(sub_array);
        //        Profiler.EndSample();

        //        send.Length = 0;

        //    }).WithoutBurst().Run();

        //    if (writer.IsCreated)
        //    {
        //        Profiler.BeginSample("NetworkDriver.EndSend");
        //        driver.EndSend(writer);
        //        Profiler.EndSample();
        //    }
        //}
    }
}