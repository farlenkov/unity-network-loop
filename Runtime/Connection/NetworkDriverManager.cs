using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.Profiling;
using System;
using Unity.Networking.Transport.Utilities;
using UnityUtility;

namespace UnityNetworkLoop
{
    public abstract class NetworkDriverManager : MonoBehaviour
    {
        // STATIC 

        public static NetworkDriverManager Current { get; private set; }

        // OBJECT

        public NetworkDriver Driver { get; private set; }
        public NetworkPipeline UnreliablePipeline { get; private set; }
        public NetworkPipeline ReliablePipeline { get; private set; }
        public NativeList<NetworkConnection> Connections { get; private set; }

        // START

        void Awake()
        {
            if (Current != null)
            {
                Destroy(this);
                return;
            }

            Current = this;

            DontDestroyOnLoad(gameObject);
            CreateNetworkDriver();
        }

        protected void CreateNetworkDriver()
        {
            // TODO: limit connection count

            var simulation = new SimulatorUtility.Parameters
            {
                MaxPacketSize = 1400,
                MaxPacketCount = 30,
                PacketDelayMs = 300,
                PacketJitterMs = 100,
                PacketDropInterval = 2,
                PacketDropPercentage = 50
            };

            var settings = new NetworkSettings(Allocator.Temp);
            //settings.AddRawParameterStruct(ref simulation);

            Driver = NetworkDriver.Create(settings);
            UnreliablePipeline = Driver.CreatePipeline(typeof(SimulatorPipelineStage)); // Driver.CreatePipeline();
            ReliablePipeline = Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        }

        protected void CreateConnections(int count)
        {
            if (Connections.IsCreated)
                Connections.Dispose();

            Connections = new NativeList<NetworkConnection>(count, Allocator.Persistent);
        }

        public void UpdateDriver()
        {
            Driver.ScheduleUpdate().Complete();
        }

        public void ReadEvents(Action<NetworkEvent.Type, NetworkConnection, DataStreamReader> callback)
        {
            for (int i = 0; i < Connections.Length; i++)
            {
                var connection = Connections[i];
                DataStreamReader reader;
                NetworkEvent.Type cmd;

                while ((cmd = Driver.PopEventForConnection(connection, out reader)) != NetworkEvent.Type.Empty)
                {
                    switch (cmd)
                    {
                        case NetworkEvent.Type.Connect:
                            Debug.LogFormat("[NetworkDriverManager: ReadEvents] Connected {0}", connection.InternalId);
                            callback(cmd, connection, default);
                            break;

                        case NetworkEvent.Type.Data:
                            if (reader.IsCreated)
                                callback(cmd, connection, reader);
                            else
                                Debug.LogErrorFormat("[NetworkDriverManager: ReadEvents] Data {0} / reader.IsCreated == false", connection.InternalId); // ?
                            break;

                        case NetworkEvent.Type.Disconnect:
                            Debug.LogFormat("[NetworkDriverManager: ReadEvents] Disconnected {0}", connection.InternalId);
                            callback(cmd, connection, default);
                            Connections.RemoveAtSwapBack(i);
                            i--;
                            break;
                    }
                }
            }
        }

        // DISCONNECT

        public void Disconnect()
        {
            if (!Connections.IsCreated)
                return;

            for (var i = 0; i < Connections.Length; i++)
            {
                var connection = Connections[i];

                if (connection.IsCreated)
                {
                    Driver.BeginSend(ReliablePipeline, connection, out var writer);
                    writer.WriteID(NetworkMessageType.Disconnect);
                    Driver.EndSend(writer);

                    Log.InfoEditor("[NetworkDriverManager: Disconnect] connection: {0}", connection.InternalId);
                    connection.Disconnect(Driver);
                }
            }
        }

        // DESTROY

        void OnDestroy()
        {
            Log.InfoEditor(
                "[NetworkDriverManager: OnDestroy] Driver: {0} Connections: {1}",
                Driver.IsCreated,
                Connections.IsCreated);

            if (Driver.IsCreated)
                Driver.Dispose();

            if (Connections.IsCreated)
                Connections.Dispose();
        }
    }
}
