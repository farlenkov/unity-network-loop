using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public struct NetworkMessage
    {
        public NativeArray<byte> Data;
        public DataStreamWriter Writer;
        public int Length;

        public void Create(int length)
        {
            Data = new NativeArray<byte>(length, Allocator.Persistent);
            Writer = new DataStreamWriter(Data);
        }
    }
}