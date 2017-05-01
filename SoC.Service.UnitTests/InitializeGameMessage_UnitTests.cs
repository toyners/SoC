
namespace Service.UnitTests
{
  using System;
  using Jabberwocky.SoC.Service;
  using Messages;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class InitializeGameMessage_UnitTests
  {
    #region Methods
    [Test]
    public void IsSameAs_FirstGameDataIsNull_ReturnsFalse()
    {
      var message1 = new InitializeGameMessage(null);
      var gameData2 = new GameInitializationData();
      var message2 = new InitializeGameMessage(gameData2);

      message1.IsSameAs(message2).ShouldBeFalse();
    }

    [Test]
    public void IsSameAs_SecondGameDataIsNull_ReturnsFalse()
    {
      var gameData1 = new GameInitializationData();
      var message1 = new InitializeGameMessage(gameData1);
      var message2 = new InitializeGameMessage(null);

      message1.IsSameAs(message2).ShouldBeFalse();
    }

    [Test]
    public void IsSameAs_NoData_ReturnsTrue()
    {
      var gameData1 = new GameInitializationData { BoardData = new Byte[0] };
      var message1 = new InitializeGameMessage(gameData1);
      var gameData2 = new GameInitializationData { BoardData = new Byte[0] };
      var message2 = new InitializeGameMessage(gameData2);

      message1.IsSameAs(message2).ShouldBeTrue();
    }

    [Test]
    public void IsSameAs_DataLengthIsDifferent_ReturnsFalse()
    {
      var gameData1 = new GameInitializationData { BoardData = new Byte[] { 1 } };
      var message1 = new InitializeGameMessage(gameData1);
      var gameData2 = new GameInitializationData { BoardData = new Byte[] { 2, 3 } };
      var message2 = new InitializeGameMessage(gameData2);

      message1.IsSameAs(message2).ShouldBeFalse();
    }

    [Test]
    public void IsSameAs_DataIsDifferent_ReturnsFalse()
    {
      var gameData1 = new GameInitializationData { BoardData = new Byte[] { 1, 2, 3 } };
      var message1 = new InitializeGameMessage(gameData1);
      var gameData2 = new GameInitializationData { BoardData = new Byte[] { 3, 2, 1 } };
      var message2 = new InitializeGameMessage(gameData2);

      message1.IsSameAs(message2).ShouldBeFalse();
    }

    [Test]
    public void IsSameAs_MessagesAreSame_ReturnsTrue()
    {
      var gameData1 = new GameInitializationData { BoardData = new Byte[] { 1, 2, 3 } };
      var message1 = new InitializeGameMessage(gameData1);
      var gameData2 = new GameInitializationData { BoardData = new Byte[] { 1, 2, 3 } };
      var message2 = new InitializeGameMessage(gameData2);

      message1.IsSameAs(message2).ShouldBeTrue();
    }
    #endregion 
  }
}
