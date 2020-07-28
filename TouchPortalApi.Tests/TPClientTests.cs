using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using TouchPortalApi.Configuration;
using TouchPortalApi.Interfaces;
using TouchPortalApi.Models.TouchPortal.Requests;
using Xunit;

namespace TouchPortalApi.Tests {
  public class TPClientTests {
    [Fact]
    public void TPClient_Instantiate() {
      // arrange
      Mock<ITPSocket> socketMock = new Mock<ITPSocket>();

      // act/asert
      //Assert.Throws<ArgumentNullException>(() => new TPClient(null, null));
    }

    [Fact]
    public void Test1() {
      // arrange
      List<byte[]> testData = new List<byte[]>();
      Mock<ITPSocket> socketMock = new Mock<ITPSocket>();
      socketMock.Setup(s => s.Connected).Returns(true);
      Mock<IOptionsSnapshot<TouchPortalApiOptions>> optionsMock = new Mock<IOptionsSnapshot<TouchPortalApiOptions>>();

      socketMock.Setup(x => x.Connected).Returns(true);
      socketMock.Setup((x) => x.Send(Capture.In<byte[]>(testData), It.IsAny<int>(), It.IsAny<SocketFlags>()));

      // act

      //var x = new TPClient(optionsMock.Object, socketMock.Object);
      //x.Send(new TPRequestState { Id = "10", Type = "Test", Value = "99" });
      
      // assert
      //Assert.True(testData.Count > 0);
    }
  }
}
