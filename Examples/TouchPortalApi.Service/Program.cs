using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TouchPortalApi.Service {
  public static class Program {
    public static void Main(string[] args) {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) => {
              services.AddHostedService<Worker>();

              // Configure options
              services.ConfigureTouchPointApi((opts) => {
                opts.ServerIp = "127.0.0.1";
                opts.ServerPort = 12136;
                opts.PluginId = "TouchPortal.SnoopPlugin";
              });
            });
  }
}
