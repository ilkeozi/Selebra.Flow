using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Selebra.Flow.Sflow;
using Selebra.Flow.TCPIP;

namespace Selebra.Flow.Test
{
    [TestClass]
    public class TcpHeaderTests
    {
        [TestMethod]
        public void TestDecodeTcpHeader()
        {
            //0000   32 cb c2 09 14 be b9 30 3d 1a d0 8b 50 10 80 00
            //0010   fe 13 00 00

            //Transmission Control Protocol, Src Port: 13003, Dst Port: 49673, Seq: 348043568, Ack: 1025167499
            //    Source Port: 13003
            //    Destination Port: 49673
            //    [Stream index: 3]
            //    Sequence number: 348043568(relative sequence number)
            //    Acknowledgment number: 1025167499(relative ack number)
            //    0101.... = Header Length: 20 bytes(5)
            //    Flags: 0x010(ACK)
            //        000. .... .... = Reserved: Not set
            //        ...0.... .... = Nonce: Not set
            //        .... 0... .... = Congestion Window Reduced(CWR): Not set
            //        .... .0.. .... = ECN - Echo: Not set
            //        .... ..0. .... = Urgent: Not set
            //        .... ...1.... = Acknowledgment: Set
            //       .... .... 0... = Push: Not set
            //        .... .... .0.. = Reset: Not set
            //        .... .... ..0. = Syn: Not set
            //        .... .... ...0 = Fin: Not set
            //        [TCP Flags: ·······A····]
            //    Window size value: 32768
            //    [Calculated window size: 32768]
            //    [Window size scaling factor: -1(unknown)]
            //    Checksum: 0xfe13[unverified]
            //    [Checksum Status: Unverified]
            //    Urgent pointer: 0

            //Missing TCP OPTINS
            //    [Timestamps]
            //        [Time since first frame in this TCP stream: 0.824813000 seconds]
            //        [Time since previous frame in this TCP stream: 0.060747000 seconds]
            //    TCP payload(1460 bytes)

            byte[] testdata = SflowDecoder.StringToByteArray("32cbc20914beb9303d1ad08b50108000fe130000");

            TcpHeaderDecoder header = new TcpHeaderDecoder();
            var result = header.DecodeTcpHeader(testdata);
            Assert.AreEqual((ushort)13003, result.SrcPort);
            Assert.AreEqual((ushort)49673, result.DstPort);
            Assert.AreEqual((uint)348043568, result.SeqNum);
            Assert.AreEqual((uint)1025167499, result.AckNum);
            Assert.AreEqual((byte)5, result.DataOffset);
            Assert.AreEqual(TCPFlags.TCPFlagAck, result.Flags);
            Assert.AreEqual((ushort)32768, result.WindowSize);
            Assert.AreEqual((ushort)0xfe13, result.Checksum);
            Assert.AreEqual((ushort)0, result.UrgentPointer);

        }
    }
}
