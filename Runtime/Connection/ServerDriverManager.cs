using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class ServerDriverManager : NetworkDriverManager
    {
        // TODO: separate port for every room

        public bool Listen(
            NetworkEndpoint endpoint,
            int maxConnections)
        {
            if (Driver.Bind(endpoint) != 0)
            {
                Debug.LogError("Failed to bind to port 9000");
                return false;
            }
            else
            {
                var result = Driver.Listen();
                CreateConnections(maxConnections);

                return result == 0;
            }
        }
    }
}