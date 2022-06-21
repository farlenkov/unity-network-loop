﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityNetworkLoop
{
    struct SendItem
    {
        public int LastSyncTime;
        public SendData Data;
    }

    [DisableAutoCreation]
    public partial class NetworkSendSystem : NetworkSystem<NetworkLoop>
    {
        List<SendItem> SendItems = new List<SendItem>();

        protected override bool UseSyncUpdate => true;

        protected override void OnUpdate()
        {
            var driver = Loop.Net.Driver;
            var connections = Loop.Net.Connections;

            for (int i = 0; i < connections.Length; i++)
            {
                var connection = connections[i];

                if (!connection.IsCreated)
                    continue;

                if (connection.GetState(driver) != NetworkConnection.State.Connected)
                    continue;

                SortSendData(connection);
                Send(driver, connection);
            }
        }

        void Send(
            NetworkDriver driver,
            NetworkConnection connection)
        {
            var writer = default(DataStreamWriter);

            for (var i = 0; i < SendItems.Count; i++)
            {
                var send = SendItems[i];

                if (!writer.IsCreated)
                {
                    Profiler.BeginSample("NetworkDriver.BeginSend");
                    driver.BeginSend(connection, out writer);
                    Profiler.EndSample();
                }

                if (send.Data.Length > writer.Capacity - writer.Length)
                {
                    break;
                    //Profiler.BeginSample("NetworkDriver.EndSend");
                    //driver.EndSend(writer);
                    //Profiler.EndSample();

                    //Profiler.BeginSample("NetworkDriver.BeginSend");
                    //driver.BeginSend(connection, out writer);
                    //Profiler.EndSample();
                }

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

        void SortSendData(NetworkConnection connection)
        {
            SendItems.Clear();

            Entities.ForEach((SendData send) =>
            {
                if (send.Length == 0)
                    return;

                send.SyncTickByConnection.TryGetValue(connection.InternalId, out var sync_time);

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