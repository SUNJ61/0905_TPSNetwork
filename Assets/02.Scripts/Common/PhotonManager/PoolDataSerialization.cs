using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PoolDataSerialization
{
    #region 직렬화 이전
    //private static byte[] poolDataMemory = new byte[10 * 10];

    //public static short SerializePoolData(StreamBuffer outStream, object targetObject)
    //{
    //    PoolData data = (PoolData)targetObject;

    //    lock (poolDataMemory)
    //    {
    //        byte[] bytes = poolDataMemory;
    //        int index = 0;

    //        string value_prefab = (string)data.prefabname;
    //        string value_name = (string)data.name;

    //        Protocol.Serialize(value_prefab, bytes, ref index);
    //        Protocol.Serialize(data.position.x, bytes, ref index);
    //        Protocol.Serialize(data.position.y, bytes, ref index);
    //        Protocol.Serialize(data.position.z, bytes, ref index);
    //        Protocol.Serialize(data.rotation.x, bytes, ref index);
    //        Protocol.Serialize(data.rotation.y, bytes, ref index);
    //        Protocol.Serialize(data.rotation.z, bytes, ref index);
    //        Protocol.Serialize(data.rotation.w, bytes, ref index);
    //        Protocol.Serialize(data.active, bytes, ref index);
    //        Protocol.Serialize(data.name, bytes, ref index);
    //        outStream.Write(bytes, 0, 10 * 10);
    //    }

    //    return 10 * 10;
    //}

    //public static object DeserializePoolData(StreamBuffer inStream, short length)
    //{
    //    Color color = new Color();

    //    lock (colorMemory)
    //    {
    //        inStream.Read(colorMemory, 0, 10 * 10);
    //        int index = 0;

    //        Protocol.Deserialize(out color.r, colorMemory, ref index);
    //        Protocol.Deserialize(out color.g, colorMemory, ref index);
    //        Protocol.Deserialize(out color.b, colorMemory, ref index);
    //        Protocol.Deserialize(out color.a, colorMemory, ref index);
    //    }

    //    return color;
    //}
    #endregion
    public static short SerializePoolData(StreamBuffer outStream, List<PoolData> poolDataList) //직렬화
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(poolDataList.Count);

                foreach (var data in poolDataList)
                {
                    SerializeString(data.prefabname, binaryWriter);
                    SerializeVector3(data.position, binaryWriter);
                    SerializeQuaternion(data.rotation, binaryWriter);
                    binaryWriter.Write(data.active);
                    SerializeString(data.name, binaryWriter);
                }
                byte[] bytes = memoryStream.ToArray();
                outStream.Write(bytes, 0, bytes.Length);

                return (short)bytes.Length;
            }
        }
    }
    private static void SerializeString(string value, System.IO.BinaryWriter writer)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
        writer.Write((short)bytes.Length);  // Length of the string
        writer.Write(bytes);  // Actual string data
    }

    private static void SerializeVector3(Vector3 vector, System.IO.BinaryWriter writer)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
        writer.Write(vector.z);
    }

    private static void SerializeQuaternion(Quaternion quaternion, System.IO.BinaryWriter writer)
    {
        writer.Write(quaternion.x);
        writer.Write(quaternion.y);
        writer.Write(quaternion.z);
        writer.Write(quaternion.w);
    }

    public static List<PoolData> DeserializePoolData(StreamBuffer buffer) //역직렬화
    {
        List<PoolData> poolDataList = new List<PoolData>();

        using (var memoryStream = new MemoryStream(buffer.ToArray()))
        {
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                int count = binaryReader.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    PoolData data = new PoolData
                    {
                        prefabname = DeserializeString(binaryReader),
                        position = DeserializeVector3(binaryReader),
                        rotation = DeserializeQuaternion(binaryReader),
                        active = binaryReader.ReadBoolean(),
                        name = DeserializeString(binaryReader)
                    };
                    poolDataList.Add(data);
                }
            }
        }

        return poolDataList;
    }


    private static string DeserializeString(System.IO.BinaryReader reader)
    {
        short length = reader.ReadInt16();
        byte[] bytes = reader.ReadBytes(length);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    private static Vector3 DeserializeVector3(System.IO.BinaryReader reader)
    {
        return new Vector3(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    private static Quaternion DeserializeQuaternion(System.IO.BinaryReader reader)
    {
        return new Quaternion(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }
}
