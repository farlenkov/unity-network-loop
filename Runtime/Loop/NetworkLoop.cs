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
        // FUNC

        public NetworkDriverManager Net => NetworkDriverManager.Current;
        public NetworkMessageReaderList Readers { get; private set; }
        public GameLoopFuncList SyncUpdate { get; private set; }

        // DATA

        public int Tick { get; internal set; }
        public NativeParallelHashMap<ushort, Entity> EntityIndex;

        // MESSAGES

        public NetworkMessageList UnreliableMessages { get; private set; }
        public NetworkMessageList ReliableMessages { get; private set; }

        // CONNECTIONS

        public List<NetworkConnection> ConnectEvents { get; private set; }
        public List<NetworkConnection> DisconnectEvents { get; private set; }

        /// <summary>
        /// Connections that are ready to be used in NetworkSendSystem
        /// </summary>

        public List<NetworkConnection> ReadyConnections { get; private set; }

        public NetworkLoop(Transform rootTransform) : base(rootTransform)
        {
            Readers = new NetworkMessageReaderList();
            SyncUpdate = new GameLoopFuncList();

            UnreliableMessages = new NetworkMessageList();
            ReliableMessages = new NetworkMessageList();

            EntityIndex = new NativeParallelHashMap<ushort, Entity>(10000, Allocator.Persistent);

            ConnectEvents = new List<NetworkConnection>();
            DisconnectEvents = new List<NetworkConnection>();
            ReadyConnections = new List<NetworkConnection>(200);
        }
    }
}