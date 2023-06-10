using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public abstract class ReceiveSystem<LOOP> : GameLoopManager<LOOP> where LOOP : NetworkLoop
    {
        protected virtual ushort EventID => 0;
        protected virtual ushort StartID => 0;
        protected virtual ushort EndID => 0;

        protected override void OnInit()
        {
            if (EventID > 0)
                Loop.Readers.Add(EventID, ReadMessage);
            else
                Loop.Readers.Add(StartID, EndID, ReadMessage);
        }

        protected abstract void ReadMessage(
            ref NetworkConnection connection,
            ref DataStreamReader reader,
            ushort entityId);
    }
}