using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class AcceptConnectionsSystem : GameLoopManager<NetworkLoop>
    {
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
                Loop.NewConnections.Add(connection);
                Debug.Log("Accepted a connection");
            }
        }
    }
}