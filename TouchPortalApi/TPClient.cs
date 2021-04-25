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
  /// <summary>
  /// The Touch Portal Client Class
  /// </summary>
  public class TPClient : ITPClient {
    private readonly IOptionsMonitor<TouchPortalApiOptions> _options;
    private ITPSocket _tpsocket;
    private readonly IProcessQueueingService _processQueueingService;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">Configured TP Options</param>
    /// <param name="socket">The TP Socket Object</param>
    /// <param name="processQueueingService">THe Process Queue Service</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    public TPClient(IOptionsMonitor<TouchPortalApiOptions> options, ITPSocket socket, IProcessQueueingService processQueueingService) {
      _options = options ?? throw new ArgumentNullException(nameof(options));
      _tpsocket = socket ?? throw new ArgumentNullException(nameof(socket));
      _processQueueingService = processQueueingService ?? throw new ArgumentNullException(nameof(processQueueingService));

      // Add this somewhere in your project, most likely in Startup.cs
      JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };

      InitializeConnection();
    }

    /// <summary>
    /// Initialize TP Socket if it isn't already.
    /// </summary>
    private void InitializeConnection() {
      if (_tpsocket == null || !_tpsocket.Connected) {
        var ipe = new IPEndPoint(IPAddress.Parse(_options.CurrentValue.ServerIp), _options.CurrentValue.ServerPort);
        _tpsocket = new TPSocket(ipe.AddressFamily);
        _tpsocket.Connect(ipe);
      }
    }

    /// <summary>
    /// SendAsync Message to TP through Socket
    /// </summary>
    /// <param name="model">Object to send</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task SendAsync(object model, CancellationToken cancellationToken = default) {
      string request = PrepareMessage(model);
      var bytesSent = Encoding.ASCII.GetBytes(request);

      await _tpsocket.SendAsync(bytesSent, cancellationToken);
    }

    /// <summary>
    /// Serialzies the object
    /// </summary>
    /// <param name="model">The object to serialize</param>
    /// <returns>A string representation of the object</returns>
    internal string PrepareMessage(object model) {
      return $"{JsonConvert.SerializeObject(model)}\n";
    }

    /// <summary>
    /// Process the read and write pipes
    /// </summary>
    public async Task ProcessPipes() {
      var pipe = new Pipe();
      Task writing = FillPipeAsync(pipe.Writer);
      Task reading = ReadPipeAsync(pipe.Reader);

      await Task.WhenAll(reading, writing).ConfigureAwait(false);
      if(!_tpsocket.Connected)
        throw new SocketException();
    }

    /// <summary>
    /// Fill the pipe with the result.
    /// </summary>
    /// <param name="writer">The pipe writer</param>
    private async Task FillPipeAsync(PipeWriter writer) {
      const int minimumBufferSize = 512;

      while (true) {
        // Allocate at least 512 bytes from the PipeWriter.
        Memory<byte> memory = writer.GetMemory(minimumBufferSize);
        try {
          int bytesRead = await _tpsocket.ReceiveAsync(memory);
          if (bytesRead == 0) {
            break;
          }
          // Tell the PipeWriter how much was read from the Socket.
          writer.Advance(bytesRead);
        } catch {
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

    /// <summary>
    /// Read the result from the pipe.
    /// </summary>
    /// <param name="reader">The pipe reader</param>
    private async Task ReadPipeAsync(PipeReader reader) {
      while (true) {
        ReadResult result = await reader.ReadAsync();
        ReadOnlySequence<byte> buffer = result.Buffer;

        while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line)) {
          // Process the line.
          // Put in queue
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

    /// <summary>
    /// Try to read a single line terminated with line return.
    /// </summary>
    /// <param name="buffer">The ReadOnlySequence buffer</param>
    /// <param name="line">The output resulting sequence line</param>
    private bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line) {
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

    /// <summary>
    /// Dispose of Socket 
    /// </summary>
    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing) {
      if (disposing && _tpsocket != null) {
        if (_tpsocket.Connected) {
          _tpsocket.Disconnect(false);
        }

        _tpsocket.Dispose();
      }
    }
  }
}
