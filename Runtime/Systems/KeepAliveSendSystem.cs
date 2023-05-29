using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityGameLoop;
using UnityEngine.PlayerLoop;

namespace UnityNetworkLoop
{
    public class KeepAliveSendSystem : GameLoopManager<NetworkLoop>
    {
        /// <summary>
        /// Create entity with 'SendData' component and 'KeepAlive' message type.
        /// TODO: need to rework to send KeepAlive less often.
        /// </summary>
        /// 

        public KeepAliveSendSystem() { }

        float NextSendTime;

        protected override void OnInit()
        {
            Loop.SyncUpdate.Add(Update);
        }

        void Update(float dt)
        {
            if (ElapsedTime < NextSendTime)
                return;

            NextSendTime = ElapsedTime + Loop.Net.KeepAliveInterval;
            Loop.UnreliableMessages.Add(NetworkMessageType.KeepAlive);
        }
    }
}