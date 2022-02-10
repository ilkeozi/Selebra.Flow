using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.Sflow
{
    public class SflowDatagram
    {
        /* sFlow Datagram Version 4 */

        /* Revision History
           - version 4 adds support BGP communities
           - version 3 adds support for extended_url information
        */

        /* sFlow Sample types */

        /* Address Types */

        //typedef opaque ip_v4[4];
        //typedef opaque ip_v6[16];

        public enum address_type
        {
            IP_V4 = 1,
            IP_V6 = 2
        }

        //union address(address_type type)
        //{
        //    case IP_V4:
        //        ip_v4;
        //    case IP_V6:
        //        ip_v6;
        //}

        /* Packet header data */

        const uint MAX_HEADER_SIZE = 256;   /* The maximum sampled header size. */

        /* The header protocol describes the format of the sampled header */
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

        public struct Header
        {
            public Protocol Protocol;       /* Format of sampled header */
            public uint FrameLength;      /* Original length of packet before sampling */
            public byte[] HeaderData;
            //opaque header<MAX_HEADER_SIZE>; /* Header bytes */
        }

        /* Packet IP version 4 data */

        


        /* Packet data */

        public enum PacketInformationType
        {
            HEADER = 1,      /* Packet headers are sampled */
            IPV4 = 2,      /* IP version 4 data */
            IPV6 = 3       /* IP version 6 data */
        }

        //public object packet_data_type(PacketInformationType type)
        //// union packet_data_type(packet_information_type type)
        //{
        //case HEADER:
        //    return new Header()
        //case IPV4:
        //    SampledIPv4 ipv4;
        //case IPV6:
        //    SampledIPv6 ipv6;
        //}

        /* Extended data types */


        /* Extended gateway data */

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

        //     union as_path_type(as_path_segment_type)
        //     {
        //case AS_SET:
        //         uint as_set<>;
        //case AS_SEQUENCE:
        //         uint as_sequence<>;
        //     }

        public struct ExtendedGateway
        {
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

        /* Extended user data */

        public struct extended_user
        {
            // string src_user<>;          /* User ID associated with packet source */
            //string dst_user<>;          /* User ID associated with packet destination */

        }

        /* Extended URL data */

        public enum url_direction
        {
            src = 1,                 /* URL is associated with source
                                  address */
            dst = 2                  /* URL is associated with destination
                                  address */
        }

        struct extended_url
        {
            url_direction direction;    /* URL associated with packet source */
            //string url<>;               /* URL associated with the packet flow */
        }

        /* Extended data */
        public enum extended_information_type
        {
            SWITCH = 1,      /* Extended switch information */
            ROUTER = 2,      /* Extended router information */
            GATEWAY = 3,      /* Extended gateway router information */
            USER = 4,      /* Extended TACACS/RADIUS user information */
            URL = 5       /* Extended URL information */
        }

        //     union extended_data_type(extended_information_type type)
        //     {
        //case SWITCH:
        //         extended_switch switch;
        //case ROUTER:
        //         extended_router router;
        //case GATEWAY:
        //         extended_gateway gateway;
        //case USER:
        //         extended_user user;
        //case URL:
        //         extended_url url;
        //     }

        /* Format of a single flow sample */

        public struct flow_sample
        {
            uint sequence_number;    /* Incremented with each flow sample
                                    generated by this source_id */
            uint source_id;          /* sFlowDataSource encoded as follows:
                                    The most significant byte of the
                                    source_id is used to indicate the
                                    type of sFlowDataSource
                                    (0 = ifIndex,
                                    1 = smonVlanDataSource,
                                    2 = entPhysicalEntry) and the
                                    lower three bytes contain the
                                    relevant index value.*/

            uint sampling_rate;      /* sFlowPacketSamplingRate */
            uint sample_pool;        /* Total number of packets that could
                                    have been sampled (i.e., packets
                                    skipped by sampling process + total
                                    number of samples) */
            uint drops;              /* Number times a packet was dropped
                                    due to lack of resources */

            uint input;               /* SNMP ifIndex of input interface.
                                     0 if interface is not known.  */
            uint output;              /* SNMP ifIndex of output interface,
                                     0 if interface is not known.
                                     Set most significant bit to
                                     indicate multiple destination
                                     interfaces (i.e., in case of
                                     broadcast or multicast)
                                     and set lower order bits to
                                     indicate number of destination
                                     interfaces.
                                     Examples:
                                        0x00000002  indicates ifIndex =
                                                    2
                                        0x00000000  ifIndex unknown.
                                        0x80000007  indicates a packet
                                                    sent to 7
                                                    interfaces.
                                        0x80000000  indicates a packet
                                                    sent to an unknown
                                                    number of interfaces
                                                    greater than 1. */

            // packet_data_type packet_data;       /* Information about sampled packet */
            //extended_data_type extended_data<>; /* Extended flow information */
        }

        /* Counter types */

        /* Generic interface counters - see RFC 2233 */

        public struct IfCounters
        {
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

        /* Ethernet interface counters - see RFC 2358 */

        public struct EthernetCounters
        {
            IfCounters generic;
            public uint Dot3StatsAlignmentErrors;
            uint Dot3StatsFCSErrors;
            uint Dot3StatsSingleCollisionFrames;
            uint Dot3StatsMultipleCollisionFrames;
            uint Dot3StatsSQETestErrors;
            uint Dot3StatsDeferredTransmissions;
            uint Dot3StatsLateCollisions;
            uint Dot3StatsExcessiveCollisions;
            uint Dot3StatsInternalMacTransmitErrors;
            uint Dot3StatsCarrierSenseErrors;
            uint Dot3StatsFrameTooLongs;
            uint Dot3StatsInternalMacReceiveErrors;
            uint Dot3StatsSymbolErrors;
        }

        /* FDDI interface counters - see RFC 1512 */
        public struct fddi_counters
        {
            IfCounters generic;
        }

        /* Token ring counters - see RFC 1748 */

       

        /* WAN counters */

        public struct wan_counters
        {
            IfCounters generic;
        }

        /* VLAN counters */

        struct vlan_counters
        {
            uint vlan_id;
            ulong octets;
            uint ucastPkts;
            uint multicastPkts;
            uint broadcastPkts;
            uint discards;
        }

        /* Counter data */

        enum counters_version
        {
            GENERIC = 1,
            ETHERNET = 2,
            TOKENRING = 3,
            FDDI = 4,
            VG = 5,
            WAN = 6,
            VLAN = 7
        }

        //     union counters_type(counters_version version)
        //     {
        //case GENERIC:
        //         if_counters generic;
        //case ETHERNET:
        //         ethernet_counters ethernet;
        //case TOKENRING:
        //         tokenring_counters tokenring;
        //case FDDI:
        //         fddi_counters fddi;
        //case VG:
        //         vg_counters vg;
        //case WAN:
        //         wan_counters wan;
        //case VLAN:
        //         vlan_counters vlan;
        //     }

        /* Format of a single counter sample */

        struct counters_sample
        {
            uint sequence_number;   /* Incremented with each counter
                                      sample generated by this
                                      source_id */
            uint source_id;         /* sFlowDataSource encoded as
                                      follows:
                                       The most significant byte of the
                                       source_id is used to indicate the
                                       type of sFlowDataSource
                                       (0 = ifIndex,
                                       1 = smonVlanDataSource,
                                       2 = entPhysicalEntry) and the
                                           lower three
                                       bytes contain the relevant
                                       index value.*/

            uint sampling_interval; /* sFlowCounterSamplingInterval*/
            //counters_type counters;
        }

        /* Format of a sample datagram */

        enum sample_types
        {
            FLOWSAMPLE = 1,
            COUNTERSSAMPLE = 2
        }

        //     union sample_type(sample_types sampletype)
        //     {
        //case FLOWSAMPLE:
        //         flow_sample flowsample;
        //case COUNTERSSAMPLE:
        //         counters_sample counterssample;
    }

    public struct sample_datagram_v4
    {
        IPAddress agent_address;           /* IP address of sampling agent,
                                      sFlowAgentAddress. */
        uint sequence_number;  /* Incremented with each sample
                                     datagram generated */
        uint uptime;           /* Current time (in milliseconds since
                                     device last booted).  Should be set
                                     as close to datagram transmission
                                     time as possible.*/
                               //sample_type samples<>;         /* An array of flow, counter and delay   samples */
    }
 
    public enum datagram_version
    {
        VERSION4 = 4
    }

    // union sample_datagram_type(datagram_version version)
    // {
    //case VERSION4:
    //     sample_datagram_v4 datagram;
    // }

    //struct sample_datagram
    //{
    //    sample_datagram_type version;
    //}
}

