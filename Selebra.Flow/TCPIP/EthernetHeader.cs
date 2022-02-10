using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.TCPIP
{
    public struct EthernetHeader
    {
        public byte[] Preamble;
        public byte SFD;
        public byte[] DstAddress;
        public byte[] SrcAddress;
        public ushort TypeLength;
    }

    public class EthernetHeaderDecoder
    {
        public EthernetHeader DecodeEthernetHeader(byte[] payload)
        {
            EthernetHeader header = new EthernetHeader();
            header.Preamble = payload;
            header.SFD = payload[0];


            ArraySegment<byte> dstbytes = new ArraySegment<byte>(payload, 0, 6);
            header.DstAddress = dstbytes.ToArray();

            ArraySegment<byte> srcbytes = new ArraySegment<byte>(payload, 6, 6);
            header.SrcAddress = srcbytes.ToArray();

            header.TypeLength = ToUInt16BigEndian(payload, 12);

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
