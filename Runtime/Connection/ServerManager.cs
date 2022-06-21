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

        public void Listen(
            NetworkEndPoint endpoint,
            int max_connections)
        {
            if (Driver.Bind(endpoint) != 0)
            {
                Debug.LogError("Failed to bind to port 9000");
            }
            else
            {
                Driver.Listen();
                CreateConnections(max_connections);
            }
        }    
    }
}