
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.IO;
  using System.Text;
  using Interfaces;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class PlayerPool_UnitTests
  {
    #region Methods
    [Test]
    [Category("All")]
    [Category("Player")]
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
        var playerPool = new PlayerPool();
        player = playerPool.CreatePlayer(memoryStream);
      }

      // Assert
      player.ShouldNotBeNull();
      player.ShouldBeOfType<IComputerPlayer>();
      player.Id.ShouldBe(playerId);
      player.Name.ShouldBe("Player");
      player.BrickCount.ShouldBe(1);
      player.GrainCount.ShouldBe(2);
      player.LumberCount.ShouldBe(3);
      player.OreCount.ShouldBe(4);
      player.WoolCount.ShouldBe(5);
      player.IsComputer.ShouldBeTrue();
    }
    #endregion 
  }
}
