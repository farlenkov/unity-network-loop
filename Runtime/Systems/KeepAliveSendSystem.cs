using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class KeepAliveSendSystem : GameLoopManager<NetworkLoop>
    {
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