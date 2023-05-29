using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkUpdateSystem : GameLoopManager<NetworkLoop>
    {
        bool UseSyncUpdate;

        /// <summary>
        /// Call Driver.ScheduleUpdate()
        /// </summary>

        public NetworkUpdateSystem(bool useSyncUpdate)
        {
            UseSyncUpdate = useSyncUpdate;
        }

        protected override void OnInit()
        {
            if (UseSyncUpdate)
                Loop.SyncUpdate.Add(Update);
            else
                Loop.Update.Add(Update);
        }

        void Update(float obj)
        {
            Loop.Net.Driver.ScheduleUpdate().Complete();
        }
    }
}