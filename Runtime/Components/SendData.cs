using System;
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
        public int UpdateTick;
        public NativeArray<byte> Data;
        public DataStreamWriter Writer;
        public int Length => Writer.Length;
        public Dictionary<int, int> SyncTickByConnection;

        public DataStreamWriter Init(int length, int tick = 0)
        {
            if (!Data.IsCreated)
            {
                Data = new NativeArray<byte>(length, Allocator.Persistent);
                Writer = new DataStreamWriter(Data);
                SyncTickByConnection = new Dictionary<int, int>();
            }
            else
            {
                Writer.Clear();
            }

            if (tick > 0)
                UpdateTick = tick; // sugar for Init()

            return Writer; // sugar for Init()
        }
    }
}