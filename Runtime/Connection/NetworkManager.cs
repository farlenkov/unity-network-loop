using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using UnityEngine.Profiling;
using System;

namespace UnityNetworkLoop
{
    public abstract class NetworkManager : MonoBehaviour
    {
        // STATIC 

        public static NetworkManager Current { get; private set; }

        // OBJECT

        public NetworkDriver Driver { get; private set; }
        public NetworkPipeline ReliablePipeline { get; private set; }
        public NativeList<NetworkConnection> Connections { get; protected set; }

        // START

        void Awake()
        {
            Current = this;
            DontDestroyOnLoad(gameObject);
            CreateNetworkDriver();
        }

        protected void CreateNetworkDriver()
        {
            Driver = NetworkDriver.Create();
            ReliablePipeline = Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        }

        // DESTROY

        void OnDestroy()
        {
            if (Driver.IsCreated)
                Driver.Dispose();
            
            if (Connections.IsCreated)
                Connections.Dispose();
        }
    }
}
