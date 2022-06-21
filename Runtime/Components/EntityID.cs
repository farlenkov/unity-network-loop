using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace UnityNetworkLoop
{
    public struct EntityID : IComponentData
    {
        public ushort id;
    }
}