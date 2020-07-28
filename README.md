# TouchPortalApi

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
var stateService = app.ApplicationServices.GetRequiredService<IStateService>();
var actionService = app.ApplicationServices.GetRequiredService<IActionService>();
var choiceService = app.ApplicationServices.GetRequiredService<IChoiceService>();
```

Register events to call back when specific event IDs fire:
``` csharp
// Register event callbacks with ID of the button or choice id from your plugin, returned data is a list of action IDs and values from your plugin
actionService.RegisterActionEvent("TouchPortal.SnoopPlugin.DCS.Action.UFC.Keypad", async (obj) => {
  Console.WriteLine($"{DateTime.Now} DCS Action Event Fired.");
  foreach (var o in obj) {
    Console.WriteLine($"Id: {o.Id} Value: {o.Value}");
  }
});

// Register Choice Events - Returned data is the new value
choiceService.RegisterChoiceEvent("choice test", (obj) => {
  Console.WriteLine($"{DateTime.Now} Choice Event Fired.");
});
```

Update a state:
``` csharp
stateService.UpdateState(new StateUpdate() { Id = obj[0].Id, Value = "Off" });
```
