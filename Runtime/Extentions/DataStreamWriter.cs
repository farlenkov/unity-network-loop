using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

namespace UnityNetworkLoop
{
    public static class DataStreamWriterExt
    {
        public static bool WriteEntityID(this ref DataStreamWriter writer, ushort value)
        {
            return writer.WriteUShort(value);
        }
    }
}