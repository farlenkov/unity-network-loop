using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkMessagesInspectSystem : GameLoopManager<NetworkLoop>
    {
        /// <summary>
        /// 
        /// </summary>
        
        public NetworkMessagesInspectSystem() { }

        public static Action<NetworkLoop> SetNetworkLoop;

        protected override void OnInit()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float obj)
        {
            if (SetNetworkLoop != null)
                SetNetworkLoop(Loop);
        }
    }
}