using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Selebra.Flow.TCPIP;
using Selebra.Flow.Sflow;
using System.Net;

namespace Selebra.Flow.Test
{
    [TestClass]
    public class Ipv4HeaderTests
    {
        [TestMethod]
        public void TestDecodeIpV4Header()
        {

            //0000   45 00 05 dc 6f 6b 40 00 80 06 ac 75 ac 1e 40 06
            //0010   ac 1e 40 f8

            //Internet Protocol Version 4, Src: 172.30.64.6, Dst: 172.30.64.248
            //    0100.... = Version: 4
            //   .... 0101 = Header Length: 20 bytes(5)
            //    Differentiated Services Field: 0x00(DSCP: CS0, ECN: Not - ECT)
            //        0000 00.. = Differentiated Services Codepoint: Default(0)
            //        .... ..00 = Explicit Congestion Notification: Not ECN-Capable Transport(0)
            //    Total Length: 1500
            //    Identification: 0x6f6b(28523)
            //    Flags: 0x4000, Don't fragment
            //        0... .... .... .... = Reserved bit: Not set
            //        .1.. .... .... .... = Don't fragment: Set
            //        ..0. .... .... .... = More fragments: Not set
            //        ...0 0000 0000 0000 = Fragment offset: 0
            //    Time to live: 128
            //    Protocol: TCP(6)
            //    Header checksum: 0xac75[validation disabled]
            //    [Header checksum status: Unverified]
            //    Source: 172.30.64.6
            //    Destination: 172.30.64.248


            byte[] testdata = SflowDecoder.StringToByteArray("450005dc6f6b40008006ac75ac1e4006ac1e40f8");

            Ipv4HeaderDecoder header = new Ipv4HeaderDecoder();
            var result = header.DecodeIpv4Header(testdata);
            Assert.AreEqual((byte)4, result.Version);
            Assert.AreEqual((byte)20, result.IHL);
            Assert.AreEqual((byte)0, result.TOS);
            Assert.AreEqual((ushort)1500, result.TotalLength);
            Assert.AreEqual((ushort)28523, result.ID);
            Assert.AreEqual((byte)2, result.Flags);
            Assert.AreEqual((byte)0, result.FragmentOffset);
            Assert.AreEqual((byte)128, result.TTL);
            Assert.AreEqual((byte)6, result.Protocol);
            Assert.AreEqual((ushort)0xac75, result.Checksum);
            Assert.AreEqual(IPAddress.Parse("172.30.64.6"), result.SrcAddr);
            Assert.AreEqual(IPAddress.Parse("172.30.64.248"), result.DstAddr);
            Assert.AreEqual((ushort)0xac75, result.Checksum);

        }
    }
}
