using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class NetworkMessageReader
    {
        public ushort StartID;
        public ushort EndID;
        public NetworkMessageReaderCallback Callback;
    }
}