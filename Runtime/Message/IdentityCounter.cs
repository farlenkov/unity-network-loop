using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityNetworkLoop
{
    public class IdentityCounter
    {
        ushort idCounter;
        Queue<ushort> releasedIds;

        public ushort StartID { get; private set; }
        public ushort EndID { get; private set; }

        public IdentityCounter(
            ushort startId,
            ushort endId)
        {
            StartID = startId;
            EndID = endId;

            idCounter = startId;
            releasedIds = new Queue<ushort>(EndID - StartID);
        }

        public ushort GetID()
        {
            if (idCounter < EndID)
                return idCounter++;

            else if (releasedIds.Count > 0)
                return releasedIds.Dequeue();

            else
                throw new Exception(string.Format("[EntityCounter: GetID] No free IDs in range {0}-{1}", StartID, EndID));
        }

        public void ReleaseID(ushort id)
        {
            if (id >= StartID && id <= EndID)
                releasedIds.Enqueue(id);
        }

        public void Reset()
        {
            idCounter = StartID;
            releasedIds.Clear();
        }
    }
}