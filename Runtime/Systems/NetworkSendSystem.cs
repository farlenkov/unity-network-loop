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
    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    public partial class NetworkSendSystem : GameLoopSystem<NetworkLoop>
    {
        /// <summary>
        /// Send 'NetworkMessage' and 'SendData' to 'ReadyConnections'
        /// </summary>

        public NetworkSendSystem() { }

        protected override GameLoopFuncList UpdateList => Loop.SyncUpdate;

        protected override void OnUpdate()
        {
            var driver = Loop.Net.Driver;
            var connections = Loop.ReadyConnections;
            var dataSended = false;

            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];

                if (!connection.IsCreated)
                    continue;

                if (connection.GetState(driver) != NetworkConnection.State.Connected)
                    continue;

                var writer = default(DataStreamWriter);

                Send(driver, connection, Loop.Net.ReliablePipeline, Loop.ReliableMessages, ref writer);
                Send(driver, connection, Loop.Net.UnreliablePipeline, Loop.UnreliableMessages, ref writer);

                if (writer.IsCreated)
                {
                    Profiler.BeginSample("NetworkDriver.EndSend");
                    driver.EndSend(writer);
                    Profiler.EndSample();

                    dataSended = true;
                }
            }

            ClearMessages(Loop.UnreliableMessages);
            ClearMessages(Loop.ReliableMessages);

            if (dataSended)
                driver.ScheduleFlushSend(default).Complete();
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

                CheckAndInitWriter(
                    message.Length,
                    driver,
                    connection,
                    pipeline,
                    ref writer);

                Profiler.BeginSample("NativeArray<byte>.GetSubArray");
                var subArray = message.Data.GetSubArray(0, message.Length);
                Profiler.EndSample();

                Profiler.BeginSample("DataStreamWriter.WriteBytes");
                writer.WriteBytes(subArray);
                Profiler.EndSample();
            }
        }

        void CheckAndInitWriter(
            int messageLenght,
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

            if (messageLenght > writer.Capacity - writer.Length)
            {
                Profiler.BeginSample("NetworkDriver.EndSend");
                driver.EndSend(writer);
                Profiler.EndSample();

                Profiler.BeginSample("NetworkDriver.BeginSend");
                driver.BeginSend(pipeline, connection, out writer);
                Profiler.EndSample();
            }
        }
    }
}