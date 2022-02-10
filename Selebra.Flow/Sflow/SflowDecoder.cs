using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Selebra.Flow.Sflow.SflowPacket;

namespace Selebra.Flow.Sflow
{
    public class SflowDecoder
    {
      
        /// <summary>
        /// Decodes the header of the sflow datagram.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <returns>SflowPacket with populated Header Data</returns>
        public SflowPacket DecodePacketHeader(byte[] payload)
        {
            int offset = 0;
            SflowPacket packet = new SflowPacket();
            packet.Version = ToUInt32BigEndian(payload, offset);
            packet.IpVersion = (IpVersion)ToUInt32BigEndian(payload, offset += 4);

            if (packet.IpVersion == IpVersion.IP_V4)
            {
                ArraySegment<byte> agentipv4bytes = new ArraySegment<byte>(payload, offset += 4, 4);
                packet.AgentIP = new IPAddress(agentipv4bytes.ToArray());
                packet.SubAgentId = ToUInt32BigEndian(payload, offset += 4);
                packet.SequenceNumber = ToUInt32BigEndian(payload, offset += 4);
                packet.Uptime = ToUInt32BigEndian(payload, offset += 4);
                packet.SamplesCount = ToUInt32BigEndian(payload, offset += 4);
            }
            else if (packet.IpVersion == IpVersion.IP_V6)
            {
                ArraySegment<byte> agentipv4bytes = new ArraySegment<byte>(payload, offset += 16, 16);
                packet.AgentIP = new IPAddress(agentipv4bytes.ToArray());
                packet.SubAgentId = ToUInt32BigEndian(payload, offset += 4);
                packet.SequenceNumber = ToUInt32BigEndian(payload, offset += 4);
                packet.Uptime = ToUInt32BigEndian(payload, offset += 4);
                packet.SamplesCount = ToUInt32BigEndian(payload, offset += 4);
            }
            else
            {
                throw new ArgumentOutOfRangeException("IpVersion is out of range.");
            }
            packet.Offset = offset + 4;            
            return packet;
        }

        /// <summary>
        /// Decodes header of a sflow sample in a payload starting from the offset.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="offset">Starting point offset.</param>
        /// <returns></returns>
        public SampleHeader DecodeSampleHeader(byte[] payload, int offset)
        {
            ArraySegment<byte> sampleheaderbytes = new ArraySegment<byte>(payload, offset, 4);


            uint valuein = ToUInt32BigEndian(sampleheaderbytes.ToArray(), 0);
            //byte mask4bit = 0xF; // 00001111
            //byte mask8bit = 0xFF; // 11111111
            uint format = valuein & 0x0fff;
            uint enterprise = valuein >> 12 & 0x0fff;


            SampleHeader sampleheader = new SampleHeader();
            sampleheader.Format = (SampleFormat)format;
            sampleheader.Length = ToUInt32BigEndian(payload, offset += 4);
            sampleheader.SampleSequenceNumber = ToUInt32BigEndian(payload, offset += 4);
            sampleheader.SourceIdType = (SourceIdType)(uint)payload[offset += 4];
            sampleheader.SourceIdValue = (uint)(payload[offset += 1] << 16 | payload[offset += 1] << 8 | payload[offset += 1]);
            sampleheader.Offset = offset + 1;
            return sampleheader;
        }

        /// <summary>
        /// Decodes a Counter sample specified within a Sample header.
        /// </summary>
        /// <param name="header">Samples generic Header portion specifiying offset and format of the sample.</param>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <returns></returns>
        public CounterSample DecodeCounterSample(SampleHeader header, byte[] payload)
        {
            int offset = header.Offset;
            CounterSample countersample = new CounterSample();
            countersample.Header = header;
            countersample.Type = typeof(CounterSample);

            countersample.CounterRecordsCount = ToUInt32BigEndian(payload, offset);
            countersample.Offset = offset + 4;
            return countersample;
        }

