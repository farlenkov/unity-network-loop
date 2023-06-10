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

        public ushort EventID { get; private set; }

        public static NetworkMessage Create(ushort eventId, int length = 2)
        {
            var message = new NetworkMessage();
            message.Data = new NativeArray<byte>(length, Allocator.Persistent);
            message.Writer = new DataStreamWriter(message.Data);
            message.EventID = eventId;

            if (eventId > 0)
                message.Writer.WriteID(eventId);
            else
                throw new ArgumentException(string.Format("[NetworkMessage: Create] event_id need to be greater than 0. Current value: {0}", eventId));

            return message;
        }
    }
}