using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TouchPortalApi.Interfaces {
  public interface ITPSocket : IDisposable {
    bool Connected { get; }
    void Connect(EndPoint remoteEP);
    void Disconnect(bool reuseSocket);
    ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    ValueTask<int> SendAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
  }
}
