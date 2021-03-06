using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Selebra.Flow.Sflow;

namespace Selebra.Flow.Test
{
    [TestClass]
    public class SflowDecoderTests
    {

        byte[] TestsFlowRawPacket = new byte[] {0x00, 0x00, 0x00, 0x05, 0x00, 0x00,
    0x00, 0x01, 0x18, 0x03, 0x40, 0x21, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x8d, 0x63, 0x16, 0x1c,
    0x54, 0x89, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xe8, 0xa6, 0x5c,
    0xc8, 0xeb, 0x00, 0x00, 0x03, 0x56, 0x00, 0x00, 0x10, 0x00, 0xcc, 0x8e, 0xc0, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x02, 0x31, 0x00, 0x00, 0x02, 0xc3, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00,
    0x00, 0x01, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x05, 0xee, 0x00, 0x00,
    0x00, 0x04, 0x00, 0x00, 0x00, 0x80, 0x40, 0x55, 0x39, 0x41, 0x04, 0xb8, 0xae, 0x4b, 0xc8, 0x41,
    0x3a, 0xe2, 0x08, 0x00, 0x45, 0x00, 0x05, 0xdc, 0xfa, 0x5d, 0x40, 0x00, 0x3e, 0x06, 0x27, 0x76,
    0x98, 0xc3, 0x21, 0x28, 0x45, 0x2a, 0x16, 0x33, 0x01, 0xbb, 0xd4, 0xd2, 0x81, 0x2c, 0x72, 0x9d,
    0x00, 0x05, 0x6d, 0x6f, 0x50, 0x10, 0x01, 0x6b, 0x03, 0xd0, 0x00, 0x00, 0xbb, 0x6e, 0xa1, 0x32,
    0xf3, 0x60, 0xcf, 0x2c, 0x45, 0x8e, 0x53, 0x02, 0x02, 0x3d, 0xd5, 0xe9, 0xda, 0x9d, 0x59, 0x40,
    0x4f, 0xf8, 0x1a, 0x48, 0x0e, 0x90, 0x16, 0xa0, 0x0a, 0x42, 0x37, 0x20, 0x28, 0x78, 0x36, 0x9f,
    0xdf, 0x7d, 0x7f, 0x8b, 0x80, 0xa2, 0xf3, 0x67, 0x83, 0x41, 0xfd, 0x76, 0xed, 0xac, 0xd7, 0x5b,
    0xbd, 0xcb, 0x5f, 0x5f, 0x65, 0xe4, 0xdc, 0xe4, 0x00, 0xa3, 0x56, 0x22, 0xe8, 0x47, 0x31, 0xc0,
    0x42, 0x8f, 0x87, 0x89, 0xb0, 0x82, 0x00, 0x00, 0x03, 0xe9, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x03, 0xea, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x01, 0xce, 0x48, 0xd2, 0x46, 0x00, 0x00,
    0x00, 0x18, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xe8, 0xa6, 0x5c,
    0xc8, 0xec, 0x00, 0x00, 0x03, 0x56, 0x00, 0x00, 0x10, 0x00, 0xcc, 0x8e, 0xd0, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x02, 0x31, 0x00, 0x00, 0x02, 0xc3, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00,
    0x00, 0x01, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x05, 0xb2, 0x00, 0x00,
    0x00, 0x04, 0x00, 0x00, 0x00, 0x80, 0xd4, 0x6d, 0x50, 0x7f, 0x8a, 0xc9, 0xae, 0x4b, 0xc8, 0x41,
    0x3a, 0xe2, 0x08, 0x00, 0x45, 0x00, 0x05, 0xa0, 0x6a, 0x89, 0x40, 0x00, 0x3e, 0x06, 0x5e, 0xdc,
    0x98, 0xc3, 0x0d, 0x59, 0xac, 0x3a, 0x1b, 0x9c, 0x01, 0xbb, 0xb9, 0xf9, 0x03, 0xfa, 0xad, 0xec,
    0xf3, 0x37, 0xe3, 0x60, 0x50, 0x10, 0x01, 0x28, 0x28, 0xda, 0x00, 0x00, 0xb3, 0x6f, 0xc1, 0x7e,
    0x8a, 0x37, 0x74, 0x95, 0xbc, 0xb9, 0x7c, 0xaa, 0x85, 0x35, 0xcd, 0x05, 0x3f, 0x3a, 0x27, 0xcf,
    0xa8, 0x7d, 0xb0, 0x46, 0x51, 0xfc, 0x5c, 0xb8, 0x83, 0x76, 0xcb, 0x85, 0x2a, 0xb6, 0x42, 0x85,
    0x86, 0xa2, 0x61, 0x57, 0x92, 0xf0, 0x71, 0xf6, 0xa2, 0xa3, 0xfc, 0x58, 0x93, 0x99, 0x88, 0x9f,
    0x56, 0x21, 0x88, 0x22, 0x89, 0x66, 0xe8, 0x7a, 0xb2, 0x2e, 0x98, 0xaf, 0x70, 0xd6, 0xc0, 0x6e,
    0xe4, 0xbd, 0xc5, 0x78, 0x96, 0x05, 0x00, 0x00, 0x03, 0xe9, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x03, 0xea, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x01, 0xce, 0x48, 0xd3, 0x16, 0x00, 0x00,
    0x00, 0x18, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xe8, 0xa6, 0x5c,
    0xc8, 0xed, 0x00, 0x00, 0x03, 0x56, 0x00, 0x00, 0x10, 0x00, 0xcc, 0x8e, 0xe0, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x02, 0x31, 0x00, 0x00, 0x02, 0xc3, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00,
    0x00, 0x01, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x05, 0x9e, 0x00, 0x00,
    0x00, 0x04, 0x00, 0x00, 0x00, 0x80, 0xd4, 0x6d, 0x50, 0x7f, 0x8a, 0xc9, 0xae, 0x4b, 0xc8, 0x41,
    0x3a, 0xe2, 0x08, 0x00, 0x45, 0x02, 0x05, 0x8c, 0xd1, 0xce, 0x40, 0x00, 0x3e, 0x06, 0xe0, 0x45,
    0x98, 0xc3, 0x21, 0x84, 0xac, 0x3a, 0x1e, 0xd4, 0x01, 0xbb, 0x9f, 0xd8, 0xaa, 0x45, 0xdc, 0x86,
    0x6f, 0x4c, 0xfd, 0x41, 0x50, 0x10, 0x01, 0x26, 0x91, 0x45, 0x00, 0x00, 0xd9, 0x89, 0x5f, 0x11,
    0x8f, 0x1c, 0xdc, 0xda, 0x35, 0x98, 0xc4, 0x03, 0xa4, 0x7b, 0x56, 0x11, 0xd3, 0x3d, 0x25, 0xe7,
    0xf9, 0x19, 0x57, 0xd0, 0x44, 0xa2, 0x59, 0x3d, 0xc9, 0x90, 0xca, 0x7a, 0xa5, 0xbf, 0x00, 0x1e,
    0x98, 0x1c, 0x8c, 0x00, 0x4f, 0x5c, 0xf7, 0x89, 0x86, 0xfe, 0x88, 0x2e, 0x32, 0x03, 0x59, 0xbc,
    0x51, 0x06, 0x56, 0xd9, 0x38, 0xe5, 0xbe, 0x6b, 0x79, 0x8a, 0xdf, 0xf8, 0x34, 0x6b, 0x86, 0xc7,
    0xb2, 0x91, 0x4c, 0x11, 0x47, 0x50, 0x00, 0x00, 0x03, 0xe9, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x03, 0xea, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x01, 0xce, 0x48, 0xd3, 0x16, 0x00, 0x00,
    0x00, 0x18, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xac, 0xa6, 0x5c,
    0xc8, 0xee, 0x00, 0x00, 0x03, 0x56, 0x00, 0x00, 0x10, 0x00, 0xcc, 0x8e, 0xf0, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x02, 0xc3, 0x00, 0x00, 0x02, 0x31, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00,
    0x00, 0x01, 0x00, 0x00, 0x00, 0x54, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x46, 0x00, 0x00,
    0x00, 0x04, 0x00, 0x00, 0x00, 0x42, 0xae, 0x4b, 0xc8, 0x41, 0x3a, 0xe2, 0x40, 0x55, 0x39, 0x41,
    0x04, 0xb8, 0x08, 0x00, 0x45, 0x00, 0x00, 0x34, 0xd8, 0xb1, 0x40, 0x00, 0x38, 0x06, 0x06, 0xc5,
    0x68, 0xdc, 0xc5, 0x06, 0x5d, 0xb8, 0xd7, 0xb2, 0x95, 0x98, 0x01, 0xbb, 0xad, 0x33, 0xd4, 0x9c,
    0xf7, 0x0d, 0xcd, 0xc0, 0x80, 0x10, 0x2c, 0xcc, 0x54, 0x73, 0x00, 0x00, 0x01, 0x01, 0x05, 0x0a,
    0xf7, 0x0d, 0xd9, 0x28, 0xf7, 0x0d, 0xef, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x03, 0xe9, 0x00, 0x00,
    0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x03, 0xea, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x01, 0x98, 0xc3,
    0x4d, 0x83, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
    0x00, 0xe8, 0xa6, 0x5c, 0xc8, 0xef, 0x00, 0x00, 0x03, 0x56, 0x00, 0x00, 0x10, 0x00, 0xcc, 0x8f,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x32, 0x00, 0x00, 0x02, 0xc3, 0x00, 0x00,
    0x00, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
    0x05, 0x8a, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x80, 0xd4, 0x6d, 0x50, 0x7f, 0x8a, 0xc9,
    0xae, 0x4b, 0xc8, 0x41, 0x3a, 0xe2, 0x08, 0x00, 0x45, 0x00, 0x05, 0x78, 0xdf, 0xfd, 0x40, 0x00,
    0x3e, 0x06, 0xfe, 0x28, 0xc0, 0xe5, 0xd2, 0xb5, 0xac, 0x3a, 0x19, 0x84, 0x01, 0xbb, 0x9d, 0x0e,
    0xd5, 0xf4, 0x53, 0xb6, 0x01, 0xe7, 0xe1, 0xc4, 0x50, 0x18, 0x01, 0x6b, 0x8b, 0xc9, 0x00, 0x00,
    0xa7, 0xd3, 0xc5, 0x76, 0x7f, 0x47, 0x38, 0xaf, 0x34, 0xc7, 0x01, 0xcb, 0xcc, 0xec, 0xa3, 0xc4,
    0x73, 0xac, 0xa9, 0xa2, 0x83, 0x26, 0x09, 0x43, 0x98, 0x8d, 0x88, 0x88, 0x84, 0x71, 0x8a, 0x21,
    0x72, 0xe0, 0xd6, 0x09, 0xf4, 0x31, 0x31, 0x4f, 0x18, 0xb3, 0x81, 0x71, 0xc3, 0x91, 0x52, 0xa0,
    0x73, 0xed, 0x97, 0xde, 0xa2, 0x2d, 0xff, 0x27, 0xd4, 0xb7, 0x8c, 0x9b, 0x3b, 0xb3, 0x92, 0x5b,
    0xdc, 0x6e, 0x51, 0x97, 0xaf, 0xa9, 0xde, 0xec, 0xcb, 0x8a, 0x00, 0x00, 0x03, 0xe9, 0x00, 0x00,
    0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x03, 0xea, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x01, 0xce, 0x48,
    0xd3, 0x16, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x15};

