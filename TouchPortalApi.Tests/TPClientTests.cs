using Moq;
using System.Collections.Generic;
using TouchPortalApi.Interfaces;
using Xunit;

namespace TouchPortalApi.Tests {
  public class TPClientTests {
    [Fact]
    public void Test1() {
      // arrange
      List<byte[]> testData = new List<byte[]>();
      Mock<ITPSocket> socketMock = new Mock<ITPSocket>();
      socketMock.Setup(s => s.Connected).Returns(true);

      socketMock.Setup(x => x.Connected).Returns(true);

      Assert.NotNull(testData);
    }
  }
}
