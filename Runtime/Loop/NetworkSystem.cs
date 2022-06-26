using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public abstract class NetworkSystem<LOOP> : GameLoopSystem<LOOP> 
        where LOOP : NetworkLoop
    {
        public int Tick => Loop.Tick;

        protected override void Init()
        {
            Loop.Start.Add(Start);
            Loop.SyncUpdate.Add(Update);
            Loop.Destroy.Add(Destroy);
        }

        void Start(float dt)
        {
            OnStart();
        }
    }
}