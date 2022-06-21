using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public delegate void NetworkMessageReaderCallback(
        ref NetworkConnection connection,
        ref DataStreamReader reader,
        ushort id);
}
