using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    [DisableAutoCreation]
    public partial class NetworkClearSystem : NetworkSystem<NetworkLoop>
    {
        protected override void OnUpdate()
        {
            Loop.NewConnections.Clear();
            Loop.OldConnections.Clear();
        }

        protected override void OnDestroy()
        {
            DisposeSendData();
            DisposeEntityIndex();
        }

        void DisposeSendData()
        {
            Entities.ForEach((SendData data) =>
            {
                if (data.Data.IsCreated)
                    data.Data.Dispose();

            }).WithoutBurst().Run();
        }

        void DisposeEntityIndex()
        {
            if (Loop.EntityIndex.IsCreated)
                Loop.EntityIndex.Dispose();
        }
    }
}
