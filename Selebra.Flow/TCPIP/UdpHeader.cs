using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.TCPIP
{
    public struct UdpHeader
    {
        public ushort SrcPort;
        public ushort DstPort;
        public ushort Length;
        public ushort Checksum;
    }

    public class UdpHeaderDecoder
    {
        public UdpHeader DecodeUdpHeader(byte[] payload)
        {
            UdpHeader header = new UdpHeader();
            header.SrcPort = ToUInt16BigEndian(payload, 0);
            header.DstPort = ToUInt16BigEndian(payload, 2);
            header.Length = ToUInt16BigEndian(payload, 4);
            header.Checksum = ToUInt16BigEndian(payload, 6);           

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
