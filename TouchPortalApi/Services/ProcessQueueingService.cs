using System;
using System.Buffers;
using System.Threading.Channels;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;

namespace TouchPortalApi.Services {
  internal class ProcessQueueingService : IProcessQueueingService {
    private ChannelWriter<ReadOnlySequence<byte>> _writer;

    public void SetupChannel(Action<ReadOnlySequence<byte>> callback) {
      var channel = Channel.CreateUnbounded<ReadOnlySequence<byte>>(new UnboundedChannelOptions() { SingleReader = true });
      var reader = channel.Reader;
      _writer = channel.Writer;

      Task.Run(async () => {
        while (await reader.WaitToReadAsync()) {
          while (reader.TryRead(out var sequence)) {
            try {
              callback.DynamicInvoke(sequence);
            } catch (Exception ex) {
              Console.WriteLine(ex);
            }
          }
        }
      });
    }

    public void Enqueue(ReadOnlySequence<byte> sequence) {
      _writer.TryWrite(sequence);
    }

    public void Stop() {
      _writer.Complete();
    }
  }
}
