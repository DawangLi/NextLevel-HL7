## Welcome!

The NextLevel HL7 API enables .NET interface developers to send and receive HL7 messages via a simple client server architecture.

## What's in the API:

*   Host an HL7 server and receive HL7 messages through an event-based architecture
*   Connect to an HL7 server and transmit HL7 messages
*   Inspect HL7 messages without complex parsing or conditional logic 
*   Receive HL7 messages via socket or file system
*   Track statistics for open connections, message history, and delivery failures

## Why use this API?
*   Next Level HL7 is built for thread-safety.  Multiple clients and servers may exist within a single AppDomain
*   Provides a Task-based Asynchronous Programming model for connection management
*   Leverages .NET dynamic and implicit conversion for a 'developer humane' interface

## Contribute Back!

Is there a feature missing that you'd like to see, or found a bug that you have a fix for? Or do you have an idea or just interest in helping out in building the library? Let us know and we'd love to work with you. 


## Getting Started
1: Create an inbound HL7 consumer using MLLP frames over TCP/IP sockets
```c#
BaseHL7Interface hl7InboundSocketInterface = new HL7InboundSocketInterface("Inbound Socket Sample", 2575);
hl7InboundSocketInterface.MessageEvent += OnMessageEvent;
hl7InboundSocketInterface.StatusEvent += OnStatusEvent;
hl7InboundSocketInterface.ErrorEvent += OnErrorEvent;
hl7InboundSocketInterface.SendAcknowledgements = false;
hl7InboundSocketInterface.StartAsync();
```


2: Attach an event handler to receive messages
```c#
private void OnMessageEvent(object sender, Message message)
{
   IEHRInterface ehrInterface = sender as IEHRInterface;
   string messageType = message.MessageType();
   Console.WriteLine("[{0}]: {1} Event ", ehrInterface.Name, messageType);
}
```

3: Attach optional event handlers to receive interface errors or status events
```c#
private void OnErrorEvent(object sender, Exception exception)
{
   IEHRInterface ehrInterface = sender as IEHRInterface;
   Console.WriteLine(e.ToString());
}
```

4: Parse HL7 messages
```c#
// find patient's first name in PID segment, sequence 5, field 0.  
Segment pid = message.FindSegment("PID");
string firstName = pid[5][0];
```

5: Show interface statistics
```c#
InterfaceStatistics statistics = hl7InboundSocketInterface.Statistics;
Console.WriteLine("Interface Start Time: {0}", statistics.StartDateTime.ToString());
Console.WriteLine("Interface Uptime: {0}s", statistics.UpTime.TotalSeconds);
Console.WriteLine("Interface Last Message: {0}", statistics.LastMessageDateTime.ToString());
foreach (KeyValuePair<string, int> messageSuccess in statistics.Successes)
   Console.WriteLine("Interface Message {0}: {1}", messageSuccess.Key, messageSuccess.Value);
```

6: Scan a file system for HL7 messages
```c#
BaseHL7Interface hl7InboundFileSystemInterface = new HL7InboundFileSystemInterface("File System Sample", @"C:\", "hl7");
hl7InboundFileSystemInterface.MessageEvent += OnMessageEvent;
hl7InboundFileSystemInterface.StartAsync();
```

7: Connect to an external HL7 server and deliver messages
```c#
HL7OutboundSocketInterface hl7OutboundSocketInterface = new HL7OutboundSocketInterface("Outbound Socket Sample", "127.0.0.1", 2575);
hl7OutboundSocketInterface.StartAsync();
hl7OutboundSocketInterface.EnqueueMessage("MSH|...");
```

##

This project is maintained by Shaun Tonstad.  Contact [tonstad@clarion.consulting](mailto:tonstad@clarion.consulting) with questions or comments.
