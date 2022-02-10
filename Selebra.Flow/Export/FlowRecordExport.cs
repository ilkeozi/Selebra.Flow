using Selebra.Flow.Sflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.Export
{
    public class FlowRecordExport
    {
        /// <summary>
        /// IP address of device sending flow data.
        /// </summary>
        public IPAddress FlowSourceIPAddress { get; set; }


        /// <summary>
        /// Direction of the Flow.
        /// </summary>
        /// <value>Flow direction: false = ingress flow(default), true ) egress flow</value>
        public bool Direction { get; set; }

        public uint SourceID { get; set; }
        public SourceIdType SourceIDType { get; set; }


        public PhysicalAddress SrcMacAddress { get; set; }
        public PhysicalAddress DstMacAddress { get; set; }

        /// <summary>
        /// Source IP address.
        /// </summary>
        public IPAddress SourceIpAddress { get; set; }

        /// <summary>
        /// Destination IP address.
        /// </summary>
        public IPAddress DestinationIpAddress { get; set; }

        /// <summary>
        /// SNMP index of input interface.
        /// </summary>
        public long Input { get; set; }

        /// <summary>
        /// SNMP index of output interface.
        /// </summary>
        public long Output { get; set; }

        public long SamplePool { get; set; }
        public long SampleSequenceNumber { get; set; }

        public long SamplingRate { get; set; }

        /// <summary>
        /// Packets in the flow.
        /// </summary>
        public long Packets { get; set; }

        /// <summary>
        /// Total number of Layer 3 bytes in the packets of the flow.
        /// </summary>
        public long Octets { get; set; }
        

        /// <summary>
        /// TCP/UDP source port number or equivalent.
        /// </summary>
        public int SourcePort { get; set; }

        /// <summary>
        /// TCP/UDP destination port number or equivalent.
        /// </summary>
        public int DestinationPort { get; set; }


        /// <summary>
        /// Cumulative OR of TCP flags.
        /// </summary>
        public int TcpFlags { get; set; }

        /// <summary>
        /// IP protocol type (for example, TCP = 6; UDP = 17).
        /// </summary>
        public int Protocol { get; set; }

        /// <summary>
        /// IP type of service (ToS).
        /// </summary>
        public int Tos { get; set; }
              
    }
}
