using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using TouchPortalApi.Configuration;
using TouchPortalApi.Interfaces;
using Xunit;

namespace TouchPortalApi.Tests {
  public class TouchPointApiExtensionsTests {
    [Fact]
    public void ConfigureServices_ShouldHaveOptions() {
      // arrange
      var services = new ServiceCollection();
      services.ConfigureTouchPointApi((options) => {
        options.PluginId = "TestPlugin";
        options.ServerIp = "127.0.0.1";
        options.ServerPort = 1234;
      });

      var provider = services.BuildServiceProvider();

      // act
      var options = provider.GetRequiredService<IOptionsSnapshot<TouchPortalApiOptions>>();

      // assert
      Assert.NotNull(options);
      Assert.Equal("TestPlugin", options.Value.PluginId);
    }

    [Fact]
    public void ConfigureServices_ShouldHaveSocketService() {
      // arrange
      var services = new ServiceCollection();
      services.ConfigureTouchPointApi((options) => {
        options.PluginId = "TestPlugin";
        options.ServerIp = "127.0.0.1";
        options.ServerPort = 1234;
      });

      var provider = services.BuildServiceProvider();

      // act
      var tpsocket = provider.GetRequiredService<ITPSocket>();

      // assert
      Assert.NotNull(tpsocket);
    }

    [Fact]
    public void ConfigureServices_ShouldHaveClientService() {
      // arrange
      var services = new ServiceCollection();
      services.ConfigureTouchPointApi((options) => {
        options.PluginId = "TestPlugin";
        options.ServerIp = "127.0.0.1";
        options.ServerPort = 70000;
      });

      var provider = services.BuildServiceProvider();

      // act/assert - Service exists but port invalid. Can't mock a socket in this fashion.
      Assert.Throws<ArgumentOutOfRangeException>(() => provider.GetRequiredService<ITPClient>());
    }
  }
}
