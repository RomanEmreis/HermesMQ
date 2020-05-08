# HermesMQ
Lightweight message broker on .NET Core

### Status
![Hermes.Abstrations (.NET Core)](https://github.com/RomanEmreis/HermesMQ/workflows/Hermes.Abstrations%20(.NET%20Core)/badge.svg?branch=master)
![Hermes.Infrastructure (.NET Core)](https://github.com/RomanEmreis/HermesMQ/workflows/Hermes.Infrastructure%20(.NET%20Core)/badge.svg?branch=master)
![Hermes.MessageQueue.Service (.NET Core)](https://github.com/RomanEmreis/HermesMQ/workflows/Hermes.MessageQueue.Service%20(.NET%20Core)/badge.svg?branch=master)

## Client usage

First, have to install the HermesMQ.Client NuGet package
```cmd
Install-Package HermesMQ.Client -Version 0.0.1-alpha
```
For [ASP.NET Core](http://asp.net) you just simply add at Startup.cs
```csharp
using HermesMQ.Extensions;
...
public IConfiguration Configuration { get; }

public void ConfigureServices(IServiceCollection services)
{
  ...
  serviceCollection.AddHermes(Configuration);
}
```
and inject `IConnectionFactory` into desired services.
```csharp
using HermesMQ;
using Hermes.Abstractions;

public class MyService
{
  private readonly IConnectionFactory _connectionFactory;
  private readonly IMessageAdapter _messageAdapter;
  private readonly IOptions<HermesSettings> _hermesOptions;
  
  public MyService(IConnectionFactory connectionFactory, IMessageAdapter messageAdapter, IOptions<HermesSettings> hermesOptions)
  {
    _connectionFactory = connectionFactory;
    _messageAdapter = messageAdapter;
    _hermesOptions = hermesOptions;
  }
  ...
}
```
To create the connection to HermesMQ you just need:
```csharp
//using IOptions
using HermesMQ.Extensions;
...
var connection = await _connectionFactory.ConnectAsync(_hermesOptions.Value);

//using host and port directly
var connection = await _connectionFactory.ConnectAsync("127.0.0.1", 8087);
```
To create the producer of specific channel:
```csharp
//with injected IMessageAdapter
var producer = connection.GetProducer<Guid, string>("MyChannel", _messageAdapter);

//without injected IMessageAdapter, every time creates the default JsonMessageAdapter
var producer = connection.GetProducer<Guid, string>("MyChannel");

// The code below sends the message "Hello World!" to "MyChannel" channel
var message = await producer.ProduceAsync(Guid.NewGuid(), "Hello World!");

//or you can create your own message
var message = new Message<Guid, string>(Guid.NewGuid(), "Hello World!");
await producer.ProduceAsync(message);

//MessageSent event
producer.MessageSent += message =>
{
  //handle when the message was sent
};
```
To create the simple consumer you need:
```csharp
var consumer = connection.GetConsumer<Guid, string>("MyChannel");

//Starting the consuming "MyChannel" channel
await consumer.ConsumeAsync();

consumer.MessageReceived += (channel, message) =>
{
  //handle when the message received
};
```

## Setting up the HermesMQ server