        /// <summary>
        /// Decodes a record header whether CounterRecord or FlowRecord.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="offset">Starting point offset.</param>
        /// <returns>RecordHeader including Record Format, Length and offset data.</returns>
        public RecordHeader DecodeRecordHeader(byte[] payload, int offset)
        {
            RecordHeader record = new RecordHeader();
            ArraySegment<byte> formatbytes = new ArraySegment<byte>(payload, offset, 4);

            uint valuein = ToUInt32BigEndian(formatbytes.ToArray(), 0);
            //byte mask4bit = 0xF; // 00001111
            //byte mask8bit = 0xFF; // 11111111
            uint format = valuein & 0x0fff;
            uint enterprise = valuein >> 12 & 0x0fff;
            record.DataFormat = format;
            record.Length = ToUInt32BigEndian(payload, offset += 4);
            record.Offset = offset+4;
            return record;
        }

        /// <summary>
        /// Decodes a Raw Packet Header with Format 1 of a Flow Data Record.
        /// Should not be confused sFlow sample,record or packet headers.
        /// This is a part of Flow Data Sample with Format 1.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="header">Record Header Information including starting offset.</param>
        /// <returns>FlowDataHeader (Type Format 1 - Raw Packet Header)</returns>
        public FlowDataHeader DecodeFlowDataHeader(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            FlowDataHeader flowdataheader = new FlowDataHeader();
            flowdataheader.Header = header;
            flowdataheader.Type = typeof(FlowDataHeader);

            flowdataheader.Protocol = (Protocol)ToUInt32BigEndian(payload, offset);
            flowdataheader.Framelength = ToUInt32BigEndian(payload, offset += 4);
            flowdataheader.Stripped = ToUInt32BigEndian(payload, offset += 4);
            flowdataheader.Headersize = ToUInt32BigEndian(payload, offset += 4);

            ArraySegment<byte> headersampled = new ArraySegment<byte>(payload, offset += 4, (int)flowdataheader.Headersize);
            flowdataheader.HeaderSampled = headersampled.ToArray();

            return flowdataheader;
        }

        /// <summary>
        /// Decodes a Generic Interface Counter with Format 1 of a Counter Data Record.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="header">Record Header Information including starting offset.</param>
        /// <returns>IfCounter (Type Format 1 - Generic Interface Counter)</returns>
        public IfCounter DecodeIfCounter(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            IfCounter counter = new IfCounter();
            counter.Header = header;
            counter.Type = typeof(IfCounter);

            counter.IfIndex = ToUInt32BigEndian(payload, offset);
            counter.IfType = ToUInt32BigEndian(payload, offset += 4);
            counter.IfSpeed = ToUInt64BigEndian(payload, offset += 4);
            counter.IfDirection = ToUInt32BigEndian(payload, offset += 8);
            counter.IfStatus = ToUInt32BigEndian(payload, offset += 4);
            counter.IfInOctets = ToUInt64BigEndian(payload, offset += 4);
            counter.IfInUcastPkts = ToUInt32BigEndian(payload, offset += 8);
            counter.IfInMulticastPkts = ToUInt32BigEndian(payload, offset += 4);
            counter.IfInBroadcastPkts = ToUInt32BigEndian(payload, offset += 4);
            counter.IfInDiscards = ToUInt32BigEndian(payload, offset += 4);
            counter.IfInErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.IfInUnknownProtos = ToUInt32BigEndian(payload, offset += 4);
            counter.IfOutOctets = ToUInt64BigEndian(payload, offset += 4);
            counter.IfOutUcastPkts = ToUInt32BigEndian(payload, offset += 8);
            counter.IfOutMulticastPkts = ToUInt32BigEndian(payload, offset += 4);
            counter.IfOutBroadcastPkts = ToUInt32BigEndian(payload, offset += 4);
            counter.IfOutDiscards = ToUInt32BigEndian(payload, offset += 4);
            counter.IfOutErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.IfPromiscuousMode = ToUInt32BigEndian(payload, offset += 4);

            return counter;
        }

