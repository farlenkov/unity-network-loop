using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public abstract class NetworkSystem<LOOP> : GameLoopSystem<LOOP> 
        where LOOP : NetworkLoop
    {
        protected virtual bool UseSyncUpdate => false;

        protected override void Init()
        {
            Loop.Start.Add(Start);            
            Loop.Destroy.Add(Destroy);

            if (UseSyncUpdate)
                Loop.SyncUpdate.Add(Update);
            else
                Loop.Update.Add(Update);
        }

        void Start(float dt)
        {
            OnStart();
        }
    }
}