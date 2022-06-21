using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace UnityNetworkLoop
{
    [DisableAutoCreation]
    public partial class NetworkDestroySystem : NetworkSystem<NetworkLoop>
    {
        protected override void OnDestroy()
        {
            // SEND DATA

            Entities.ForEach((SendData data) =>
            {
                if(data.Data.IsCreated)
                    data.Data.Dispose();

            }).WithoutBurst().Run();

            // ENTITY INDEX

            if (Loop.EntityIndex.IsCreated)
                Loop.EntityIndex.Dispose();

            //// DRIVER

            //Entities.ForEach((NetworkDriverData net) => {

            //    net.Driver.Dispose();

            //}).WithoutBurst().Run();
        }
    }
}