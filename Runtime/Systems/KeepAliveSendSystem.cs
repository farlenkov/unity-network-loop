using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class KeepAliveSendSystem : GameLoopManager<NetworkLoop>
    {
        /// <summary>
        /// Create entity with 'SendData' component and 'KeepAlive' message type.
        /// TODO: need to rework to send KeepAlive less often.
        /// </summary>

        public KeepAliveSendSystem() { }

        protected override void Init()
        {            
            Loop.Start.Add(Start);
        }

        void Start(float dt)
        {
            var send = new SendData();
            send.Init(2);
            send.Writer.WriteID(NetworkMessageType.KeepAlive);

            var entity = EntityManager.CreateEntity(typeof(SendData));
            EntityManager.SetComponentData(entity, send);
        }
    }
}