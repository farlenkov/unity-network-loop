using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class CleanUpConnectionsSystem : GameLoopManager<NetworkLoop>
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
                    connections.RemoveAtSwapBack(i);
                    Loop.Disconnected.Add(connection);
                    i--;

                    Log.InfoEditor(
                        "[CleanUpConnectionsSystem] Loop.Disconnected.Add() - {0}", 
                        connection.InternalId);
                }
            }
        }
    }
}