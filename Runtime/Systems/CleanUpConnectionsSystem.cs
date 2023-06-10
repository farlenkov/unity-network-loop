using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityGameLoop;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class CleanupConnectionsSystem : GameLoopManager<NetworkLoop>
    {
        /// <summary>
        /// Check connection.IsCreated for exsisting connections
        /// </summary>

        public CleanupConnectionsSystem() { }

        protected override void OnInit()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float obj)
        {
            for (int i = 0; i < Loop.Net.Connections.Length; i++)
            {
                var connection = Loop.Net.Connections[i];

                if (connection.GetState(Loop.Net.Driver) == NetworkConnection.State.Connected)
                    continue;

                // REMOVE from CONNECTIONS

                Loop.Net.Connections.RemoveAtSwapBack(i);
                i--;

                // REMOVE from READY CONNECTIONS

                var readyIndex = Loop.ReadyConnections.IndexOf(connection);

                if (readyIndex >= 0)
                    Loop.ReadyConnections.RemoveAt(readyIndex);

                // CREATE DISCONNECT EVENT

                Loop.DisconnectEvents.Add(connection);

                // Log.InfoEditor(
                //     "[CleanUpConnectionsSystem] Loop.Disconnected.Add() - {0}",
                //     connection.InternalId);
            }
        }
    }
}