using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkUpdateSystem : GameLoopManager<NetworkLoop>
    {
        bool useSyncUpdate;

        /// <summary>
        /// Call Driver.ScheduleUpdate()
        /// </summary>

        public NetworkUpdateSystem()
        {

        }

        public NetworkUpdateSystem UseSyncUpdate(bool value)
        {
            useSyncUpdate = value;
            return this;
        }

        protected override void OnInit()
        {
            if (useSyncUpdate)
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