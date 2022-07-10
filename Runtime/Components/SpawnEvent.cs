using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public struct SpawnEvent : IComponentData
    {
        public float3 Position;
    }
}