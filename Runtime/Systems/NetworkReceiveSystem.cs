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
            Loop.Net.ReadEvents(OnNetworkEvent);
        }

        void OnNetworkEvent(
            NetworkEvent.Type event_type,
            NetworkConnection connection,
            DataStreamReader reader)
        {
            if (event_type != NetworkEvent.Type.Data)
                return;

            var readers = Loop.Readers;

            while (reader.GetBytesRead() < reader.Length)
            {
                var id = reader.ReadID();
                var reader_func = readers.GetReader(id);

                if (reader_func == null)
                {
                    Debug.LogErrorFormat("[NetworkReceiveSystem] Can't find NetworkMessageReader for message ID {0} GetBytesRead: {1}", id, reader.GetBytesRead());
                    break;
                }

                reader_func(ref connection, ref reader, id);
            }
        }
    }
}