        [TestMethod]
        public void TestDecodePacketHeader()
        {
            SflowDecoder decoder = new SflowDecoder();
            var result = decoder.DecodePacketHeader(TestsFlowRawPacket);
            Assert.AreEqual((uint)5, result.Version, "Expected Version 5, actual " + result.Version);
            Assert.AreEqual((uint)370955401, result.Uptime, "Expected Uptime 370955401 , actual " + result.Version);
            Assert.AreEqual(IpVersion.IP_V4, result.IpVersion, "Expected IpVersion IP_V4 actual " + result.IpVersion);
            Assert.AreEqual((uint)5, result.SamplesCount, "Expected SamplesCount 5 actual " + result.SamplesCount);
            Assert.AreEqual((uint)36195, result.SequenceNumber, "Expected SequenceNumber 36195 actual " + result.SequenceNumber);
            Assert.AreEqual("24.3.64.33", result.AgentIP.ToString(), "Expected AgentIP 24.3.64.33 actual " + result.AgentIP);
        }

        [TestMethod]
        public void TestDecodeSampleHeader()
        {
            SflowDecoder decoder = new SflowDecoder();
            var result = decoder.DecodeSampleHeader(TestsFlowRawPacket, 28);

            Assert.AreEqual(SampleFormat.FLOWSAMPLE, result.Format, "Expected Format FLOWSAMPLE, actual " + result.Format);
            Assert.AreEqual((uint)232, result.Length, "Expected Length 232, actual " + result.Length);
            Assert.AreEqual((uint)0xa65cc8eb, result.SampleSequenceNumber, "Expected SampleSequenceNumber 0xa65cc8eb, actual " + result.SampleSequenceNumber);
            Assert.AreEqual(SourceIdType.IfIndex, result.SourceIdType, "Expected SourceIdType IfIndex, actual " + result.SourceIdType);
            Assert.AreEqual((uint)854, result.SourceIdValue, "Expected SourceIdValue 854, actual " + result.SourceIdValue);
        }


