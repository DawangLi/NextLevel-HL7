using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace NextLevelHL7.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            // sample #1: create an inbound HL7 consumer using MLLP frames over TCP/IP sockets
            BaseHL7Interface hl7InboundSocketInterface = new HL7InboundSocketInterface("Inbound Socket Sample", 2575);
            hl7InboundSocketInterface.MessageEvent += OnMessageEvent;
            hl7InboundSocketInterface.StatusEvent += OnStatusEvent;
            hl7InboundSocketInterface.ErrorEvent += OnErrorEvent;
            hl7InboundSocketInterface.SendAcknowledgements = false;
            hl7InboundSocketInterface.StartAsync();

            // sample #2: create an outbound HL7 publisher using MLLP frames over TCP/IP sockets
            HL7OutboundSocketInterface hl7OutboundSocketInterface = new HL7OutboundSocketInterface("Outbound Socket Sample", "127.0.0.1", 2575);
            hl7OutboundSocketInterface.MessageEvent += OnMessageEvent;
            hl7OutboundSocketInterface.StatusEvent += OnStatusEvent;
            hl7OutboundSocketInterface.ErrorEvent += OnErrorEvent;
            hl7OutboundSocketInterface.StartAsync();
            hl7OutboundSocketInterface.EnqueueMessage(GetSampleHL7Message());

            // sample #3: create an inbound HL7 consmer using messages persisted to file system
            BaseHL7Interface hl7InboundFileSystemInterface = new HL7InboundFileSystemInterface("File System Sample", Environment.CurrentDirectory, "hl7");
            hl7InboundFileSystemInterface.MessageEvent += OnMessageEvent;
            hl7InboundFileSystemInterface.StatusEvent += OnStatusEvent;
            hl7InboundFileSystemInterface.ErrorEvent += OnErrorEvent;
            hl7InboundFileSystemInterface.StartAsync();

            // sample #4: show statistics for active interfaces
            Thread.Sleep(4000); // wait a few seconds for async processing to complete before reporting statistics
            InterfaceStatistics statistics = hl7InboundSocketInterface.Statistics;
            Console.WriteLine("[Statistics]: Interface Start Time: {0}", statistics.StartDateTime.ToString());
            Console.WriteLine("[Statistics]: Interface Uptime: {0}s", statistics.UpTime.TotalSeconds);
            Console.WriteLine("[Statistics]: Interface Last Message: {0}", statistics.LastMessageDateTime.ToString());
            foreach (KeyValuePair<string, int> messageSuccess in statistics.Successes)
                Console.WriteLine("[Statistics]: Interface Message {0}: {1}", messageSuccess.Key, messageSuccess.Value);

            Console.ReadKey();

            hl7InboundSocketInterface.Stop();
            hl7InboundFileSystemInterface.Stop();
            hl7OutboundSocketInterface.Stop();
        }

        /// <summary>
        /// Demonstrates how to capture errors issued by an interface.  Errors are shared as an event
        /// to ensure the interface continues to process incoming traffic.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void OnErrorEvent(object sender, Exception e)
        {
            IEHRInterface ehrInterface = sender as IEHRInterface;
            Console.WriteLine("[{0}]: {1}", ehrInterface.Name, e.ToString());
        }


        /// <summary>
        /// Demonstrates how to capture an event which describes a change in interface status (i.e. Started, Stopped).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="statusEvent">The e.</param>
        private static void OnStatusEvent(object sender, InterfaceStatusEvent statusEvent)
        {
            IEHRInterface ehrInterface = sender as IEHRInterface;
            Console.WriteLine("[{0}]: {1}", ehrInterface.Name, statusEvent.Text);
        }

        /// <summary>
        /// Demonstrates how to parse incoming HL7 messages.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        private static void OnMessageEvent(object sender, Message message)
        {
            IEHRInterface ehrInterface = sender as IEHRInterface;

            SampleMessageProcessor sampleMessageProcessor = new SampleMessageProcessor();
            string messageType = message.MessageType();

            Console.WriteLine("[{0}]: {1} Event ", ehrInterface.Name, messageType);

            switch (messageType)
            {
                case "ADT^A01":
                case "ADT^A02":
                case "ADT^A03":
                case "ADT^A04":
                    // demonstrates how to read an HL7 ADT ("Admit Dischrage Transfer") message.  
                    sampleMessageProcessor.HandleADT(message);
                    break;
                case "ORU^R01":
                    // demonstrates how to read an HL7 ORU ("Observation Result") message
                    sampleMessageProcessor.HandleORU(message);
                    break;
                case "ADT^A34":
                    // demonstrates how to read an HL7 ADT Patient Merge message
                    sampleMessageProcessor.HandleMerge(message);
                    break;
                default:
                    break;
            }
        }

        private static string GetSampleHL7Message()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "NextLevelHL7.Samples.sample.hl7";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader streamReader = new StreamReader(stream))
                return streamReader.ReadToEnd();
        }
    }
}
