using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public static class DataStreamReaderExt
    {
        public static ushort ReadEntityID(this ref DataStreamReader reader)
        {
            return reader.ReadUShort();
        }
    }
}