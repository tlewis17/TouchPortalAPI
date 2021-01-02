# TouchPortalApi

![.NET Core](https://github.com/tlewis17/TouchPortalAPI/workflows/.NET%20Core/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/v/TouchPortalApi)](https://www.nuget.org/packages/TouchPortalApi/)
[![Downloads](https://img.shields.io/nuget/dt/TouchPortalApi)](https://www.nuget.org/packages/TouchPortalApi/)
[![Stars](https://img.shields.io/github/stars/tlewis17/TouchPortalAPI)](https://github.com/tlewis17/TouchPortalAPI/stargazers)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)

SDK to interface with Touch Portal API through sockets.
Sample projects include WebApp, Console, and Worker Services.

## Setup

Configure your options/DI:
``` csharp
services.Configure<TouchPortalApiOptions>((options) => {
  options.ServerIp = "127.0.0.1";
  options.ServerPort = 12136;
  options.PluginId = "TouchPortal.SnoopPlugin";
});
```

Startup the listener and pair to Touch Portal:
```csharp
Task.WhenAll(new Task[] {
    messageProcessor.Listen(),
    messageProcessor.TryPairAsync()
});
```

## Usage

Use or add to your class constructor for DI for the services you want to use:
``` csharp
// Our services, can be retrieved through DI in constructors
var messageProcessor = app.ApplicationServices.GetRequiredService<IMessageProcessor>();
```

Setup your event handlers that you want to use:
``` csharp
// On Plugin Connect Event
messageProcessor.OnConnectEventHandler += () => {
  Console.WriteLine($"{DateTime.Now} Plugin Connected to TouchPortal");
};

// On Action Event
messageProcessor.OnActionEvent += (actionId, dataList) => {
  Console.WriteLine($"{DateTime.Now} Action Event Fired.");
  foreach (var o in dataList) {
    Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
  }
};

// On List Change Event
messageProcessor.OnListChangeEventHandler += (actionId, value) => {
  Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
};

// On Plugin Disconnect
messageProcessor.OnCloseEventHandler += () => {
  Console.Write($"{DateTime.Now} Plugin Quit Command");
};
```

Update a state:
``` csharp
messageProcessor.UpdateState(new StateUpdate() { Id = obj[0].Id, Value = "Off" });
```
