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

        protected override void Init()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float obj)
        {
            var connections = Loop.Net.Connections;
            var driver = Loop.Net.Driver;
            NetworkConnection connection;

            while ((connection = driver.Accept()) != default(NetworkConnection))
            {
                connections.Add(connection);
                Loop.ConnectEvents.Add(connection);

                Log.InfoEditor(
                    "[AcceptConnectionsSystem] Loop.Connected.Add() - {0}",
                    connection.InternalId);
            }
        }
    }
}