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

            CreateConnections(1);
            Connections.Add(connection);
        }
    }
}