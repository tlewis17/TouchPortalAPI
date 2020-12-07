using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;

namespace TouchPortalApi.Wrappers {
  /// <summary>
  /// Socket Wrapper
  /// </summary>
  public class TPSocket : ITPSocket {
    private readonly Socket _socket;

    // Default Constructor for dependency injection
    public TPSocket() {}

    /// <summary>
    /// Constructor if existing socket connection is uesd
    /// </summary>
    /// <param name="socket">Existing socket</param>
    public TPSocket(Socket socket) {
      _socket = socket;
    }

    /// <summary>
    /// Constructor for creating a new socket connection
    /// </summary>
    /// <param name="addressFamily">The IP Address Binding for the socket</param>
    public TPSocket(AddressFamily addressFamily) {
      _socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary>
    /// Returns true if connected
    /// </summary>
    public virtual bool Connected => _socket != null && _socket.Connected;

    /// <summary>
    /// Connect to the socket IP Address
    /// </summary>
    /// <param name="remoteEP"></param>
    public virtual void Connect(EndPoint remoteEP) {
      _socket.Connect(remoteEP);
    }

    /// <summary>
    /// Disconnect from the socket
    /// </summary>
    /// <param name="reuseSocket">Allow reuse if true</param>
    public virtual void Disconnect(bool reuseSocket) {
      _socket.Disconnect(reuseSocket);
    }

    /// <summary>
    /// Dispose the socket
    /// </summary>
    public virtual void Dispose() {
      _socket.Dispose();
    }

    /// <summary>
    /// Receive a message from the socket
    /// </summary>
    /// <param name="buffer">The buffere reference</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public virtual ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) {
      return _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
    }

    /// <summary>
    /// Send a message through the socket
    /// </summary>
    /// <param name="buffer">The reference buffer</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public virtual ValueTask<int> SendAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) {
      return _socket.SendAsync(buffer, SocketFlags.None, cancellationToken);
    }

  }
}
