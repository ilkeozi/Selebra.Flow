using GalaSoft.MvvmLight;
using Selebra.Flow.Export;
using Selebra.Flow.Sflow;
using Selebra.Flow.TCPIP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Flow_Test_Dashboard.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public double MessagesSec { get; set; }
        public double SamplesSec { get; set; }

        public ObservableCollection<FlowRecordExport> Exports { get; set; }

        public ObservableCollection<uint> SamplePools { get; set; }

        public ObservableCollection<DistinctFlowModel> DistinctGroups { get; set; }
        public ObservableCollection<DistinctFlowModel> DistinctSamplePools { get; set; }
        public ObservableCollection<DistinctFlowModel> BySourceIdType { get; set; }

        public ObservableCollection<SourceIdStatistic> SourceIdStatistics { get; set; }


        public ObservableCollection<FlowRecordExport> PreviousExports { get; set; }



        public ObservableCollection<string> Agents { get; set; }

        UdpClient listener;
        IPEndPoint groupEP;

        private readonly BackgroundWorker workerreceiver = new BackgroundWorker();
        private readonly BackgroundWorker workeraggregator = new BackgroundWorker();

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                //DistinctGroups = new ObservableCollection<DistinctFlowModel>();

                //var flow = new DistinctFlowModel();
                //flow.SourcePort = 67;
                //flow.DestinationPort = 22;
                //DistinctGroups.Add(flow);
                //RaisePropertyChanged("DistinctGroups");



            }
            else
            {
                SourceIdStatistics = new ObservableCollection<SourceIdStatistic>();
                DistinctSamplePools = new ObservableCollection<DistinctFlowModel>();
                DistinctGroups = new ObservableCollection<DistinctFlowModel>();
                Agents = new ObservableCollection<string>();
                PreviousExports = new ObservableCollection<FlowRecordExport>();
                BySourceIdType = new ObservableCollection<DistinctFlowModel>();

                listener = new UdpClient(6343);
                groupEP = new IPEndPoint(IPAddress.Any, 6343);



                workerreceiver.DoWork += Workerreceiver_DoWork;
                workerreceiver.RunWorkerAsync();

                workeraggregator.DoWork += Workeraggregator_DoWork;
                workeraggregator.RunWorkerAsync();
            }


        }

        private void Workeraggregator_DoWork(object sender, DoWorkEventArgs e)
        {
            bool done = false;

            while (!done)
            {
                //So let's imagine that a particular data-source has been configured to
                //sample at 1 -in-512, and you received 100 flow - samples in 1 minute,
                //and between the first sample and the last sample the samplePool
                //increased from, say, 120,332 to 172,332(an increase of 52, 000), and
                //of those 100 samples, 50 were from the IP source address 10.2.3.4:

                //effective_sampling_ratio = 52000 / 100 = 520
                //frames(10.2.3.4) = 50 * 520 = 26,000

                if (Exports != null && Exports.Count != 0)
                {

                    DistinctGroups = new ObservableCollection<DistinctFlowModel>();

                    var bysouridtypes = from g in Exports
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
                    SourceIdStatistics = new ObservableCollection<SourceIdStatistic>(stats);
                    RaisePropertyChanged("SourceIdStatistics");

                    var distinctgroups = from g in Exports
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
                                         } into distincg
                                         select new DistinctFlowModel()
                                         {
                                             SourceIpAddress = distincg.Key.SourceIpAddress,
                                             DestinationIpAddress = distincg.Key.DestinationIpAddress,
                                             SourcePort = distincg.Key.SourcePort,
                                             DestinationPort = distincg.Key.DestinationPort,
                                             Protocol = distincg.Key.Protocol,
                                             Input = distincg.Key.Input,
                                             Output = distincg.Key.Output,
                                             EffectiveSamplingRatio = SourceIdStatistics.Where(ef => ef.SourceIDType == distincg.Key.SourceIDType && ef.SourceID == distincg.Key.SourceID).FirstOrDefault().EffectiveSamplingRatio,
                                             DeltaSamplePool = SourceIdStatistics.Where(ef => ef.SourceIDType == distincg.Key.SourceIDType && ef.SourceID == distincg.Key.SourceID).FirstOrDefault().DeltaSamplePool,
                                             MinSamplePool = SourceIdStatistics.Where(ef => ef.SourceIDType == distincg.Key.SourceIDType && ef.SourceID == distincg.Key.SourceID).FirstOrDefault().MinSamplePool,
                                             MaxSamplePool = SourceIdStatistics.Where(ef => ef.SourceIDType == distincg.Key.SourceIDType && ef.SourceID == distincg.Key.SourceID).FirstOrDefault().MaxSamplePool,
                                             Total = (Convert.ToInt64(distincg.Average(ef => ef.Octets) * SourceIdStatistics.Where(ef => ef.SourceIDType == distincg.Key.SourceIDType && ef.SourceID == distincg.Key.SourceID).FirstOrDefault().EffectiveSamplingRatio)) / 30,
                                             Packets = (Convert.ToInt64(distincg.Count() * SourceIdStatistics.Where(ef => ef.SourceIDType == distincg.Key.SourceIDType && ef.SourceID == distincg.Key.SourceID).FirstOrDefault().EffectiveSamplingRatio)) / 30,
                                             Count = distincg.Count()
                                         };

             
                
                    DistinctGroups = new ObservableCollection<DistinctFlowModel>(distinctgroups);
                    RaisePropertyChanged("DistinctGroups");

                    Exports = new ObservableCollection<FlowRecordExport>();
                }
                System.Threading.Thread.Sleep(30000);
            }
        }

        private void Workerreceiver_DoWork(object sender, DoWorkEventArgs e)
        {
            SamplePools = new ObservableCollection<uint>();
            Exports = new ObservableCollection<FlowRecordExport>();
            //6343

            bool done = false;
            SflowDecoder decoder = new SflowDecoder();

            DateTime start = DateTime.Now;
            int flowsamples = 0;
            int messages = 0;
            try
            {
                while (!done)
                {

                    byte[] bytes = listener.Receive(ref groupEP);
                    var message = decoder.DecodeMessage(bytes);


                    if (Agents.Where(es => es == message.AgentIP.ToString()).FirstOrDefault() == null)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Agents.Add(message.AgentIP.ToString());
                            RaisePropertyChanged("Agents");
                        }));

                    }

                    //Console.WriteLine("Received Sflow Version: {0} AgentIP: {1} SequenceNumber: {2} Uptime: {3} SamplesCount; {4} ", message.Version, message.AgentIP.ToString(), message.SequenceNumber, message.Uptime, message.SamplesCount);
                    messages++;
                    var differencesec = DateTime.Now.Subtract(start).TotalSeconds;

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

                                    EthernetHeaderDecoder ethernetdecoder = new EthernetHeaderDecoder();
                                    var ethernet = ethernetdecoder.DecodeEthernetHeader(flowdataheader.HeaderSampled);
                                    //Console.WriteLine("Received Ethernet Frame: SrcAdress={0} , DstAddress={1} , Type={2} ", ethernet.SrcAddress, ethernet.DstAddress, ethernet.TypeLength);

                                    if (ethernet.TypeLength == 0x0800)
                                    {
                                        byte[] ipsegment = new ArraySegment<byte>(flowdataheader.HeaderSampled, 14, flowdataheader.HeaderSampled.Length - 14).ToArray();

                                        Ipv4HeaderDecoder ipdecoder = new Ipv4HeaderDecoder();
                                        var ip = ipdecoder.DecodeIpv4Header(ipsegment);
                                        //Console.WriteLine("Received Ip Datagram:  SrcAdress={0} , DstAddress={1} , Protocol={2}, Length={3}", ip.SrcAddr, ip.DstAddr, ip.Protocol, ip.TotalLength);

                                        byte[] tcpsegment = new ArraySegment<byte>(ipsegment, ip.IHL, ipsegment.Length - (ip.IHL)).ToArray();

                                        if (ip.Protocol == 6)
                                        {
                                            TcpHeaderDecoder tcpdecoder = new TcpHeaderDecoder();
                                            var tcp = tcpdecoder.DecodeTcpHeader(tcpsegment);
                                            //Console.WriteLine("Received Tcp Packet:  SrcPort={0} , DstPort={1}", tcp.SrcPort, tcp.DstPort);
                                            FlowRecordExport export = new FlowRecordExport();
                                            export.DstMacAddress = new System.Net.NetworkInformation.PhysicalAddress(ethernet.DstAddress);
                                            export.SrcMacAddress = new System.Net.NetworkInformation.PhysicalAddress(ethernet.SrcAddress);
                                            export.SourcePort = tcp.SrcPort;
                                            export.DestinationPort = tcp.DstPort;
                                            export.SourceIpAddress = ip.SrcAddr;
                                            export.DestinationIpAddress = ip.DstAddr;
                                            export.Protocol = ip.Protocol;
                                            export.Tos = ip.TOS;
                                            export.TcpFlags = ip.Flags;
                                            export.Octets = ip.TotalLength;
                                            export.Packets = flowsample.SamplingRate;
                                            export.Input = flowsample.Input;
                                            export.Output = flowsample.Output;
                                            export.SamplePool = flowsample.SamplePool;
                                            export.SamplingRate = flowsample.SamplingRate;
                                            export.SampleSequenceNumber = sample.Header.SampleSequenceNumber;
                                            export.SourceIDType = sample.Header.SourceIdType;
                                            export.SourceID = sample.Header.SourceIdValue;
                                            if (message.AgentIP.ToString() == "10.1.90.2")
                                            {
                                                Exports.Add(export);

                                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    PreviousExports.Add(export);
                                                    RaisePropertyChanged("PreviousExports");
                                                }));
                                            }

                                        }
                                    }

                                }
                            }
                            flowsamples++;
                        }
                    }

                    //var distinctgroups =
                    //                            from g in Exports
                    //                            group g by new
                    //                            {
                    //                                g.SourceIpAddress,
                    //                                g.DestinationIpAddress,
                    //                                g.SourcePort,
                    //                                g.DestinationPort,
                    //                                g.Protocol,
                    //                                g.Input,
                    //                                g.Output,
                    //                            } into distincg
                    //                            select new
                    //                            {
                    //                                distincg.Key.SourceIpAddress,
                    //                                distincg.Key.DestinationIpAddress,
                    //                                distincg.Key.SourcePort,
                    //                                distincg.Key.DestinationPort,
                    //                                distincg.Key.Protocol,
                    //                                distincg.Key.Input,
                    //                                distincg.Key.Output,


                    //                            };

                    //Console.WriteLine("Distinct Groups: " + distinctgroups.Count());
                    //if (Exports.Count != 0)
                    //{
                    //    double percent = (double)distinctgroups.Count() / Exports.Count();
                    //    Console.WriteLine("Distinct Percent: " + percent);
                    //}
                    if (Convert.ToInt32(differencesec) != 0)
                    {
                        MessagesSec = messages / Convert.ToInt32(differencesec);
                        SamplesSec = flowsamples / Convert.ToInt32(differencesec);

                        RaisePropertyChanged("MessagesSec");
                        RaisePropertyChanged("SamplesSec");

                    }


                }
            }
            catch (Exception ex)
            {
                using (System.IO.TextWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + @"\Exceptions.txt", true))
                {
                    file.WriteLine(ex);
                }
            }
            finally
            {
                listener.Close();
            }
        }
    }
}