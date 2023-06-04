using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;
using UnityGameLoop;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class NetworkReceiveSystem : GameLoopManager<NetworkLoop>
    {
        /// <summary>
        /// Process Data and Disconnect events.
        /// Data events goes to NetworkMessageReader.
        /// Disconnect events goes to Loop.DisconnectEvents.
        /// </summary>

        public NetworkReceiveSystem() { }

        protected override void OnInit()
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
            switch (event_type)
            {
                case NetworkEvent.Type.Data:
                    Read(connection, reader);
                    break;

                case NetworkEvent.Type.Disconnect:
                    Disconnect(connection);
                    break;
            }
        }

        void Read(
            NetworkConnection connection,
            DataStreamReader reader)
        {
            var readers = Loop.Readers;

            while (reader.GetBytesRead() < reader.Length)
            {
                var id = reader.ReadID();
                var reader_func = readers.GetReader(id);

                if (reader_func == null)
                {
                    Log.Error("[NetworkReceiveSystem] Can't find NetworkMessageReader for message ID {0} GetBytesRead: {1}", id, reader.GetBytesRead());
                    break;
                }

                //Log.InfoEditor("[NetworkReceiveSystem] Received message ID {0}", id);
                reader_func(ref connection, ref reader, id);
            }
        }

        void Disconnect(NetworkConnection connection)
        {
            // Log.InfoEditor(
            //     "[NetworkReceiveSystem] Loop.Disconnected.Add() - {0}",
            //     connection.InternalId);

            Loop.DisconnectEvents.Add(connection);
        }
    }
}