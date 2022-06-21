using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class NetworkMessageReaderList
    {
        Dictionary<ushort, NetworkMessageReaderCallback> event_readers;
        List<NetworkMessageReader> entity_readers;

        public NetworkMessageReaderList()
        {
            event_readers = new Dictionary<ushort, NetworkMessageReaderCallback>();
            entity_readers = new List<NetworkMessageReader>();
        }

        public void Add(
            ushort event_id,
            NetworkMessageReaderCallback callback)
        {
            if (event_id < 1 || event_id > 254)
                throw new Exception("[NetworkMessageReaderList: Add] Event ID must be in range 1-254");
            else
                event_readers.Add(event_id, callback);
        }

        public void Add(
            ushort start_id,
            ushort end_id,
            NetworkMessageReaderCallback callback)
        {
            if (start_id <= 255)
                throw new Exception("[NetworkMessageReaderList: Add] Start ID must greater than 255");

            if (start_id >= end_id)
                throw new Exception("[NetworkMessageReaderList: Add] Start ID must be less than End ID");

            entity_readers.Add(new NetworkMessageReader()
            {
                StartID = start_id,
                EndID = end_id,
                Callback = callback
            });
        }

        public NetworkMessageReaderCallback GetReader(ushort id)
        {
            if (id <= 254)
            {
                if (event_readers.TryGetValue(id, out var callback))
                    return callback;
            }
            else
            {
                for (var i = 0; i < entity_readers.Count; i++)
                {
                    var reader = entity_readers[i];

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
