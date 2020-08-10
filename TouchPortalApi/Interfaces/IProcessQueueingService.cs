using System;
using System.Buffers;

namespace TouchPortalApi.Interfaces {
  public interface IProcessQueueingService {
    bool AddEvent<T>(string id, Action<T> callback);
    void ExecuteEventCallback<T>(string id, T data);
    void SetupChannel(Action<ReadOnlySequence<byte>> callback);
    void Enqueue(ReadOnlySequence<byte> sequence);
    void Stop();
  }
}