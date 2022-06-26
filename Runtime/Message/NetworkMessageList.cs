using System.Collections;
using System.Collections.Generic;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class NetworkMessageList
    {
        List<NetworkMessage> list;

        public int Count => list.Count;
        public NetworkMessage this[int i] { get => list[i]; }

        public NetworkMessageList()
        {
            list = new List<NetworkMessage>(100);
        }

        public void Add(NetworkMessage message)
        {
            if (message.Writer.HasFailedWrites)
            {
                message.Data.Dispose();
                Log.Error("[NetworkMessageList: Add] Message Has Failed Writes");
            }
            else
            {
                list.Add(message);
            }
        }

        public void Clear()
        {
            list.Clear();
        }
    }
}