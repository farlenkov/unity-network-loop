using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace UnityNetworkLoop
{
    public class KeepAliveSendSystem : NetworkSystem<NetworkLoop>
    {
        protected override void Init()
        {
            Loop.Start.Add(Start);
        }

        void Start(float dt)
        {
            var send = new SendData();
            send.Create(2);

            send.Writer.WriteID(NetworkMessageType.KeepAlive);
            send.Length = send.Writer.Length;

            var entity = EntityManager.CreateEntity(typeof(SendData));
            EntityManager.SetComponentData(entity, send);
        }
    }
}