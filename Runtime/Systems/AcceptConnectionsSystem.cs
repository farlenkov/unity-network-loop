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
            NetworkConnection c;

            while ((c = driver.Accept()) != default(NetworkConnection))
            {
                connections.Add(c);
                Debug.Log("Accepted a connection");
            }
        }
    }
}