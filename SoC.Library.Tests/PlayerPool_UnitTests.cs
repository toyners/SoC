
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.IO;
  using System.Text;
  using System.Xml;
  using Interfaces;
    using Jabberwocky.SoC.Library.GameBoards;
    using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class PlayerPool_UnitTests
  {
    #region Methods
    [Test]
    [Category("All")]
    [Category("PlayerPool")]
    public void CreatePlayer_AllPropertiesInStream_PlayerPropertiesAreCorrect()
    {
      // Arrange
      IPlayer player = null;
      var playerId = Guid.NewGuid();
      var content = "<player id=\"" + playerId + "\" name=\"Player\" iscomputer=\"true\" brick=\"1\" grain=\"2\" lumber=\"3\" ore=\"4\" wool=\"5\" />";
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      using (var memoryStream = new MemoryStream(contentBytes))
      {
        using (var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, CloseInput = false, IgnoreComments = true, IgnoreWhitespace = true }))
        {
          reader.Read();
          var playerPool = new PlayerPool();
          player = playerPool.CreatePlayer(reader);
        } 
      }

      // Assert
      player.ShouldNotBeNull();
      player.ShouldBeOfType<ComputerPlayer>();
      player.Id.ShouldBe(playerId);
      player.Name.ShouldBe("Player");
      player.Resources.BrickCount.ShouldBe(1);
      player.Resources.GrainCount.ShouldBe(2);
      player.Resources.LumberCount.ShouldBe(3);
      player.Resources.OreCount.ShouldBe(4);
      player.Resources.WoolCount.ShouldBe(5);
      player.IsComputer.ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("PlayerPool")]
    public void CreatePlayer_NameOnlyInStream_PlayerPropertiesAreCorrect()
    {
      // Arrange
      IPlayer player = null;
      var playerId = Guid.NewGuid();
      var content = "<player id=\"" + playerId + "\" name=\"Player\" />";
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      using (var memoryStream = new MemoryStream(contentBytes))
      {
        using (var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, CloseInput = false, IgnoreComments = true, IgnoreWhitespace = true }))
        {
          reader.Read();
          var playerPool = new PlayerPool();
          player = playerPool.CreatePlayer(reader);
        }
      }

      // Assert
      player.Id.ShouldBe(playerId);
      player.Name.ShouldBe("Player");
      player.Resources.BrickCount.ShouldBe(0);
      player.Resources.GrainCount.ShouldBe(0);
      player.Resources.LumberCount.ShouldBe(0);
      player.Resources.OreCount.ShouldBe(0);
      player.Resources.WoolCount.ShouldBe(0);
      player.IsComputer.ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("PlayerPool")]
    public void CreatePlayer_NoIdInStream_ThrowsMeaningfulException()
    {
      // Arrange
      var content = "<player name=\"Player\" brick=\"1\" grain=\"2\" lumber=\"3\" ore=\"4\" wool=\"5\" />";
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      Action action = () =>
      {
        using (var memoryStream = new MemoryStream(contentBytes))
        {
          using (var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, CloseInput = false, IgnoreComments = true, IgnoreWhitespace = true }))
          {
            var playerPool = new PlayerPool();
            playerPool.CreatePlayer(reader);
          }
        }
      };

      // Assert
      Should.Throw<Exception>(action).Message.ShouldBe("No id found for player in stream.");
    }

    [Test]
    [Category("All")]
    [Category("PlayerPool")]
    public void CreatePlayer_NoNameInStream_ThrowsMeaningfulException()
    {
      // Arrange
      var content = "<player id=\"" + Guid.NewGuid() + "\" brick=\"1\" grain=\"2\" lumber=\"3\" ore=\"4\" wool=\"5\" />";
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      Action action = () =>
      {
        using (var memoryStream = new MemoryStream(contentBytes))
        {
          using (var reader = XmlReader.Create(memoryStream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, CloseInput = false, IgnoreComments = true, IgnoreWhitespace = true }))
          {
            reader.Read();
            var playerPool = new PlayerPool();
            playerPool.CreatePlayer(reader);
          }
        }
      };

      // Assert
      Should.Throw<Exception>(action).Message.ShouldBe("No name found for player in stream.");
    }

    [Test]
    [Category("All")]
    [Category("PlayerPool")]
    public void CreatePlayer_IsComputerParameterIsFalse_PlayerInstanceIsNotComputerControlled()
    {
      var playerPool = new PlayerPool();
      var player = playerPool.CreatePlayer();

      player.IsComputer.ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("PlayerPool")]
    public void CreateComputerPlayer_NoGameBoardArgument_PlayerInstanceIsComputerControlled()
    {
      var playerPool = new PlayerPool();
      var player = playerPool.CreateComputerPlayer((GameBoard)null, null, null);

      player.IsComputer.ShouldBeTrue();
    }
    #endregion
  }
}
