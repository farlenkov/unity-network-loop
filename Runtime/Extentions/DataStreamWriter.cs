using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;

namespace UnityNetworkLoop
{
    public static class DataStreamWriterExt
    {
        public static bool WriteID(this ref DataStreamWriter writer, ushort value)
        {
            return writer.WriteUShort(value);
        }

        public static bool WriteVector3(this ref DataStreamWriter writer, Vector3 value)
        {
            return
                writer.WriteFloat(value.x) &&
                writer.WriteFloat(value.y) &&
                writer.WriteFloat(value.z);
        }

        public static bool WriteFloat2(this ref DataStreamWriter writer, float2 value)
        {
            return
                writer.WriteFloat(value.x) &&
                writer.WriteFloat(value.y);
        }
    }
}