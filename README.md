# HermesMQ
Lightweight message broker on .NET Core

### Status
![.NET Core](https://github.com/RomanEmreis/HermesMQ/workflows/.NET%20Core/badge.svg)

## Client usage

First, have to install the HermesMQ.Client NuGet package
```cmd
Install-Package HermesMQ.Client -Version 0.0.1-alpha
```
For [ASP.NET Core](http://asp.net) you just simply add at Startup.cs
```csharp
using HermesMQ.Extensions;
...
public void ConfigureServices(IServiceCollection services)
{
  ...
  serviceCollection.AddHermes<Guid, string>();
}
```
and inject `IConnectionFactory` into desired services.
```csharp
public class MyService
{
  private readonly IConnectionFactory _connectionFactory;
  
  public MyService(IConnectionFactory connectionFactory)
  {
    _connectionFactory = connectionFactory;
  }
  ...
}
```
To create the connection to HermesMQ you just need:
```csharp
var connection = await _connectionFactory.ConnectAsync("127.0.0.1", 8087);
```
To create the producer of specific channel:
```csharp
var producer = connection.GetProducer<Guid, string>("MyChannel");

// The code below sends the message "Hello World!" to "MyChannel" channel
await producer.ProduceAsync(Guid.NewGuid(), "Hello World!");

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
