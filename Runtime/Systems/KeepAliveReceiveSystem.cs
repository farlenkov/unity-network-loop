using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityNetworkLoop;
using Unity.Networking.Transport;

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
            ushort event_id)
        {
            // keep me empty
            // id is the only data we get in this message
        }
    }
}