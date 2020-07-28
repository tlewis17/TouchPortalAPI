using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouchPortalApi.Configuration;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Wrappers;

[assembly: InternalsVisibleTo("TouchPortalApi.Tests")]

namespace TouchPortalApi {
  public class TPClient : ITPClient {
    private readonly IOptionsMonitor<TouchPortalApiOptions> _options;
    private ITPSocket _tpsocket;
    private readonly IProcessQueueingService _processQueueingService;
    private readonly CancellationToken _cancellationToken;
    
    public TPClient(IOptionsMonitor<TouchPortalApiOptions> options, ITPSocket socket, IProcessQueueingService processQueueingService,
      CancellationToken cancellationToken = new CancellationToken()) {
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _tpsocket = socket ?? throw new ArgumentNullException(nameof(socket));
      _processQueueingService = processQueueingService ?? throw new ArgumentNullException(nameof(processQueueingService));
      _cancellationToken = cancellationToken;

      // Add this somewhere in your project, most likely in Startup.cs
      JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };

      InitializeConnection();
    }

    private void InitializeConnection() {
      if (_tpsocket == null || !_tpsocket.Connected) {
        var ipe = new IPEndPoint(IPAddress.Parse(_options.CurrentValue.ServerIp), _options.CurrentValue.ServerPort);
        _tpsocket = new TPSocket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _tpsocket.Connect(ipe);
      }
    }

    public async Task SendAsync(object model, CancellationToken cancellationToken = default) {
      string request = PrepareMessage(model);
      var bytesSent = Encoding.ASCII.GetBytes(request);

      await _tpsocket.SendAsync(bytesSent, SocketFlags.None, cancellationToken);
    }

    internal string PrepareMessage(object model) {
      return $"{JsonConvert.SerializeObject(model)}\n";
    }

    public async Task ProcessPipes() {
      var pipe = new Pipe();
      Task writing = FillPipeAsync(pipe.Writer);
      Task reading = ReadPipeAsync(pipe.Reader);

      await Task.WhenAll(reading, writing);
    }

    public async Task FillPipeAsync(PipeWriter writer) {
      const int minimumBufferSize = 512;

      while (true) {
        // Allocate at least 512 bytes from the PipeWriter.
        Memory<byte> memory = writer.GetMemory(minimumBufferSize);
        try {
          int bytesRead = await _tpsocket.ReceiveAsync(memory, SocketFlags.None);
          if (bytesRead == 0) {
            break;
          }
          // Tell the PipeWriter how much was read from the Socket.
          writer.Advance(bytesRead);
        } catch (Exception ex) {
          //LogError(ex);
          break;
        }

        // Make the data available to the PipeReader.
        FlushResult result = await writer.FlushAsync();

        if (result.IsCompleted) {
          break;
        }
      }

      // By completing PipeWriter, tell the PipeReader that there's no more data coming.
      await writer.CompleteAsync();
    }

    public async Task ReadPipeAsync(PipeReader reader) {
      while (true) {
        ReadResult result = await reader.ReadAsync();
        ReadOnlySequence<byte> buffer = result.Buffer;

        while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line)) {
          // Process the line.
          // Put in queue
          //_messageProcessor.ProcessLine(line);
          //_processQueueingService.AddToProcessQueue(line);
          _processQueueingService.Enqueue(line);
        }

        // Tell the PipeReader how much of the buffer has been consumed.
        reader.AdvanceTo(buffer.Start, buffer.End);

        // Stop reading if there's no more data coming.
        if (result.IsCompleted) {
          break;
        }
      }

      // Mark the PipeReader as complete.
      await reader.CompleteAsync();
    }

    public bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line) {
      // Look for a EOL in the buffer.
      SequencePosition? position = buffer.PositionOf((byte)'\n');

      if (position == null) {
        line = default;
        return false;
      }

      // Skip the line + the \n.
      line = buffer.Slice(0, position.Value);
      buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
      return true;
    }

    // Dispose of Socket
    public void Dispose() {
      if (_tpsocket != null) {
        if (_tpsocket.Connected) {
          _tpsocket.Disconnect(false);
        }

        _tpsocket.Dispose();
      }
    }
  }
}
