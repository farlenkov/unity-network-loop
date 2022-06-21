using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityNetworkLoop
{
    public class ClientManager : NetworkManager
    {
        public void Connect(NetworkEndPoint endpoint)
        {
            var connection = Driver.Connect(endpoint);
            Connections = new NativeList<NetworkConnection>(1, Allocator.Persistent);
            Connections.Add(connection);
        }
    }
}