        /// <summary>
        /// Decodes a Ethernet Interface Counter with Format 2 of a Counter Data Record.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="header">Record Header Information including starting offset.</param>
        /// <returns>EthernetCounter (Type Format 2 - Ethernet Interface Counter)</returns>
        public EthernetCounter DecodeEthernetCounter(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            EthernetCounter counter = new EthernetCounter();
            counter.Header = header;
            counter.Type = typeof(EthernetCounter);
            counter.Dot3StatsAlignmentErrors = ToUInt32BigEndian(payload, offset);
            counter.Dot3StatsFCSErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsSingleCollisionFrames = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsMultipleCollisionFrames = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsSQETestErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsDeferredTransmissions = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsLateCollisions = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsExcessiveCollisions = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsInternalMacTransmitErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsCarrierSenseErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsFrameTooLongs = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsInternalMacReceiveErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot3StatsSymbolErrors = ToUInt32BigEndian(payload, offset += 4);
            return counter;
        }

        /// <summary>
        /// Decodes a Token Ring Counter with Format 3 of a Counter Data Record.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="header">Record Header Information including starting offset.</param>
        /// <returns>TokenRingCounter (Type Format 3 - Token Ring Counter)</returns>
        public TokenRingCounter DecodeTokenRingCounter(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            TokenRingCounter counter = new TokenRingCounter();
            counter.Header = header;
            counter.Type = typeof(TokenRingCounter);

            counter.Dot5StatsLineErrors = ToUInt32BigEndian(payload, offset);
            counter.Dot5StatsBurstErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsACErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsAbortTransErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsInternalErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsLostFrameErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsReceiveCongestions = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsFrameCopiedErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsTokenErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsSoftErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsHardErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsSignalLoss = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsTransmitBeacons = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsRecoverys = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsLobeWires = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsRemoves = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsSingles = ToUInt32BigEndian(payload, offset += 4);
            counter.Dot5StatsFreqErrors = ToUInt32BigEndian(payload, offset += 4);

            return counter;
        }

        /// <summary>
        /// Decodes a 100 BaseVG Interface Counter with Format 4 of a Counter Data Record.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="header">Record Header Information including starting offset.</param>
        /// <returns>VgCounter (Type Format 4 - 100 BaseVG Interface Counter)</returns>
        public VgCounter DecodeVgCounter(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            VgCounter counter = new VgCounter();
            counter.Header = header;
            counter.Type = typeof(VgCounter);

            counter.dot12InHighPriorityFrames = ToUInt32BigEndian(payload, offset);
            counter.dot12InHighPriorityOctets = ToUInt64BigEndian(payload, offset += 4);
            counter.dot12InNormPriorityFrames = ToUInt32BigEndian(payload, offset += 8);
            counter.dot12InNormPriorityOctets = ToUInt64BigEndian(payload, offset += 4);
            counter.dot12InIPMErrors = ToUInt32BigEndian(payload, offset += 8);
            counter.dot12InOversizeFrameErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.dot12InDataErrors = ToUInt32BigEndian(payload, offset += 4);
            counter.dot12InNullAddressedFrames = ToUInt32BigEndian(payload, offset += 4);
            counter.dot12OutHighPriorityFrames = ToUInt32BigEndian(payload, offset += 4);
            counter.dot12OutHighPriorityOctets = ToUInt64BigEndian(payload, offset += 4);
            counter.dot12TransitionIntoTrainings = ToUInt32BigEndian(payload, offset += 8);
            counter.dot12HCInHighPriorityOctets = ToUInt64BigEndian(payload, offset += 4);
            counter.dot12HCInNormPriorityOctets = ToUInt64BigEndian(payload, offset += 8);
            counter.dot12HCOutHighPriorityOctets = ToUInt64BigEndian(payload, offset += 8);            

            return counter;
        }


        /// <summary>
        /// Decodes a Vlan Counter with Format 5 of a Counter Data Record.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="header">Record Header Information including starting offset.</param>
        /// <returns>VlanCounter (Type Format 5 - Vlan Counter)</returns>
        public VlanCounter DecodeVlanCounter(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            VlanCounter counter = new VlanCounter();
            counter.Header = header;
            counter.Type = typeof(VlanCounter);

            counter.VlanId = ToUInt32BigEndian(payload, offset);
            counter.Octets = ToUInt64BigEndian(payload, offset += 4);
            counter.UcastPkts = ToUInt32BigEndian(payload, offset += 8);
            counter.MulticastPkts = ToUInt32BigEndian(payload, offset += 4);
            counter.BroadcastPkts = ToUInt32BigEndian(payload, offset += 4);
            counter.Discards = ToUInt32BigEndian(payload, offset += 4);            

            return counter;
        }


