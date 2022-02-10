using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.TCPIP
{
    public enum TcpOptions
    {
        TCPOptionEOL = 0,
        TCPOptionNOP = 1,
        TCPOptionMSS = 2,
        TCPOptionWS = 3,
        TCPOptionTS = 8,
        TCPOptionSACKPermitted = 4,
        TCPOptionSACK = 5
    }
    public enum TCPFlags
    {
        TCPFlagFin = 0x01,
        TCPFlagSyn = 0x02,
        TCPFlagRst = 0x04,
        TCPFlagPsh = 0x08,
        TCPFlagAck = 0x10,
        TCPFlagUrg = 0x20,
        TCPFlagECE = 0x40,
        TCPFlagCWR = 0x80
    }
    public struct TcpHeader
    {
        public ushort SrcPort;
        public ushort DstPort;
        public uint SeqNum;
        public uint AckNum;

        /// <summary>
        /// Number of 32-bit words in TCP header multiply by 4 to get byte count.
        /// </summary>
        public byte DataOffset;

        public TCPFlags Flags;
        public ushort WindowSize;
        public ushort Checksum;
        public ushort UrgentPointer;
    }
    public struct TCPSynOptions
    {
        public ushort MSS;
        public int WS;
        public bool TS;
        public uint TSVal;
        public uint TSEcr;
        public bool SACKPermitted;
    }
    public struct TCPOptions
    {
        public bool TS;
        public uint TSVal;
        public uint TSEcr;
    }   

    public class TcpHeaderDecoder
    {
        public TcpHeader DecodeTcpHeader(byte[] payload)
        {
            TcpHeader header = new TcpHeader();
            header.SrcPort = ToUInt16BigEndian(payload, 0);
            header.DstPort = ToUInt16BigEndian(payload, 2);
            header.SeqNum = ToUInt32BigEndian(payload, 4);
            header.AckNum = ToUInt32BigEndian(payload, 8);

            header.DataOffset = (byte)(payload[12] >> 4);
            header.Flags = (TCPFlags)payload[13];
            header.WindowSize = ToUInt16BigEndian(payload,14);
            header.Checksum = ToUInt16BigEndian(payload, 16);
            header.UrgentPointer = ToUInt16BigEndian(payload, 18);

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
