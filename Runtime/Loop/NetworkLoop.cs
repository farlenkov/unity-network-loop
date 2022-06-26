using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public abstract class NetworkLoop : GameLoop
    {
        public NetworkDriverManager Net => NetworkDriverManager.Current;
        public NetworkMessageReaderList Readers { get; private set; }
        public GameLoopFuncList SyncUpdate { get; private set; }

        // DATA

        public int Tick { get; internal set; }

        public NativeParallelHashMap<ushort, Entity> EntityIndex;
        public NetworkMessageList SendMessages { get; private set; }

        public List<NetworkConnection> NewConnections { get; private set; }
        public List<NetworkConnection> OldConnections { get; private set; }

        public NetworkLoop(GameLoopRunner loopRunner) : base(loopRunner)
        {
            Readers = new NetworkMessageReaderList();
            SyncUpdate = new GameLoopFuncList();

            SendMessages = new NetworkMessageList();
            EntityIndex = new NativeParallelHashMap<ushort, Entity>(10000, Allocator.Persistent);
            NewConnections = new List<NetworkConnection>();
            OldConnections = new List<NetworkConnection>();
        }
    }
}