        [TestMethod]
        public void TestDecodeSampleHeaders()
        {
            
            SflowDecoder decoder = new SflowDecoder();
            int[] sizes = new int[] { 232, 232, 232, 172, 232 };

            long offset = 28;
            for (int i = 0; i < 5; i++)
            {
                var header = decoder.DecodeSampleHeader(TestsFlowRawPacket, (int)offset);
                offset = offset + header.Length + 8;
                Assert.AreEqual(SampleFormat.FLOWSAMPLE, header.Format);
                Assert.AreEqual(sizes[i], (int)header.Length, "Excpected Length: " + sizes[i] + " got " + header.Length);
            }
        }


        [TestMethod]
        public void TestDecodeFlowSample()
        {
            SflowDecoder decoder = new SflowDecoder();
            var resultheader = decoder.DecodeSampleHeader(TestsFlowRawPacket, 28);
            var result = decoder.DecodeFlowSample(resultheader, TestsFlowRawPacket);

            Assert.AreEqual((uint)0x1000, result.SamplingRate, "Expected SamplingRate 0x1000, actual " + result.SamplingRate);
            Assert.AreEqual((uint)0xcc8ec000, result.SamplePool, "Expected SamplePool  0xcc8ec000 , actual " + result.SamplePool); //
            Assert.AreEqual((uint)0, result.Drops, "Expected Drops  0, actual " + result.Drops);
            Assert.AreEqual((uint)0x231, result.Input, "Expected Input  0x231 , actual " + result.Input);
            Assert.AreEqual((uint)0x2c3, result.Output, "Expected Output   0x2c3  , actual " + result.Output);
            Assert.AreEqual((uint)0x3, result.FlowRecordsCount, "Expected FlowRecordsCount  0x3, actual " + result.FlowRecordsCount);

        }

