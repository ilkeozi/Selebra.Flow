using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selebra.Flow.Export;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using Selebra.Flow.Sflow;
using Selebra.Flow.TCPIP;

namespace Selebra.Flow.Producers
{
    public class SflowProducer : IFlowProducer
    {
        private readonly BackgroundWorker workerreceiver = new BackgroundWorker();
        private readonly BackgroundWorker workeraggregator = new BackgroundWorker();

        private UdpClient listener;
        private IPEndPoint groupEP;

        #region IFlowProducer Members
        public Queue<FlowRecordExport> OutputQueue { get; set; }

        public Queue<SflowRecordInput> InputQueue { get; set; }

        public int Interval { get; set; }

        public int Protocol { get; set; }

        public int ListeningPort { get; set; }

        public void Start(int interval, int listeningport, int protocol)
        {
            Interval = interval;
            ListeningPort = listeningport;
            Protocol = protocol;

            listener = new UdpClient(ListeningPort);
            groupEP = new IPEndPoint(IPAddress.Any, ListeningPort);

            workerreceiver.DoWork += Workerreceiver_DoWork; ;
            workerreceiver.RunWorkerAsync();

            workeraggregator.DoWork += Workeraggregator_DoWork; ;
            workeraggregator.RunWorkerAsync();
        }

        public void Stop()
        {
            workerreceiver.CancelAsync();
            workeraggregator.CancelAsync();
        }

        #endregion

