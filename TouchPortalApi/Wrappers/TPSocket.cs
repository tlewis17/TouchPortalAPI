using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;

namespace TouchPortalApi.Wrappers {
  public class TPSocket : ITPSocket {
    private Socket _socket;

    public TPSocket() {
    
    }

    public TPSocket(Socket socket) {
      _socket = socket;
    }

    public TPSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) {
      _socket = new Socket(addressFamily, socketType, protocolType);
    }

    public virtual bool Connected => _socket != null && _socket.Connected;

    public virtual void Connect(EndPoint remoteEP) {
      _socket.Connect(remoteEP);
    }

    public virtual void Disconnect(bool reuseSocket) {
      _socket.Disconnect(reuseSocket);
    }

    public virtual void Dispose() {
      _socket.Dispose();
    }

    public virtual ValueTask<int> ReceiveAsync(Memory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default) {
      return _socket.ReceiveAsync(buffer, socketFlags, cancellationToken);
    }

    public virtual int Send(byte[] buffer, int size, SocketFlags socketFlags) {
      return _socket.Send(buffer, size, socketFlags);
    }

    public virtual ValueTask<int> SendAsync(Memory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default) {
      return _socket.SendAsync(buffer, socketFlags, cancellationToken);
    }

  }
}
