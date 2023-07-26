using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityGameLoop;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class AcceptConnectionsSystem : GameLoopManager<NetworkLoop>
    {
        /// <summary>
        /// Call Driver.Accept()
        /// </summary>

        public AcceptConnectionsSystem() { }

        protected override void OnInit()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float obj)
        {
            NetworkConnection connection;

            while ((connection = Loop.Net.Driver.Accept()) != default(NetworkConnection))
            {
                Loop.Net.Connections.Add(connection);
                Loop.ConnectEvents.Add(connection);

                Log.InfoEditor(
                    "[AcceptConnectionsSystem] Loop.Connected.Add() - {0}",
                    connection);
            }
        }
    }
}