using Microsoft.Extensions.DependencyInjection;
using System;
using TouchPortalApi.Configuration;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Services;
using TouchPortalApi.Wrappers;

namespace TouchPortalApi {
  public static class TouchPointApiExtensions {
    public static IServiceCollection ConfigureTouchPointApi(this IServiceCollection services, Action<TouchPortalApiOptions> options) {
      services.Configure(options);

      services.AddSingleton<ITPSocket, TPSocket>();
      services.AddSingleton<ITPClient, TPClient>();
      services.AddSingleton<IProcessQueueingService, ProcessQueueingService>();
      services.AddSingleton<IMessageProcessor, MessageProcessor>();

      return services;
    }
  }
}