        [TestMethod]
        public void TestDecodeCounterRecords()
        {
            
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeMessage()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeExtendedRouter()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeExtendedSwitch()
        {
            //       |0 Record Header    |Extended Switch Data   16|
            //0000   00 00 03 e9 00 00 00 10 00 00 00 37 ff ff ff ff
            //0010   00 00 00 37 00 00 00 00

            //Extended switch data
            //0000 0000 0000 0000 0000.... .... .... = Enterprise: standard sFlow(0)
            //Format: Extended switch data(1001)
            //Flow data length(byte): 16
            //Incoming 802.1Q VLAN: 55
            //Incoming 802.1p priority: 4294967295
            //Outgoing 802.1Q VLAN: 55
            //Outgoing 802.1p priority: 0

            byte[] testdata = SflowDecoder.StringToByteArray("000003e90000001000000037ffffffff0000003700000000");
            RecordHeader header = new RecordHeader() { DataFormat = 1001, Offset = 8, Length = 16 };

            SflowDecoder decoder = new SflowDecoder();
            var result = decoder.DecodeExtendedSwitch(testdata, header);

            Assert.AreEqual((uint)55, result.SrcVlan);
            Assert.AreEqual((uint)4294967295, result.SrcPriority);
            Assert.AreEqual((uint)55, result.DstVlan);
            Assert.AreEqual((uint)0, result.DstPriority); 

        }

        [TestMethod]
        public void TestDecodeSampledIPv6()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeSampledIPv4()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeSampledEthernet()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeProcessorCounter()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeVlanCounter()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeVgCounter()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestTokenRingCounter()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeEthernetCounter()
        {


            //       |0 Record Header    |Ethernet  Counter Data 16|
            //0000   00 00 00 02 00 00 00 34 00 00 00 01 00 00 00 02
            //0010   00 00 00 03 00 00 00 04 00 00 00 05 00 00 00 06
            //0020   00 00 00 07 00 00 00 08 00 00 00 09 00 00 00 01
            //0030   00 00 00 02 00 00 00 03 00 00 00 04

            //Ethernet interface counters
            //    0000 0000 0000 0000 0000 .... .... .... = Enterprise: standard sFlow(0)
            //    .... .... .... .... .... 0000 0000 0010 = Format: Ethernet interface counters (2)
            //    Flow data length(byte): 52
            //    Alignment Errors: 1
            //    FCS Errors: 2
            //    Single Collision Frames: 3
            //    Multiple Collision Frames: 4
            //    SQE Test Errors: 5
            //    Deferred Transmissions: 6
            //    Late Collisions: 7
            //    Excessive Collisions: 8
            //    Internal Mac Transmit Errors: 9
            //    Carrier Sense Errors: 1
            //    Frame Too Longs: 2
            //    Internal Mac Receive Errors: 3
            //    Symbol Errors: 4

            //                 |0                      |4                      |8                      |12                  16|
            byte[] testdata = { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x34, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02,
                                0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x06,
                                0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x01,
                                0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04  };

            SflowDecoder decoder = new SflowDecoder();
            RecordHeader header = new RecordHeader() { DataFormat = 1, Offset = 8, Length = 52 };
            var result = decoder.DecodeEthernetCounter(testdata, header);

            Assert.AreEqual((uint)1, result.Dot3StatsAlignmentErrors);
            Assert.AreEqual((uint)2, result.Dot3StatsFCSErrors);
            Assert.AreEqual((uint)3, result.Dot3StatsSingleCollisionFrames);
            Assert.AreEqual((uint)4, result.Dot3StatsMultipleCollisionFrames);
            Assert.AreEqual((uint)5, result.Dot3StatsSQETestErrors);
            Assert.AreEqual((uint)6, result.Dot3StatsDeferredTransmissions);
            Assert.AreEqual((uint)7, result.Dot3StatsLateCollisions);
            Assert.AreEqual((uint)8, result.Dot3StatsExcessiveCollisions);
            Assert.AreEqual((uint)9, result.Dot3StatsInternalMacTransmitErrors);
            Assert.AreEqual((uint)1, result.Dot3StatsCarrierSenseErrors);
            Assert.AreEqual((uint)2, result.Dot3StatsFrameTooLongs);
            Assert.AreEqual((uint)3, result.Dot3StatsInternalMacReceiveErrors);
            Assert.AreEqual((uint)4, result.Dot3StatsSymbolErrors);
        }

        [TestMethod]
        public void TestDecodeIfCounter()
        {
            //       |0 Record Header    |Interface Counter Data 16|
            //0000   00 00 00 01 00 00 00 58 00 00 00 08 00 00 00 06
            //0010   00 00 00 00 3b 9a ca 00 00 00 00 01 00 00 00 03
            //0020   00 00 00 0c 37 aa 51 ff 00 9e b3 e7 17 b7 de d0
            //0030   04 06 85 95 00 00 00 00 00 00 00 00 00 00 00 00
            //0040   00 00 00 24 9d 70 d0 cc 2a b5 bf 66 2c a5 f7 42
            //0050   07 64 69 87 00 3a d3 d6 00 00 00 00 00 00 00 00

            //Generic interface counters
            //    0000 0000 0000 0000 0000 .... .... .... = Enterprise: standard sFlow(0)
            //    .... .... .... .... .... 0000 0000 0001 = Format: Generic interface counters (1)
            //    Flow data length(byte): 88
            //    Interface index: 8
            //    Interface Type: 6
            //    Interface Speed: 1000000000
            //    Interface Direction: Full-Duplex(1)
            //    .... .... .... .... .... .... .... ...1 = IfAdminStatus: Up
            //    .... .... .... .... .... .... .... ..1. = IfOperStatus: Up
            //    Input Octets: 52473516543
            //    Input Packets: 10400743
            //    Input Multicast Packets: 397926096
            //    Input Broadcast Packets: 67536277
            //    Input Discarded Packets: 0
            //    Input Errors: 0
            //    Input Unknown Protocol Packets: 0
            //    Output Octets: 157260239052
            //    Output Packets: 716554086
            //    Output Multicast Packets: 749074242
            //    Output Broadcast Packets: 124021127
            //    Output Discarded Packets: 3855318
            //    Output Errors: 0
            //    Promiscuous Mode: 0

            byte[] testdata = SflowDecoder.StringToByteArray("00000001000000580000000800000006000000003b9aca0000000001000000030000000c37aa51ff009eb3e717b7ded004068595000000000000000000000000000000249d70d0cc2ab5bf662ca5f74207646987003ad3d60000000000000000");
            SflowDecoder decoder = new SflowDecoder();
            RecordHeader header = new RecordHeader() { DataFormat = 1, Offset = 8, Length = 88 };
            var result = decoder.DecodeIfCounter(testdata, header);

            Assert.AreEqual((uint)8, result.IfIndex);
            Assert.AreEqual((uint)6, result.IfType);
            Assert.AreEqual((uint)1000000000, result.IfSpeed);
            Assert.AreEqual((ulong)52473516543, result.IfInOctets);
            Assert.AreEqual((uint)10400743, result.IfInUcastPkts);
            Assert.AreEqual((uint)397926096, result.IfInMulticastPkts);
            Assert.AreEqual((uint)67536277, result.IfInBroadcastPkts);
            Assert.AreEqual((uint)0, result.IfInDiscards);
            Assert.AreEqual((uint)0, result.IfInErrors);
            Assert.AreEqual((uint)0, result.IfInUnknownProtos);
            Assert.AreEqual((ulong)157260239052, result.IfOutOctets);
            Assert.AreEqual((uint)716554086, result.IfOutUcastPkts);
            Assert.AreEqual((uint)749074242, result.IfOutMulticastPkts);
            Assert.AreEqual((uint)124021127, result.IfOutBroadcastPkts);
            Assert.AreEqual((uint)3855318, result.IfOutDiscards);
            Assert.AreEqual((uint)0, result.IfOutErrors);
            Assert.AreEqual((uint)0, result.IfPromiscuousMode);

            Assert.Inconclusive("Admin Status and Oper Status not correctly designed");

        }

        [TestMethod]
        public void TestDecodeFlowDataHeader()
        {
            //       |0 Record Header    |Raw Packet Header      16|
            //0000   00 00 00 01 00 00 00 4c 00 00 00 01 00 00 00 44
            //0010   00 00 00 08 00 00 00 3c 00 d0 89 14 f9 6b 0c c4
            //0020   7a 4b b6 84 08 00 45 00 00 28 4e fe 40 00 80 06
            //0030   d3 0d ac 1e 40 05 ac 1e 40 82 d1 f8 02 2a fc 99
            //0040   8f 77 ac f7 0f cb 50 10 ff 37 ba e1 00 00 00 00
            //0050   00 00 00 00

            ///FIRST 8 BYTES INCLUDES FLOW RECORD 4 BYTE FORMAT 4 BYTE FRAME LENGTH            

            //Raw packet header
            //0000 0000 0000 0000 0000.... .... .... = Enterprise: standard sFlow(0)
            //Format: Raw packet header(1)
            //Flow data length(byte): 76
            //Header protocol: Ethernet(1)
            //Frame Length: 68
            //Payload removed: 8
            //Original packet length: 60
            //Header of sampled packet: 00d08914f96b0cc47a4bb6840800450000284efe40008006...

            byte[] testdata = SflowDecoder.StringToByteArray("000000010000004c0000000100000044000000080000003c00d08914f96b0cc47a4bb6840800450000284efe40008006d30dac1e4005ac1e4082d1f8022afc998f77acf70fcb5010ff37bae10000000000000000");

            SflowDecoder decoder = new SflowDecoder();            
            RecordHeader header = new RecordHeader() { DataFormat = 1, Offset = 8, Length = 76 };
            var result = decoder.DecodeFlowDataHeader(testdata, header);

            Assert.AreEqual(Protocol.ETHERNET_ISO8023, result.Protocol);
            Assert.AreEqual((uint)68, result.Framelength);
            Assert.AreEqual((uint)8, result.Stripped);
            Assert.AreEqual((uint)60, result.Headersize); //Flow Data Length

            byte[] testheadersampledpacket = SflowDecoder.StringToByteArray("00d08914f96b0cc47a4bb6840800450000284efe40008006d30dac1e4005ac1e4082d1f8022afc998f77acf70fcb5010ff37bae10000000000000000");
            CollectionAssert.AreEqual(testheadersampledpacket, result.HeaderSampled);
        }

        [TestMethod]
        public void TestDecodeRecordHeader()
        {
            //       |0 Record Header    |Raw Packet Header      16|
            //0000   00 00 00 01 00 00 00 4c 00 00 00 01 00 00 00 44
            //0010   00 00 00 08 00 00 00 3c 00 d0 89 14 f9 6b 0c c4
            //0020   7a 4b b6 84 08 00 45 00 00 28 4e fe 40 00 80 06
            //0030   d3 0d ac 1e 40 05 ac 1e 40 82 d1 f8 02 2a fc 99
            //0040   8f 77 ac f7 0f cb 50 10 ff 37 ba e1 00 00 00 00
            //0050   00 00 00 00

            //Raw packet header
            //0000 0000 0000 0000 0000.... .... .... = Enterprise: standard sFlow(0)
            //Format: Raw packet header(1)
            //Flow data length(byte): 76

            byte[] testdata = SflowDecoder.StringToByteArray("000000010000004c0000000100000044000000080000003c00d08914f96b0cc47a4bb6840800450000284efe40008006d30dac1e4005ac1e4082d1f8022afc998f77acf70fcb5010ff37bae10000000000000000");

            SflowDecoder decoder = new SflowDecoder();
            var result = decoder.DecodeRecordHeader(testdata, 0);
            Assert.AreEqual((uint)1, result.DataFormat);
            Assert.AreEqual((uint)76, result.Length);
        }

        [TestMethod]
        public void TestDecodeCounterSample()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestDecodeFlowRecords()
        {
            Assert.Inconclusive();
        }
       
    }
}
