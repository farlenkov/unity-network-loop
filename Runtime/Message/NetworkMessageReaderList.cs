using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class NetworkMessageReaderList
    {
        Dictionary<ushort, NetworkMessageReaderCallback> eventReaders;
        List<NetworkMessageReader> entityReaders;

        public NetworkMessageReaderList()
        {
            eventReaders = new Dictionary<ushort, NetworkMessageReaderCallback>();
            entityReaders = new List<NetworkMessageReader>();
        }

        public void Add(
            ushort eventId,
            NetworkMessageReaderCallback callback)
        {
            if (eventId < 1 || eventId > 254)
                throw new Exception("[NetworkMessageReaderList: Add] Event ID must be in range 1-254");
            else
                eventReaders.Add(eventId, callback);
        }

        public void Add(
            ushort startId,
            ushort endId,
            NetworkMessageReaderCallback callback)
        {
            if (startId <= 255)
                throw new Exception("[NetworkMessageReaderList: Add] Start ID must greater than 255");

            if (startId >= endId)
                throw new Exception("[NetworkMessageReaderList: Add] Start ID must be less than End ID");

            entityReaders.Add(new NetworkMessageReader()
            {
                StartID = startId,
                EndID = endId,
                Callback = callback
            });
        }

        public NetworkMessageReaderCallback GetReader(ushort id)
        {
            if (id <= 254)
            {
                if (eventReaders.TryGetValue(id, out var callback))
                    return callback;
            }
            else
            {
                for (var i = 0; i < entityReaders.Count; i++)
                {
                    var reader = entityReaders[i];

                    if (reader.StartID <= id &&
                        reader.EndID >= id)
                    {
                        return reader.Callback;

                    }
                }
            }

            return null;
        }
    }
}
