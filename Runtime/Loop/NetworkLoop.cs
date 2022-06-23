using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkLoop : GameLoop
    {
        public NetworkDriverManager Net => NetworkDriverManager.Current;
        public NetworkMessageReaderList Readers;
        public GameLoopFuncList SyncUpdate { get; private set; }
        public int Tick { get; internal set; }

        public NativeParallelHashMap<ushort, Entity> EntityIndex;
        public List<NetworkMessage> SendMessages { get; private set; }

        public List<NetworkConnection> NewConnections { get; private set; }
        public List<NetworkConnection> OldConnections { get; private set; }

        public NetworkLoop(GameLoopRunner loopRunner) : base(loopRunner)
        {
            Readers = new NetworkMessageReaderList();
            SendMessages = new List<NetworkMessage>();
            SyncUpdate = new GameLoopFuncList();

            EntityIndex = new NativeParallelHashMap<ushort, Entity>(10000, Allocator.Persistent);
            NewConnections = new List<NetworkConnection>();
            OldConnections = new List<NetworkConnection>();
        }
    }
}