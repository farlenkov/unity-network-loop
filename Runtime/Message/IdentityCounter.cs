using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class IdentityCounter
    {
        ushort id_counter;
        Queue<ushort> released_ids;

        public ushort StartID { get; private set; }
        public ushort EndID { get; private set; }

        public IdentityCounter(
            ushort start_id, 
            ushort end_id)
        {
            StartID = start_id;
            EndID = end_id;

            id_counter = start_id;
            released_ids = new Queue<ushort>(EndID - StartID);
        }

        public ushort GetID()
        {
            if (id_counter < EndID)
                return id_counter++;
            else if (released_ids.Count > 0)
                return released_ids.Dequeue();
            else
                throw new Exception(string.Format("[EntityCounter: GetID] No free IDs in range {0}-{1}", StartID, EndID));
        }

        public void ReleaseID(ushort id)
        {
            if (id >= StartID && id <= EndID)
                released_ids.Enqueue(id);
        }
    }
}