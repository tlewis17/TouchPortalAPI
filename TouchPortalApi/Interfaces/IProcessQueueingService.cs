using System;
using System.Buffers;

namespace TouchPortalApi.Interfaces {
  public interface IProcessQueueingService {
    void SetupChannel(Action<ReadOnlySequence<byte>> callback);
    void Enqueue(ReadOnlySequence<byte> sequence);
    void Stop();
  }
}