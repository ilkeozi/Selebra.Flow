using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.Sflow
{
    public enum IpVersion
    {
        IP_V4 = 1,
        IP_V6 = 2
    }

    /// <summary>
    ///The header protocol describes the format of the sampled header
    /// </summary>
    public enum Protocol
    {
        ETHERNET_ISO8023 = 1,
        ISO88024_TOKENBUS = 2,
        ISO88025_TOKENRING = 3,
        FDDI = 4,
        FRAME_RELAY = 5,
        X25 = 6,
        PPP = 7,
        SMDS = 8,
        AAL5 = 9,
        AAL5_IP = 10, /* e.g., Cisco AAL5 mux */
        IPv4 = 11,
        IPv6 = 12,
        MPLS = 13
    }

    public enum FlowRecordFormat
    {
        RawPacketHeader = 1,
        EthernetFrameData = 2,
        IPv4Data = 3,
        IPv6Data = 4,
        ExtendedSwitchData = 1001,
        ExtendedRouterData = 1002,
        ExtendedGatewayData = 1003,
        ExtendedUserData = 1004,
        ExtendedUrlData = 1005,
        ExtendedMplsData = 1006,
        ExtendedNatData = 1007,
        ExtendedMplsTunnelData = 1008,
        ExtendedMplsVcData = 1009,
        ExtendedMplsFecData = 1010,
        ExtendedMplsLvpFecData = 1011,
        ExtendedVlanTunnelData = 1012
    }
    public struct SflowPacket
    {
        public uint Version;
        public IpVersion IpVersion;
        public IPAddress AgentIP;
        public uint SubAgentId;
        public uint SequenceNumber;
        public uint Uptime;
        public uint SamplesCount;
        public List<ISample> Samples;
        public int Offset;
    }

    public struct SampleHeader
    {
        public SampleFormat Format;
        public uint Length;
        public uint SampleSequenceNumber;
        public SourceIdType SourceIdType;
        public uint SourceIdValue;
        public int Offset;

    }

    public enum SampleFormat
    {
        FLOWSAMPLE = 1,
        COUNTERSSAMPLE = 2,
        EXPANDEDFLOWSAMPLE = 3,
        EXPANDEDCOUNTERSAMPLE = 4

    }

    public struct FlowSample : ISample
    {

        public SampleHeader Header { get; set; }
        public Type Type { get; set; }

        public uint SamplingRate;
        public uint SamplePool;
        public uint Drops;
        public uint Input;
        public uint Output;
        public uint FlowRecordsCount;
        public List<IRecord> Records;
        public int Offset;
    }

    public interface ISample
    {
        SampleHeader Header { get; set; }
        Type Type { get; set; }
    }

    public enum SourceIdType
    {
        IfIndex = 0,
        SmonVlanDataSource = 1,
        EntPhysicalEntry = 2
    }
    public struct CounterSample:ISample
    {

        public SampleHeader Header { get; set; }
        public Type Type { get; set; }        

        public uint CounterRecordsCount;
        public List<IRecord> Records;
        public int Offset;

    }

    public struct ExpandedFlowSample : ISample
    {

        public SampleHeader Header { get; set; }
        public Type Type { get; set; }

        public uint SamplingRate;
        public uint SamplePool;
        public uint Drops;
        public uint InputIfFormat;
        public uint InputIfValue;
        public uint OutputIfFormat;
        public uint OutputIfValue;
        public uint FlowRecordsCount;
        public List<IRecord> Records;
    }

    public struct RecordHeader
    {

        public uint DataFormat;
        public uint Length;
        public int Offset;
    }

    public interface IRecord
    {
        RecordHeader Header { get; set; }
        Type Type { get; set; }
    }

    public struct FlowRecord
    {
        public RecordHeader Header;
        public object Data; //interface{ }
    }

    public struct CounterRecord
    {
        public RecordHeader Header;
        public object Data;// interface{ }
    }

    #region Flow Record Data Types

    /// <summary>
    /// Raw Header Data
    /// </summary>
    public struct FlowDataHeader : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }

        public Protocol Protocol;
        public uint Framelength;
        public uint Stripped;
        public uint Headersize;
        public byte[] HeaderSampled;

    }
    public struct SampledEthernet : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }

        public uint Length;
        public byte[] SrcMac;
        public byte[] DstMac;
        public uint EthType;

    }

    /// <summary>
    /// Packet IP version 4 data
    /// </summary>
    public struct SampledIPv4:IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }

        public IpVersion IpVersion;

        /// <summary>
        /// The length of the IP packet excluding lower layer encapsulations.
        /// </summary>
        public uint Length;

        /// <summary>
        /// IP Protocol type.
        /// </summary>
        /// <example>TCP = 6, UDP = 17</example>
        public uint Protocol;

        /// <summary>
        ///  Source IP Address
        /// </summary>
        public IPAddress SrcIp;

        /// <summary>
        ///  Destination IP Address
        /// </summary>
        public IPAddress DstIp;

        /// <summary>
        /// TCP/UDP source port number or equivalent
        /// </summary>
        public uint SrcPort;

        /// <summary>
        /// TCP/UDP destination port number or equivalent
        /// </summary>
        public uint DstPort;

        /// <summary>
        /// TCP Flags.
        /// </summary>
        public uint TcpFlags;

        /// <summary>
        /// IP type of service
        /// </summary>
        public uint Tos;

    }

    /// <summary>
    /// Packet IP version 6 data
    /// </summary>
    public struct SampledIPv6 : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }

        public IpVersion IpVersion;

        /// <summary>
        /// The length of the IP packet excluding lower layer encapsulations.
        /// </summary>
        public uint Length;

        /// <summary>
        /// IP Protocol type.
        /// </summary>
        /// <example>TCP = 6, UDP = 17</example>
        public uint Protocol;

        /// <summary>
        ///  Source IP Address
        /// </summary>
        public IPAddress SrcIp;

        /// <summary>
        ///  Destination IP Address
        /// </summary>
        public IPAddress DstIp;

        /// <summary>
        /// TCP/UDP source port number or equivalent
        /// </summary>
        public uint SrcPort;

        /// <summary>
        /// TCP/UDP destination port number or equivalent
        /// </summary>
        public uint DstPort;

        /// <summary>
        /// TCP Flags.
        /// </summary>
        public uint TcpFlags;

        /// <summary>
        /// IP priority
        /// </summary>
        public uint Priority;
    }



    /// <summary>
    ///  Extended switch data
    /// </summary>
    public struct ExtendedSwitch : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }
        /// <summary>
        /// The 802.1Q VLAN id of incoming frame
        /// </summary>
        public uint SrcVlan;

        /// <summary>
        /// The 802.1p priority of incoming frame
        /// </summary>
        public uint SrcPriority;

        /// <summary>
        /// The 802.1Q VLAN id of outgoing frame
        /// </summary>
        public uint DstVlan;

        /// <summary>
        /// The 802.1p priority of outgoing frame
        /// </summary>
        public uint DstPriority;
    }

    /// <summary>
    /// Extended router data
    /// </summary>
    public struct ExtendedRouter : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }

        public IpVersion IpVersion;
        /// <summary>
        /// IP address of next hop router
        /// </summary>
        public IPAddress NextHop;

        /// <summary>
        /// Source address prefix mask bits
        /// </summary>
        public uint SrcMaskLen;

        /// <summary>
        /// Destination address prefix mask bits
        /// </summary>
        public uint DstMaskLen;
    }


    public enum AsPathSegmentType
    {
        /// <summary>
        /// Unordered set of ASs 
        /// </summary>
        AS_SET = 1,

        /// <summary>
        /// Ordered set of ASs
        /// </summary>
        AS_SEQUENCE = 2
    }
    public struct ExtendedGateway : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }
        /// <summary>
        /// Autonomous system number of router
        /// </summary>
        public uint Asn;

        /// <summary>
        /// Autonomous system number of source
        /// </summary>
        public uint SrcAs;

        /// <summary>
        /// Autonomous system number of source peer
        /// </summary>
        public uint SrcPeerAs;

        public uint AsDestinations;

        public AsPathSegmentType AsPathType;

        public uint AsPathLength;

        public uint AsPath;
        // as_path_type dst_as_path<>; /* Autonomous system path to the destination */

        public uint CommunitiesLength;

        /// <summary>
        /// Communities associated with this   route
        /// </summary>
        public uint[] Communities;

        /// <summary>
        /// LocalPref associated with this route
        /// </summary>
        public uint LocalPref;
    }

    #endregion


    #region Counter Data Types

    /// <summary>
    /// Generic interface counters
    /// </summary>
    /// <see cref="https://tools.ietf.org/html/rfc2233"/>
    public struct IfCounter : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }
        public uint IfIndex;
        public uint IfType;
        public ulong IfSpeed;
        public uint IfDirection;    /* derived from MAU MIB (RFC 2668)
                                   0 = unknown, 1=full-duplex,
                                   2=half-duplex, 3 = in, 4=out */
        public uint IfStatus;       /* bit field with the following bits
                                   assigned
                                   bit 0 = IfAdminStatus
                                     (0 = down, 1 = up)
                                   bit 1 = IfOperStatus
                                     (0 = down, 1 = up) */
        public ulong IfInOctets;
        public uint IfInUcastPkts;
        public uint IfInMulticastPkts;
        public uint IfInBroadcastPkts;
        public uint IfInDiscards;
        public uint IfInErrors;
        public uint IfInUnknownProtos;
        public ulong IfOutOctets;
        public uint IfOutUcastPkts;
        public uint IfOutMulticastPkts;
        public uint IfOutBroadcastPkts;
        public uint IfOutDiscards;
        public uint IfOutErrors;
        public uint IfPromiscuousMode;
    }

    /// <summary>
    /// Ethernet interface counters 
    /// </summary>
    /// <see cref="https://tools.ietf.org/html/rfc2358"/>
    public struct EthernetCounter : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }
        public uint Dot3StatsAlignmentErrors;
        public uint Dot3StatsFCSErrors;
        public uint Dot3StatsSingleCollisionFrames;
        public uint Dot3StatsMultipleCollisionFrames;
        public uint Dot3StatsSQETestErrors;
        public uint Dot3StatsDeferredTransmissions;
        public uint Dot3StatsLateCollisions;
        public uint Dot3StatsExcessiveCollisions;
        public uint Dot3StatsInternalMacTransmitErrors;
        public uint Dot3StatsCarrierSenseErrors;
        public uint Dot3StatsFrameTooLongs;
        public uint Dot3StatsInternalMacReceiveErrors;
        public uint Dot3StatsSymbolErrors;
    }

    /// <summary>
    /// Token Ring Counters 
    /// </summary>
    /// <see cref="https://tools.ietf.org/html/rfc1748"/>
    public struct TokenRingCounter : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }

        public uint Dot5StatsLineErrors;
        public uint Dot5StatsBurstErrors;
        public uint Dot5StatsACErrors;
        public uint Dot5StatsAbortTransErrors;
        public uint Dot5StatsInternalErrors;
        public uint Dot5StatsLostFrameErrors;
        public uint Dot5StatsReceiveCongestions;
        public uint Dot5StatsFrameCopiedErrors;
        public uint Dot5StatsTokenErrors;
        public uint Dot5StatsSoftErrors;
        public uint Dot5StatsHardErrors;
        public uint Dot5StatsSignalLoss;
        public uint Dot5StatsTransmitBeacons;
        public uint Dot5StatsRecoverys;
        public uint Dot5StatsLobeWires;
        public uint Dot5StatsRemoves;
        public uint Dot5StatsSingles;
        public uint Dot5StatsFreqErrors;
    }


    /// <summary>
    /// 100 BaseVG interface counters 
    /// </summary>
    /// <see cref="https://tools.ietf.org/html/rfc2020"/>
    public struct VgCounter : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }
        public uint dot12InHighPriorityFrames;
        public ulong dot12InHighPriorityOctets;
        public uint dot12InNormPriorityFrames;
        public ulong dot12InNormPriorityOctets;
        public uint dot12InIPMErrors;
        public uint dot12InOversizeFrameErrors;
        public uint dot12InDataErrors;
        public uint dot12InNullAddressedFrames;
        public uint dot12OutHighPriorityFrames;
        public ulong dot12OutHighPriorityOctets;
        public uint dot12TransitionIntoTrainings;
        public ulong dot12HCInHighPriorityOctets;
        public ulong dot12HCInNormPriorityOctets;
        public ulong dot12HCOutHighPriorityOctets;
    }

    /// <summary>
    /// Vlan Counters
    /// </summary>
    public struct VlanCounter : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }
        public uint VlanId;
        public ulong Octets;
        public uint UcastPkts;
        public uint MulticastPkts;
        public uint BroadcastPkts;
        public uint Discards;
    }

    /// <summary>
    /// Processor Information
    /// </summary>
    public struct ProcessorInformation : IRecord
    {
        public RecordHeader Header { get; set; }
        public Type Type { get; set; }
        public uint Cpu5sPercentage;
        public uint Cpu1mPercentage;
        public uint Cpu5mPercentage;
        public ulong TotalMemory;
        public ulong FreeMemory;
    }
    #endregion
}
