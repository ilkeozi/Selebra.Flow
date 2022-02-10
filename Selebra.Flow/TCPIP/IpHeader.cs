using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.TCPIP
{
    public struct IPHeader
    {
        public byte Version;
        public byte IHL;

        public byte TOS;

        public ushort TotalLength;

        public ushort ID;

        public byte Flags;

        public ushort FragmentOffset;

        public byte TTL;

        public byte Protocol;

        public ushort Checksum;

        public IPAddress SrcAddr;

        public IPAddress DstAddr;
    }

    public class Ipv4HeaderDecoder
    {

        public IPHeader DecodeIpv4Header(byte[] payload)
        {
            //int offset = header.Offset;
            IPHeader header = new IPHeader();

            byte b = payload[0];
            header.Version = (byte)(payload[0] >> 4);
            header.IHL = payload[0] <<= 4;
            header.IHL = payload[0] >>= 4;
            header.IHL = payload[0] *= 4;


            header.TOS = payload[1];
            header.TotalLength = ToUInt16BigEndian(payload, 2);
            header.ID = ToUInt16BigEndian(payload, 4);

            int frag = ToUInt16BigEndian(payload, 6);
            header.Flags = (byte)((frag &0xe000) >> 13);
            header.FragmentOffset = (ushort)(frag & 0x1fff);
            header.TTL = payload[8];
            header.Protocol = payload[9];
            header.Checksum = ToUInt16BigEndian(payload, 10);
            ArraySegment<byte> srcarray = new ArraySegment<byte>(payload, 12, 4);
            header.SrcAddr = new IPAddress(srcarray.ToArray());
            ArraySegment<byte> dstarray = new ArraySegment<byte>(payload, 16, 4);
            header.DstAddr = new IPAddress(dstarray.ToArray());           

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
