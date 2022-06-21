using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkLoop : GameLoop
    {
        public NetworkDriverManager Net => NetworkDriverManager.Current;
        public NetworkMessageReaderList Readers;
        public NativeParallelHashMap<ushort, Entity> EntityIndex;
        public GameLoopFuncList SyncUpdate { get; private set; }
        public int Tick { get; internal set; }

        public NetworkLoop(GameLoopRunner loopRunner) : base(loopRunner)
        {
            Readers = new NetworkMessageReaderList();
            EntityIndex = new NativeParallelHashMap<ushort, Entity>(10000, Allocator.Persistent);
            SyncUpdate = new GameLoopFuncList();
        }
    }
}