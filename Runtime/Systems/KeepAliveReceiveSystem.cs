using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityNetworkLoop;
using Unity.Networking.Transport;

namespace UnityNetworkLoop
{
    public class KeepAliveReceiveSystem : ReceiveSystem<NetworkLoop>
    {
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

    //public class KeepAliveReceiveSystem : NetworkSystem<NetworkLoop>
    //{
    //    protected override void Init()
    //    {
    //        base.Init();
    //        Loop.Readers.Add(NetworkMessageType.KeepAlive, ReadKeepAliveMessage);
    //    }

    //    void ReadKeepAliveMessage(
    //        ref NetworkConnection connection,
    //        ref DataStreamReader reader,
    //        ushort id)
    //    {
    //        // keep me empty
    //        // id is the only data we get in this message
    //    }
    //}
}