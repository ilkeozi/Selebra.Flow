using Selebra.Flow.Sflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Selebra.Flow.TCPIP;
using Selebra.Flow.Export;
using System.Collections.Concurrent;

namespace TestConsole
{

    class IPAddressConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }

    class IPEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IPEndPoint ep = (IPEndPoint)value;
            JObject jo = new JObject();
            jo.Add("Address", JToken.FromObject(ep.Address, serializer));
            jo.Add("Port", ep.Port);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
            int port = (int)jo["Port"];
            return new IPEndPoint(address, port);
        }
    }
    static class Program
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        static List<uint> SamplePools;

        static List<FlowRecordExport> Exports;
        static void Main(string[] args)
        {
            SamplePools = new List<uint>();
            Exports = new List<FlowRecordExport>();
            //6343
            UdpClient listener = new UdpClient(6343);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 6343);
            bool done = false;
            SflowDecoder decoder = new SflowDecoder();
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPAddressConverter());
            settings.Converters.Add(new IPEndPointConverter());
            settings.Formatting = Formatting.Indented;
            DateTime start = DateTime.Now;
            int flowsamples = 0;
            int messages = 0;
            try
            {
                while (!done)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    var message = decoder.DecodeMessage(bytes);
                    //Console.WriteLine("Received Sflow Version: {0} AgentIP: {1} SequenceNumber: {2} Uptime: {3} SamplesCount; {4} ", message.Version, message.AgentIP.ToString(), message.SequenceNumber, message.Uptime, message.SamplesCount);
                    messages++;
                    Console.Clear();
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

                                            Exports.Add(export);


                                            //var distinctexports = Exports.DistinctBy(e => new { e.SourceIpAddress, e.SourcePort, e.DestinationIpAddress, e.DestinationPort, e.Input, e.Output, e.Protocol });
                                            //Console.WriteLine("Received Flow:  SrcPort={0} , DstPort={1} , SrcIp={2}, DstIp={3}, Protocol={4}, Input={5}, Output={6}, SamplePool={7}, Sqn={8}", tcp.SrcPort, tcp.DstPort, ip.SrcAddr, ip.DstAddr, ip.Protocol, flowsample.Input, flowsample.Output, flowsample.SamplePool, flowsample.Header.SampleSequenceNumber);
                                            //using (System.IO.TextWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + @"\export.txt", true))
                                            //{
                                            //    //file.WriteLine("Received Flow:  SrcPort={0} , DstPort={1} , SrcIp={2}, DstIp={3}, Protocol={4}, Input={5}, Output={6}, SamplePool={7}, Sqn={8}", tcp.SrcPort, tcp.DstPort, ip.SrcAddr, ip.DstAddr, ip.Protocol, flowsample.Input, flowsample.Output, flowsample.SamplePool, flowsample.Header.SampleSequenceNumber);
                                            //    file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", message.AgentIP,message.SamplesCount,message.SequenceNumber,message.Uptime,export.DstMacAddress,export.SrcMacAddress, tcp.SrcPort, tcp.DstPort, ip.SrcAddr, ip.DstAddr, ip.Protocol, flowsample.Input, flowsample.Output, flowsample.SamplePool, flowsample.Header.SampleSequenceNumber,export.Octets);

                                            //}
                                            //var sameexports = Exports.Where(e => e.SourceIpAddress == export.SourceIpAddress && e.SourcePort == export.SourcePort && e.DestinationIpAddress == export.DestinationIpAddress && e.DestinationPort == export.DestinationPort && e.Input == export.Input && e.Output == export.Output && e.Protocol == export.Protocol);

                                            //foreach (var ordered in sameexports)
                                            //{


                                            //}



                                            //var jsonexport = JsonConvert.SerializeObject(export, settings);
                                            //using (System.IO.TextWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + @"\flowexportjson.txt", true))
                                            //{
                                            //    file.WriteLine(jsonexport);
                                            //}

                                            //var jsonmessage = JsonConvert.SerializeObject(message, settings);
                                            //using (System.IO.TextWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + @"\sflowjson.txt", true))
                                            //{
                                            //    file.WriteLine(jsonmessage);
                                            //}

                                        }
                                    }

                                }
                            }
                            flowsamples++;
                        }
                    }

                    var distinctgroups =
                                                from g in Exports
                                                group g by new
                                                {
                                                    g.SourceIpAddress,
                                                    g.DestinationIpAddress,
                                                    g.SourcePort,
                                                    g.DestinationPort,
                                                    g.Protocol,
                                                    g.Input,
                                                    g.Output,
                                                } into distincg
                                                select new
                                                {
                                                    distincg.Key.SourceIpAddress,
                                                    distincg.Key.DestinationIpAddress,
                                                    distincg.Key.SourcePort,
                                                    distincg.Key.DestinationPort,
                                                    distincg.Key.Protocol,
                                                    distincg.Key.Input,
                                                    distincg.Key.Output,


                                                };
                    Console.WriteLine("Exports: " + Exports.Count());
                    Console.WriteLine("Distinct Groups: " + distinctgroups.Count());
                    if (Exports.Count != 0)
                    {
                        double percent = (double)distinctgroups.Count() / Exports.Count();
                        Console.WriteLine("Distinct Percent: " + percent);
                    }
                    if (Convert.ToInt32(differencesec) != 0)
                    {
                        Console.WriteLine("Messages/sec: " + messages / Convert.ToInt32(differencesec));
                        Console.WriteLine("Samples/sec: " + flowsamples / Convert.ToInt32(differencesec));
                    }

                    //var json = JsonConvert.SerializeObject(message, settings);
                    //using (System.IO.TextWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + @"\sflowjson.txt",true))
                    //{
                    //  file.WriteLine(json);
                    //}

                }
            }
            catch (Exception e)
            {
                using (System.IO.TextWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + @"\Exceptions.txt", true))
                {
                    file.WriteLine(e);
                }
            }
            finally
            {
                listener.Close();
            }

        }


        private static void PrintSamplePools(FlowSample flowsample)
        {
            if (!SamplePools.Any(e => e.Equals(flowsample.SamplePool)))
            {
                SamplePools.Add(flowsample.SamplePool);
                Console.WriteLine(flowsample.SamplePool);
            }
        }


    }
}
