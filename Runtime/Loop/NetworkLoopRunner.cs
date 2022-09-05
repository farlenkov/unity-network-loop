using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public abstract class NetworkLoopRunner<LOOP> : GameLoopRunner<LOOP>
        where LOOP : NetworkLoop
    {
        float Interval = 1f/20; 
        float TimeAccumulator;

        protected override void Update()
        {
            base.Update();
            CallSyncUpdate();
        }

        void CallSyncUpdate()
        {
            TimeAccumulator += Time.unscaledDeltaTime;

            while (TimeAccumulator >= Interval)
            {
                TimeAccumulator -= Interval;
                Loop.Tick++;
                Call(Interval, Loop.SyncUpdate);
            }
        }
    }
}