        /// <summary>
        /// Decodes Proccessor Information with Format 6 of a Counter Data Record.
        /// </summary>
        /// <param name="payload">Byte[] consisting the whole sflow packet payload.</param>
        /// <param name="header">Record Header Information including starting offset.</param>
        /// <returns>ProcessorInformation (Type Format 6 - Proccessor Information)</returns>
        public ProcessorInformation DecodeProcessorCounter(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            ProcessorInformation counter = new ProcessorInformation();
            counter.Header = header;
            counter.Type = typeof(ProcessorInformation);

            counter.Cpu5sPercentage = ToUInt32BigEndian(payload, offset);
            counter.Cpu1mPercentage = ToUInt32BigEndian(payload, offset += 4);
            counter.Cpu5mPercentage = ToUInt32BigEndian(payload, offset += 4);
            counter.TotalMemory = ToUInt64BigEndian(payload, offset += 4);
            counter.FreeMemory = ToUInt64BigEndian(payload, offset += 8);

            return counter;
        }

        public SampledEthernet DecodeSampledEthernet(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            SampledEthernet ethernet = new SampledEthernet();
            ethernet.Header = header;
            ethernet.Type = typeof(SampledEthernet);

            ethernet.Length = ToUInt32BigEndian(payload, offset);

            ArraySegment<byte> srcmac = new ArraySegment<byte>(payload, offset += 4, 6);
            ethernet.SrcMac = srcmac.ToArray();
            
            ArraySegment<byte> dstmac = new ArraySegment<byte>(payload, offset += 8, 6);

            ethernet.DstMac = dstmac.ToArray();
            ethernet.EthType = ToUInt32BigEndian(payload, offset += 8);
            return ethernet;
        }

        public SampledIPv4 DecodeSampledIPv4(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            SampledIPv4 ipv4 = new SampledIPv4();
            ipv4.Header = header;
            ipv4.Type = typeof(SampledIPv4);
            ipv4.Length = ToUInt32BigEndian(payload, offset);
            ipv4.Protocol = ToUInt32BigEndian(payload, offset += 4);

            ArraySegment<byte> srcipbytes = new ArraySegment<byte>(payload, offset += 4, 4);
            ipv4.SrcIp = new IPAddress(srcipbytes.ToArray());

            ArraySegment<byte> dstipbytes = new ArraySegment<byte>(payload, offset += 4, 4);

            ipv4.DstIp = new IPAddress(srcipbytes.ToArray());
            ipv4.SrcPort = ToUInt32BigEndian(payload, offset += 4);
            ipv4.DstPort = ToUInt32BigEndian(payload, offset += 4);
            ipv4.TcpFlags = ToUInt32BigEndian(payload, offset += 4);
            ipv4.Tos = ToUInt32BigEndian(payload, offset += 4);
            return ipv4;
        }

