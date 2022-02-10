using GalaSoft.MvvmLight;
using Selebra.Flow.Sflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Flow_Test_Dashboard.ViewModel
{
    public class DistinctFlowModel:ObservableObject
    {

        public uint SourceID { get; set; }
        public SourceIdType SourceIDType { get; set; }


   


        /// <summary>
        /// Source IP address.
        /// </summary>
        public IPAddress SourceIpAddress { get; set; }

        /// <summary>
        /// Destination IP address.
        /// </summary>
        public IPAddress DestinationIpAddress { get; set; }

        /// <summary>
        /// TCP/UDP source port number or equivalent.
        /// </summary>
        public int SourcePort { get; set; }

        /// <summary>
        /// TCP/UDP destination port number or equivalent.
        /// </summary>
        public int DestinationPort { get; set; }

        /// <summary>
        /// SNMP index of input interface.
        /// </summary>
        public long Input { get; set; }

        /// <summary>
        /// SNMP index of output interface.
        /// </summary>
        public long Output { get; set; }

        /// <summary>
        /// Packets in the flow.
        /// </summary>
        public long Packets { get; set; }

        /// <summary>
        /// IP protocol type (for example, TCP = 6; UDP = 17).
        /// </summary>
        public int Protocol { get; set; }

        public long Count { get; set; }
        public long Total { get; set; }

        public double DeltaSamplePool { get; set; }
        public double MinSamplePool { get; set; }
        public double MaxSamplePool { get; set; }



        public double EffectiveSamplingRatio { get; set; }
        public long Frames { get; set; }



    }
}
