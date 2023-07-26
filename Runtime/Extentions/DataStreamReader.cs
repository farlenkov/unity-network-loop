using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;

namespace UnityNetworkLoop
{
    public static class DataStreamReaderExt
    {
        public static ushort ReadID(this ref DataStreamReader reader)
        {
            return reader.ReadUShort();
        }

        public static Vector3 ReadVector3(this ref DataStreamReader reader)
        {
            return new Vector3(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat());
        }

        public static float2 ReadFloat2(this ref DataStreamReader reader)
        {
            return new float2(
                reader.ReadFloat(),
                reader.ReadFloat());
        }
    }
}