using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class ServerManager : NetworkManager
    {
        // TODO: separate port for every room

        public void Listen(NetworkEndPoint endpoint)
        {
            Connections = new NativeList<NetworkConnection>(200, Allocator.Persistent);
            
            if (Driver.Bind(endpoint) != 0)
                Debug.LogError("Failed to bind to port 9000");
            else
                Driver.Listen();
        }    
    }
}