        private void Workerreceiver_DoWork(object sender, DoWorkEventArgs e)
        {
            bool done = false;
            SflowDecoder decoder = new SflowDecoder();
            TcpHeaderDecoder tcpdecoder = new TcpHeaderDecoder();
            UdpHeaderDecoder udpdecoder = new UdpHeaderDecoder();
            EthernetHeaderDecoder ethernetdecoder = new EthernetHeaderDecoder();
            Ipv4HeaderDecoder ipdecoder = new Ipv4HeaderDecoder();


            DateTime start = DateTime.Now;
            int flowsamples = 0;
            int messages = 0;
            try
            {
                while (!done)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    var message = decoder.DecodeMessage(bytes);

                    foreach (var sample in message.Samples)
                    {
                        if (sample.Type == typeof(FlowSample))
                        {
                            var flowsample = (FlowSample)sample;
                            foreach (var flowrecord in flowsample.Records)
                            {
                                if (flowrecord.Type == typeof(FlowDataHeader))
                                {
                                    var flowdataheader = (FlowDataHeader)flowrecord;

                                    var ethernet = ethernetdecoder.DecodeEthernetHeader(flowdataheader.HeaderSampled);
                                    //Console.WriteLine("Received Ethernet Frame: SrcAdress={0} , DstAddress={1} , Type={2} ", ethernet.SrcAddress, ethernet.DstAddress, ethernet.TypeLength);

                                    if (ethernet.TypeLength == 0x0800)
                                    {
                                        byte[] ipsegment = new ArraySegment<byte>(flowdataheader.HeaderSampled, 14, flowdataheader.HeaderSampled.Length - 14).ToArray();

                                        var ip = ipdecoder.DecodeIpv4Header(ipsegment);
                                        //Console.WriteLine("Received Ip Datagram:  SrcAdress={0} , DstAddress={1} , Protocol={2}, Length={3}", ip.SrcAddr, ip.DstAddr, ip.Protocol, ip.TotalLength);

                                        SflowRecordInput input = new SflowRecordInput();

                                        if (ip.Protocol == 6)
                                        {
                                            byte[] tcpsegment = new ArraySegment<byte>(ipsegment, ip.IHL, ipsegment.Length - (ip.IHL)).ToArray();

                                            var tcp = tcpdecoder.DecodeTcpHeader(tcpsegment);
                                            //Console.WriteLine("Received Tcp Packet:  SrcPort={0} , DstPort={1}", tcp.SrcPort, tcp.DstPort);
                                            input.DstMacAddress = new System.Net.NetworkInformation.PhysicalAddress(ethernet.DstAddress);
                                            input.SrcMacAddress = new System.Net.NetworkInformation.PhysicalAddress(ethernet.SrcAddress);
                                            input.SourcePort = tcp.SrcPort;
                                            input.DestinationPort = tcp.DstPort;
                                            input.SourceIpAddress = ip.SrcAddr;
                                            input.DestinationIpAddress = ip.DstAddr;
                                            input.Protocol = ip.Protocol;
                                            input.Tos = ip.TOS;
                                            input.TcpFlags = ip.Flags;
                                            input.Octets = ip.TotalLength;
                                            input.Packets = flowsample.SamplingRate;
                                            input.Input = flowsample.Input;
                                            input.Output = flowsample.Output;
                                            input.SamplePool = flowsample.SamplePool;
                                            input.SamplingRate = flowsample.SamplingRate;
                                            input.SampleSequenceNumber = sample.Header.SampleSequenceNumber;
                                            input.SourceIDType = sample.Header.SourceIdType;
                                            input.SourceID = sample.Header.SourceIdValue;

                                        }
                                        else if(ip.Protocol == 17)
                                        {
                                            byte[] udpsegment = new ArraySegment<byte>(ipsegment, ip.IHL, ipsegment.Length - (ip.IHL)).ToArray();

                                            var udp = udpdecoder.DecodeUdpHeader(udpsegment);
                                            input.DstMacAddress = new System.Net.NetworkInformation.PhysicalAddress(ethernet.DstAddress);
                                            input.SrcMacAddress = new System.Net.NetworkInformation.PhysicalAddress(ethernet.SrcAddress);
                                            input.SourcePort = udp.SrcPort;
                                            input.DestinationPort = udp.DstPort;
                                            input.SourceIpAddress = ip.SrcAddr;
                                            input.DestinationIpAddress = ip.DstAddr;
                                            input.Protocol = ip.Protocol;
                                            input.Tos = ip.TOS;
                                            input.TcpFlags = ip.Flags;
                                            input.Octets = ip.TotalLength;
                                            input.Packets = flowsample.SamplingRate;
                                            input.Input = flowsample.Input;
                                            input.Output = flowsample.Output;
                                            input.SamplePool = flowsample.SamplePool;
                                            input.SamplingRate = flowsample.SamplingRate;
                                            input.SampleSequenceNumber = sample.Header.SampleSequenceNumber;
                                            input.SourceIDType = sample.Header.SourceIdType;
                                            input.SourceID = sample.Header.SourceIdValue;

                                        }

                                        InputQueue.Enqueue(input);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                listener.Close();
            }
        }

        private void Workeraggregator_DoWork(object sender, DoWorkEventArgs e)
        {
            bool done = false;

            while (!done)
            {
                if (InputQueue != null && InputQueue.Count != 0)
                {
                    List<SflowRecordInput> inputs = new List<SflowRecordInput>();

                    while (InputQueue.Count > 0)
                    {
                        inputs.Add(InputQueue.Dequeue());
                    }

                    var bysouridtypes = from g in inputs
                                        group g by new
                                        {
                                            g.SourceIDType,
                                            g.SourceID,
                                        };
                    List<SourceIdStatistic> stats = new List<SourceIdStatistic>();

                    foreach (var item in bysouridtypes)
                    {
                        var stat = new SourceIdStatistic();
                        stat.MinSamplePool = item.Min(eg => eg.SamplePool);
                        stat.MaxSamplePool = item.Max(eg => eg.SamplePool);
                        stat.DeltaSamplePool = stat.MaxSamplePool - stat.MinSamplePool;
                        stat.SourceID = item.Key.SourceID;
                        stat.SourceIDType = item.Key.SourceIDType;

                        stat.SamplesReceived = item.Count();
                        stat.EffectiveSamplingRatio = stat.DeltaSamplePool / stat.SamplesReceived;
                        stats.Add(stat);
                    }

                    var exportgroups = from g in inputs
                                       group g by new
                                       {
                                           g.SourceIpAddress,
                                           g.DestinationIpAddress,
                                           g.SourcePort,
                                           g.DestinationPort,
                                           g.Protocol,
                                           g.Input,
                                           g.Output,
                                           g.SourceID,
                                           g.SourceIDType,
                                       } into distincgroups
                                       select new FlowRecordExport()
                                       {

                                       };

                    foreach (var item in exportgroups)
                    {
                        OutputQueue.Enqueue(item);
                    }

                }
            }
        }
    }
}
