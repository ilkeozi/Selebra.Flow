using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.TCPIP
{
    public struct IcmpHeader
    {
        public byte Type;
        public byte Code;
        public ushort Checksum;
        public uint Other;
    }

    public class IcmpHeaderDecoder
    {
        public IcmpHeader DecodeIcmpHeader(byte[] payload)
        {
            IcmpHeader header = new IcmpHeader();
            header.Type = payload[0];
            header.Code = payload[1];
            header.Checksum = ToUInt16BigEndian(payload, 2);
            header.Other = ToUInt32BigEndian(payload, 4);

            return header;
        }

        public static UInt16 ToUInt16BigEndian(byte[] value, int startIndex)
        {
            return System.BitConverter.ToUInt16(value.Reverse().ToArray(), value.Length - sizeof(UInt16) - startIndex);
        }
        public static UInt32 ToUInt32BigEndian(byte[] value, int startIndex)
        {
            return System.BitConverter.ToUInt32(value.Reverse().ToArray(), value.Length - sizeof(UInt32) - startIndex);
        }
        public static UInt64 ToUInt64BigEndian(byte[] value, int startIndex)
        {
            return System.BitConverter.ToUInt64(value.Reverse().ToArray(), value.Length - sizeof(UInt64) - startIndex);
        }
    }
}
