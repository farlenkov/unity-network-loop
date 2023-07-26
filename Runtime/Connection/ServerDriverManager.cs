using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class ServerDriverManager : NetworkDriverManager
    {
        // TODO: separate port for every room

        public bool Listen(
#if UNITY_2022_3_OR_NEWER
            NetworkEndpoint endpoint,
#else
            NetworkEndPoint endpoint,
#endif
            int max_connections)
        {
            if (Driver.Bind(endpoint) != 0)
            {
                Debug.LogError("Failed to bind to port 9000");
                return false;
            }
            else
            {
                var result = Driver.Listen();
                CreateConnections(max_connections);

                return result == 0;
            }
        }
    }
}