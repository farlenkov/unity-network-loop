using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkReceiveSystem : GameLoopManager<NetworkLoop>
    {
        protected override void Init()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float obj)
        {
            var driver = Loop.Net.Driver;
            var connections = Loop.Net.Connections;

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
                            break;

                        case NetworkEvent.Type.Data:

                            if (reader.IsCreated)
                                OnReceiveMessage(ref connection, ref reader);
                            else
                                Debug.LogError("reader.IsCreated == false"); // ?

                            break;

                        case NetworkEvent.Type.Disconnect:
                            Debug.Log("Disconnected");
                            break;
                    }
                }
            }
        }

        void OnReceiveMessage(
            ref NetworkConnection connection, 
            ref DataStreamReader reader)
        {
            var readers = Loop.Readers;

            while (reader.GetBytesRead() < reader.Length)
            {
                var offset = reader.GetBytesRead();
                var id = reader.ReadEntityID();
                var reader_func = readers.GetReader(id);

                if (reader_func == null)
                {
                    Debug.LogErrorFormat("[NetworkReceiveSystem] Can't find NetworkMessageReader for message ID {0} GetBytesRead: {1}", id, reader.GetBytesRead());
                    break;
                }

                //Debug.LogFormat("[NetworkReceiveSystem] NetworkMessageReader: {0}", reader_func.Method.Name);
                reader_func(ref connection, ref reader, id);
                //Debug.LogFormat("[NetworkReceiveSystem] OnReceiveMessage: {0} > {1} '{2}'", offset, reader.GetBytesRead(), reader_func.Method.Name);
            }
        }
    }
}