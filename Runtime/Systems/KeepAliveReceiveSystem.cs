using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityNetworkLoop;
using Unity.Networking.Transport;
using Unity.Collections;

namespace UnityNetworkLoop
{
    public class KeepAliveReceiveSystem : ReceiveSystem<NetworkLoop>
    {
        /// <summary>
        /// Receive 'KeepAlive' message. Doesn't do anything else.
        /// </summary>

        public KeepAliveReceiveSystem() { }

        protected override ushort EventID => NetworkMessageType.KeepAlive;

        protected override void ReadMessage(
            ref NetworkConnection connection,
            ref DataStreamReader reader,
            ushort eventId)
        {
            // keep me empty
            // id is the only data we get in this message
        }
    }
}