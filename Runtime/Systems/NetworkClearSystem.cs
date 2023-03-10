using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    public partial class NetworkClearSystem : GameLoopSystem<NetworkLoop>
    {
        /// <summary>
        /// Cleanup network relative data.
        /// </summary>

        public NetworkClearSystem() { }

        protected override GameLoopFuncList UpdateList => Loop.SyncUpdate;

        protected override void OnUpdate()
        {
            //DisposeSendData_OnDestroyEvent();
            DestroyEntity_OnDestroyEvent();

            Loop.ConnectEvents.Clear();
            Loop.DisconnectEvents.Clear();
        }

        // DESTROY EVENT

        //void DisposeSendData_OnDestroyEvent()
        //{
        //    Entities.ForEach((
        //        SendData data,
        //        in DestroyEvent destroy) =>
        //    {
        //        if (data.Data.IsCreated)
        //            data.Data.Dispose();

        //    }).WithoutBurst().Run();
        //}

        void DestroyEntity_OnDestroyEvent()
        {
            Entities.ForEach((
                Entity entity,
                in EntityID entity_id,
                in DestroyEvent destroy) =>
            {
                Loop.EntityIndex.Remove(entity_id.id);
                EntityManager.DestroyEntity(entity);

            }).WithStructuralChanges().Run();
        }

        // DESTROY SYSTEM

        protected override void OnDestroy()
        {
            //DisposeSendData_OnDestroySystem();
            DisposeEntityIndex_OnDestroySystem();
        }

        //void DisposeSendData_OnDestroySystem()
        //{
        //    Entities.ForEach((SendData data) =>
        //    {
        //        if (data.Data.IsCreated)
        //            data.Data.Dispose();

        //    }).WithoutBurst().Run();
        //}

        void DisposeEntityIndex_OnDestroySystem()
        {
            if (Loop.EntityIndex.IsCreated)
                Loop.EntityIndex.Dispose();
        }
    }
}
