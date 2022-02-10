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
    public class UdpHeaderTests
    {
        [TestMethod]
        public void TestDecodeUdpHeader()
        {
            //dbbb18c704e407a8

            //0000   db bb 18 c7 04 e4 07 a8

            //User Datagram Protocol, Src Port: 56251, Dst Port: 6343
            //    Source Port: 56251
            //    Destination Port: 6343
            //    Length: 1252
            //    Checksum: 0x07a8[unverified]
            //    [Checksum Status: Unverified]
            //    [Stream index: 2]

            byte[] testdata = SflowDecoder.StringToByteArray("dbbb18c704e407a8");

            UdpHeaderDecoder decoder = new UdpHeaderDecoder();
            var result = decoder.DecodeUdpHeader(testdata);
            Assert.AreEqual((ushort)56251,result.SrcPort);
            Assert.AreEqual((ushort)6343, result.DstPort);
            Assert.AreEqual((ushort)1252, result.Length);
            Assert.AreEqual((ushort)0x07a8, result.Checksum);



        }
    }
}
