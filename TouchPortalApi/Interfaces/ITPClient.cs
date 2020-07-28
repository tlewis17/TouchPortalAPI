using System.Threading;
using System.Threading.Tasks;

namespace TouchPortalApi.Interfaces {
  public interface ITPClient {
    void Dispose();
    Task ProcessPipes();
    Task SendAsync(object model, CancellationToken cancellationToken = default);
  }
}