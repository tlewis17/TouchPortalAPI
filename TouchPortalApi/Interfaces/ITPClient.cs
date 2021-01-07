using System;
using System.Threading;
using System.Threading.Tasks;

namespace TouchPortalApi.Interfaces {
  public interface ITPClient : IDisposable {
    Task ProcessPipes();
    Task SendAsync(object model, CancellationToken cancellationToken = default);
  }
}