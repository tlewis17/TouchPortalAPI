using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using TouchPortalApi.Interfaces;

namespace TouchPortalApi.Services {
  internal class ProcessQueueingService : IProcessQueueingService {
    private Dictionary<string, Delegate> _events = new Dictionary<string, Delegate>();
    private BlockingCollection<ReadOnlySequence<byte>> _processQueue = new BlockingCollection<ReadOnlySequence<byte>>();
    private ChannelWriter<ReadOnlySequence<byte>> _writer;

    public ProcessQueueingService() {

    }

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

    public void AddToProcessQueue(ReadOnlySequence<byte> data) {
      _processQueue.Add(data);
    }

    public ReadOnlySequence<byte> GetNextFromQueue() {
      return _processQueue.Take();
    }

    public bool AddEvent<T>(string id, Action<T> callback) {
      if (_events.ContainsKey(id)) {
        return false;
      }

      _events.Add(id, callback);
      return true;
    }

    public void ExecuteEventCallback<T>(string id, T data) {
      if (_events.ContainsKey(id)) {
        _events[id].DynamicInvoke(data);
      }
    }
  }
}
