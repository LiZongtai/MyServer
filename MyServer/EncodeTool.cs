using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyServer
{
    public class EncodeTool
    {
        // struct packet: packet head + packet nail
        public static byte[] EncodePacket(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    // write packet head
                    bw.Write(data.Length);
                    // write packet payload
                    bw.Write(data);
                    byte[] packet = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, packet, 0, (int)ms.Length);
                    return packet;
                }
            }
        }
        // parse packet from buffer
        public static byte[] DecodePacket(ref List<byte> cache)
        {
            if (cache.Count < 4)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(cache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int length = br.ReadInt32();
                    int remainLength = (int)(ms.Length - ms.Position);
                    if (length > remainLength)
                    {
                        return null;
                    }
                    byte[] data = br.ReadBytes(length);
                    // reflash data buffer
                    cache.Clear();
                    int remainLengthAgain = (int)(ms.Length - ms.Position);
                    cache.AddRange(br.ReadBytes(remainLengthAgain));
                    return data;
                }
            }
        }
        // transfer NetMsg to byte[]
        public static byte[] EncodeMsg(NetMsg msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(msg.opCode);
                    bw.Write(msg.subCode);
                    if (msg.value != null)
                    {
                        bw.Write(EncodeObj(msg.value));
                    }
                    byte[] data = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
                    return data;
                }
            }
        }
        // transfer byte[] to NetMsg
        public static NetMsg DecodeMsg(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    NetMsg msg = new NetMsg();
                    msg.opCode = br.ReadInt32();
                    msg.subCode = br.ReadInt32();
                    if (ms.Length - ms.Position > 0)
                    {
                        object obj = DecodeObj(br.ReadBytes((int)(ms.Length - ms.Position)));
                        msg.value = obj;
                    }
                    return msg;
                }
            }

        }
        // Deserialize
        private static object DecodeObj(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);

            }
        }
        // Serialize
        private static byte[] EncodeObj(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                byte[] data = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
                return data;
            }
        }
    }
}