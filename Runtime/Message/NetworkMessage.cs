using System;
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
        public int Length => Writer.Length;
        public NetworkConnection Connection;

        public static NetworkMessage Create(ushort event_id, int length = 2)
        {
            var message = new NetworkMessage();
            message.Data = new NativeArray<byte>(length, Allocator.Persistent);
            message.Writer = new DataStreamWriter(message.Data);

            if (event_id > 0)
                message.Writer.WriteID(event_id);
            else
                throw new ArgumentException(string.Format("[NetworkMessage: Create] event_id need to be greater than 0. Current value: {0}", event_id));

            return message;
        }
    }
}