        public SampledIPv6 DecodeSampledIPv6(byte[] payload, RecordHeader header)
        {
            int offset = header.Offset;
            SampledIPv6 ipv6 = new SampledIPv6();
            ipv6.Header = header;
            ipv6.Type = typeof(SampledIPv6);
            ipv6.Length = ToUInt32BigEndian(payload, offset);
            ipv6.Protocol = ToUInt32BigEndian(payload, offset += 4);

            ArraySegment<byte> srcipbytes = new ArraySegment<byte>(payload, offset += 16, 16);
            ipv6.SrcIp = new IPAddress(srcipbytes.ToArray());

            ArraySegment<byte> dstipbytes = new ArraySegment<byte>(payload, offset += 16, 16);

            ipv6.DstIp = new IPAddress(srcipbytes.ToArray());
            ipv6.SrcPort = ToUInt32BigEndian(payload, offset += 4);
            ipv6.DstPort = ToUInt32BigEndian(payload, offset += 4);
            ipv6.TcpFlags = ToUInt32BigEndian(payload, offset += 4);
            ipv6.Priority = ToUInt32BigEndian(payload, offset += 4);
            return ipv6;
        }
        public ExtendedSwitch DecodeExtendedSwitch(byte[] payload,RecordHeader header)
        {
            int offset = header.Offset;
            ExtendedSwitch extendedswitch = new ExtendedSwitch();
            extendedswitch.Header = header;
            extendedswitch.Type = typeof(ExtendedSwitch);
            extendedswitch.SrcVlan = ToUInt32BigEndian(payload, offset);
            extendedswitch.SrcPriority = ToUInt32BigEndian(payload, offset += 4);
            extendedswitch.DstVlan = ToUInt32BigEndian(payload, offset += 4);
            extendedswitch.DstPriority = ToUInt32BigEndian(payload, offset += 4);
            return extendedswitch;
        }
        public ExtendedRouter DecodeExtendedRouter(byte[] payload,RecordHeader header)
        {
            int offset = header.Offset;
            ExtendedRouter extendedrouter = new ExtendedRouter();
            extendedrouter.Header = header;
            extendedrouter.Type = typeof(ExtendedRouter);

            extendedrouter.IpVersion = (IpVersion)ToUInt32BigEndian(payload, offset);

            if (extendedrouter.IpVersion == IpVersion.IP_V4)
            {
                ArraySegment<byte> ipbytes = new ArraySegment<byte>(payload, offset += 4, 4);
                extendedrouter.NextHop = new IPAddress(ipbytes.ToArray());
                extendedrouter.SrcMaskLen = ToUInt32BigEndian(payload, offset += 4);
                extendedrouter.DstMaskLen = ToUInt32BigEndian(payload, offset += 4);
            }
            else
            {
                ArraySegment<byte> ipbytes = new ArraySegment<byte>(payload, offset += 16, 16);
                extendedrouter.NextHop = new IPAddress(ipbytes.ToArray());
                extendedrouter.SrcMaskLen = ToUInt32BigEndian(payload, offset += 4);
                extendedrouter.DstMaskLen = ToUInt32BigEndian(payload, offset += 4);
            }

            return extendedrouter;
        }

        public FlowSample DecodeFlowSample(SampleHeader header, byte[] payload)
        {
            int offset = header.Offset;
            FlowSample sample = new FlowSample();

            sample.Header = header;
            sample.Type = typeof(FlowSample);

            sample.SamplingRate = ToUInt32BigEndian(payload, offset); //44
            sample.SamplePool = ToUInt32BigEndian(payload, offset += 4);
            sample.Drops = ToUInt32BigEndian(payload, offset += 4);
            sample.Input = ToUInt32BigEndian(payload, offset += 4);
            sample.Output = ToUInt32BigEndian(payload, offset += 4);
            sample.FlowRecordsCount = ToUInt32BigEndian(payload, offset += 4);
            sample.Offset = offset+4;
            return sample;

        }

        public SflowPacket DecodeMessage(byte[] payload)
        {

            var packetheader = DecodePacketHeader(payload);

            var headeroffset = packetheader.Offset;

            List<ISample> samples = new List<ISample>();

            for (int i = 0; i < packetheader.SamplesCount; i++)
            {
                var sampleheader = DecodeSampleHeader(payload, headeroffset);
                headeroffset = headeroffset + (int)sampleheader.Length + 8; //TODO: why add 8 ? is there a padding?

                if (sampleheader.Format == SampleFormat.FLOWSAMPLE)
                {
                    FlowSample flowsample = DecodeFlowSample(sampleheader, payload); //OK

                    List<IRecord> flowrecords = DecodeFlowRecords(payload, flowsample);
                    flowsample.Records = flowrecords;
                    samples.Add(flowsample);

                }
                else if (sampleheader.Format == SampleFormat.COUNTERSSAMPLE)
                {
                    CounterSample countersample = DecodeCounterSample(sampleheader, payload);

                    List<IRecord> counterrecords = DecodeCounterRecords(payload, countersample);
                    countersample.Records = counterrecords;
                    samples.Add(countersample);
                }

            }

            packetheader.Samples = samples;

            return packetheader;
        }

