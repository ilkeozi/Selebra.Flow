using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.Sflow
{
    public class SourceIdStatistic
    {
        public uint SourceID { get; set; }
        public SourceIdType SourceIDType { get; set; }
        public double DeltaSamplePool { get; set; }
        public double MinSamplePool { get; set; }
        public double MaxSamplePool { get; set; }

        public int SamplesReceived { get; set; }
        public double EffectiveSamplingRatio { get; set; }
    }
}
