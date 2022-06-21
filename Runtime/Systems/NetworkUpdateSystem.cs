using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkUpdateSystem : GameLoopManager<NetworkLoop>
    {
        protected override void Init()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float obj)
        {
            Loop.Net.Driver.ScheduleUpdate().Complete();
        }
    }
}