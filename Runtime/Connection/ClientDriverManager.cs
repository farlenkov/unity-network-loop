using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityNetworkLoop
{
    public class ClientDriverManager : NetworkDriverManager
    {
        public NetworkConnection Connect(
#if UNITY_2022_3_OR_NEWER
            NetworkEndpoint endpoint
#else
            NetworkEndPoint endpoint
#endif
            )
        {
            var connection = Driver.Connect(endpoint);

            CreateConnections(1);
            Connections.Add(connection);

            return connection;
        }
    }
}