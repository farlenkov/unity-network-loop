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
        float NextTick;
        float PrevTick;

        protected override void Update()
        {
            base.Update();
            CallSyncUpdate();
        }

        void CallSyncUpdate()
        {
            var time = Time.time;

            if (NextTick <= time)
            {
                Loop.Tick++;

                Call(time - PrevTick, Loop.SyncUpdate);

                PrevTick = time;
                NextTick = time + Interval;
            }
        }
    }
}