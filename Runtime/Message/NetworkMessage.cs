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

        public static NetworkMessage Create(int length, ushort id = 0)
        {
            var message = new NetworkMessage();
            message.Data = new NativeArray<byte>(length, Allocator.Persistent);
            message.Writer = new DataStreamWriter(message.Data);

            if (id > 0)
                message.Writer.WriteEntityID(id);

            return message;
        }

        public static NetworkMessage Create(ushort event_id)
        {
            var message = new NetworkMessage();
            message.Data = new NativeArray<byte>(2, Allocator.Persistent);
            message.Writer = new DataStreamWriter(message.Data);

            if (event_id > 0)
                message.Writer.WriteEntityID(event_id);
            else
                throw new ArgumentException(string.Format("[NetworkMessage: Create] event_id need to be greater than 0. Current value: {0}", event_id));

            return message;
        }
    }
}