using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class SendData : IComponentData
    {
        public NativeArray<byte> Data;
        public DataStreamWriter Writer;
        public int Length;
        public Dictionary<int, int> SyncTickByConnection;

        public void Create (int length)
        {
            Data = new NativeArray<byte>(length, Allocator.Persistent);
            Writer = new DataStreamWriter(Data);
            SyncTickByConnection = new Dictionary<int, int>();
        }
    }
}