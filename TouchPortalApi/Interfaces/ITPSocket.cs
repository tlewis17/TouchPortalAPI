using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TouchPortalApi.Interfaces {
  public interface ITPSocket {
    bool Connected { get; }
    void Connect(EndPoint remoteEP);
    void Dispose();
    void Disconnect(bool reuseSocket);
    ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    ValueTask<int> SendAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
  }
}
