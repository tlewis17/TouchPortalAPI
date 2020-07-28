using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace TouchPortalApi.Interfaces {
  public interface IMessageProcessor {
    Task Listen();
    Task TryPairAsync();
  }
}