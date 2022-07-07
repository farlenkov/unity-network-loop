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
        public NetworkMessageList UnreliableMessages { get; private set; }
        public NetworkMessageList ReliableMessages { get; private set; }

        public List<NetworkConnection> Connected { get; private set; }
        public List<NetworkConnection> Disconnected { get; private set; }

        public NetworkLoop(GameLoopRunner loopRunner) : base(loopRunner)
        {
            Readers = new NetworkMessageReaderList();
            SyncUpdate = new GameLoopFuncList();

            UnreliableMessages = new NetworkMessageList();
            ReliableMessages = new NetworkMessageList();

            EntityIndex = new NativeParallelHashMap<ushort, Entity>(10000, Allocator.Persistent);
            Connected = new List<NetworkConnection>();
            Disconnected = new List<NetworkConnection>();
        }
    }
}