using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class CleanupConnectionsSystem : GameLoopManager<NetworkLoop>
    {
        protected override void Init()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float obj)
        {
            var connections = Loop.Net.Connections;

            for (int i = 0; i < Loop.Net.Connections.Length; i++)
            {
                var connection = connections[i];

                if (!connection.IsCreated)
                {
                    // REMOVE from CONNECTIONS

                    connections.RemoveAtSwapBack(i);
                    i--;

                    // REMOVE from READY CONNECTIONS

                    var ready_index = Loop.ReadyConnections.IndexOf(connection);

                    if (ready_index >= 0)
                        Loop.ReadyConnections.RemoveAt(ready_index);

                    // CREATE DISCONNECT EVENT

                    Loop.DisconnectEvents.Add(connection);

                    Log.InfoEditor(
                        "[CleanUpConnectionsSystem] Loop.Disconnected.Add() - {0}", 
                        connection.InternalId);
                }
            }
        }
    }
}