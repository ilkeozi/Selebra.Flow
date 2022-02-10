using Selebra.Flow.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selebra.Flow.Producers
{
    public interface IFlowProducer
    {
        Queue<FlowRecordExport> OutputQueue { get; set; }

        int Interval { get; set; }

        int Protocol { get; set; }

        int ListeningPort { get; set; }
        
        void Start(int interval, int listeningport, int protocol);

        void Stop();

        
    }
}
