using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.Profiling;
using System;

namespace UnityNetworkLoop
{
    public abstract class NetworkDriverManager : MonoBehaviour
    {
        // STATIC 

        public static NetworkDriverManager Current { get; private set; }

        // OBJECT

        public NetworkDriver Driver { get; private set; }
        public NetworkPipeline ReliablePipeline { get; private set; }
        public NativeList<NetworkConnection> Connections { get; private set; }

        // START

        void Awake()
        {
            Current = this;

            DontDestroyOnLoad(gameObject);
            CreateNetworkDriver();
        }

        protected void CreateNetworkDriver()
        {
            // TODO: limit connection count

            Driver = NetworkDriver.Create();
            ReliablePipeline = Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        }

        public void ReadEvents(Action<NetworkEvent.Type, NetworkConnection, DataStreamReader> callback)
        {
            var driver = Driver;
            var connections = Connections;

            for (int i = 0; i < connections.Length; i++)
            {
                var connection = connections[i];

                DataStreamReader reader;
                NetworkEvent.Type cmd;

                while ((cmd = driver.PopEventForConnection(connection, out reader)) != NetworkEvent.Type.Empty)
                {
                    switch (cmd)
                    {
                        case NetworkEvent.Type.Connect:
                            Debug.Log("Connected");
                            callback(cmd, connection, default);
                            break;

                        case NetworkEvent.Type.Data:

                            if (reader.IsCreated)
                                callback(cmd, connection, reader);
                            else
                                Debug.LogError("reader.IsCreated == false"); // ?

                            break;

                        case NetworkEvent.Type.Disconnect:
                            Debug.Log("Disconnected");
                            callback(cmd, connection, default);
                            break;
                    }
                }
            }
        }

        protected void CreateConnections(int count)
        {
            if (Connections.IsCreated)
                Connections.Dispose();

            Connections = new NativeList<NetworkConnection>(count, Allocator.Persistent);
        }

        // DESTROY

        void OnDestroy()
        {
            if (Driver.IsCreated)
                Driver.Dispose();
            
            if (Connections.IsCreated)
                Connections.Dispose();
        }
    }
}