        private List<IRecord> DecodeCounterRecords(byte[] payload, CounterSample countersample)
        {
            List<IRecord> counterrecords = new List<IRecord>();
            int offset = countersample.Offset;

            for (int i = 0; i < countersample.CounterRecordsCount; i++)
            {
                RecordHeader recordheader = DecodeRecordHeader(payload, offset);

                if (recordheader.DataFormat == 1)
                {
                    IfCounter counter = DecodeIfCounter(payload, recordheader);
                    counterrecords.Add(counter);
                }
                else if (recordheader.DataFormat == 2) //Ethernet Interface Counters
                {
                    EthernetCounter counter = DecodeEthernetCounter(payload, recordheader);
                    counterrecords.Add(counter);
                }
                else if (recordheader.DataFormat == 3) //Token Ring Counters
                {
                    TokenRingCounter counter = DecodeTokenRingCounter(payload, recordheader);
                    counterrecords.Add(counter);
                }
                else if (recordheader.DataFormat == 4) //Vg Counters
                {
                    VgCounter counter = DecodeVgCounter(payload, recordheader);
                    counterrecords.Add(counter);
                }
                else if (recordheader.DataFormat == 5) //Vlan Counters
                {
                    VlanCounter counter = DecodeVlanCounter(payload, recordheader);
                    counterrecords.Add(counter);
                }
                else if (recordheader.DataFormat == 1001) //Processor Information
                {
                    ProcessorInformation counter = DecodeProcessorCounter(payload, recordheader);
                    counterrecords.Add(counter);
                }
                offset = (int)recordheader.Offset + (int)recordheader.Length;
            }

            return counterrecords;
        }

        private List<IRecord> DecodeFlowRecords(byte[] payload, FlowSample flowsample)
        {
            List<IRecord> flowrecords = new List<IRecord>();
            int offset = flowsample.Offset;
            for (int i = 0; i < flowsample.FlowRecordsCount; i++)
            {

                RecordHeader recordheader = DecodeRecordHeader(payload, offset);

                if (recordheader.DataFormat == 1) //Raw Packet Header
                {
                    FlowDataHeader flowdataheader = DecodeFlowDataHeader(payload, recordheader);
                    flowrecords.Add(flowdataheader);
                }
                else if (recordheader.DataFormat == 2) //Ethernet Frame
                {
                    SampledEthernet ethernet = DecodeSampledEthernet(payload, recordheader);  
                    flowrecords.Add(ethernet);
                }
                else if (recordheader.DataFormat == 3) //Ipv4
                {
                    SampledIPv4 ipv4 = DecodeSampledIPv4(payload, recordheader);
                    flowrecords.Add(ipv4);
                }
                else if (recordheader.DataFormat == 4) //Ipv6
                {
                    SampledIPv6 ipv6 = DecodeSampledIPv6(payload, recordheader);
                    flowrecords.Add(ipv6);

                }
                else if (recordheader.DataFormat == 1001) //Extended Switch
                {
                    ExtendedSwitch extendedswitch = DecodeExtendedSwitch(payload, recordheader);
                    flowrecords.Add(extendedswitch);

                }
                else if (recordheader.DataFormat == 1002) //Extended Router
                {
                    ExtendedRouter extendedrouter = DecodeExtendedRouter(payload, recordheader);
                    flowrecords.Add(extendedrouter);
                }

                offset = (int)recordheader.Offset + (int)recordheader.Length;
            }

            return flowrecords;
        }

        public static int ToNumeral(BitArray binary, int length)
        {
            int numeral = 0;
            for (int i = 0; i < length; i++)
            {
                if (binary[i])
                {
                    numeral = numeral | (((int)1) << (length - 1 - i));
                }
            }
            return numeral;
        }
        public static int ToNumeral(BitArray binary, int offset, int length)
        {
            int numeral = 0;
            for (int i = 0; i < length; i++)
            {
                if (binary[i])
                {
                    numeral = numeral | (((int)1) << (length - 1 - i));
                }
            }
            return numeral;
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

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
}
