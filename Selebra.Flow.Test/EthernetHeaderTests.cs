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
    public class EthernetHeaderTests
    {
        [TestMethod]
        public void TestDecodeEthernetHeader()
        {
            //0cc47a4bb68700d0891530810800

            //0000   0c c4 7a 4b b6 87 00 d0 89 15 30 81 08 00

            //Ethernet II, Src: Dynacolo_15: 30:81(00:d0: 89:15:30:81), Dst: SuperMic_4b: b6: 87(0c: c4:7a: 4b: b6:87)
            //    Destination: SuperMic_4b: b6: 87(0c: c4:7a: 4b: b6:87)
            //    Source: Dynacolo_15: 30:81(00:d0: 89:15:30:81)
            //    Type: IPv4(0x0800)

            byte[] testdata = SflowDecoder.StringToByteArray("0cc47a4bb68700d0891530810800");

            EthernetHeaderDecoder decoder = new EthernetHeaderDecoder();
            var result = decoder.DecodeEthernetHeader(testdata);
            CollectionAssert.AreEqual(new byte[] { 0x0c, 0xc4, 0x7a, 0x4b, 0xb6, 0x87 }, result.DstAddress);
            CollectionAssert.AreEqual(new byte[] { 0x00, 0xd0, 0x89, 0x15, 0x30, 0x81 }, result.SrcAddress);
            Assert.AreEqual(0x0800, result.TypeLength);


        }
    }
}
