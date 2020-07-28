using System;
using System.Buffers;
using System.Text;

namespace TouchPortalApi.Extensions {
  public static class ReadOnlySquenceByteExtension {
    /// <summary>
    /// Parses UTF8 characters in the ReadOnlySequence
    /// </summary>
    /// <param name="slice">Aligned slice of ReadOnlySequence that contains the UTF8 string bytes. Use slice before calling this function to ensure you have an aligned slice.</param>
    /// <param name="stringLengthEstimate">The amount of characters in the final string. You should use a header before the string bytes for the best accuracy. If you are not sure -1 means that the most pessimistic estimate will be used: slice.Length</param>
    /// <returns>a string parsed from the bytes in the ReadOnlySequence</returns>
    public static string ParseAsUTF8String(this ReadOnlySequence<byte> slice, int stringLengthEstimate = -1) {
      if (stringLengthEstimate == -1) {
        stringLengthEstimate = (int)slice.Length; //overestimate
      }

      var decoder = Encoding.UTF8.GetDecoder();
      var preProcessedBytes = 0;
      var processedCharacters = 0;
      Span<char> characterSpan = stackalloc char[stringLengthEstimate];
      foreach (var memory in slice) {
        preProcessedBytes += memory.Length;
        var isLast = (preProcessedBytes == slice.Length);
        var emptyCharSlice = characterSpan.Slice(processedCharacters, characterSpan.Length - processedCharacters);
        var charCount = decoder.GetChars(memory.Span, emptyCharSlice, isLast);
        processedCharacters += charCount;
      }
      var finalCharacters = characterSpan.Slice(0, processedCharacters);
      return new string(finalCharacters);
    }
  }
}
