
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.IO;
  using System.Text;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Player_UnitTests
  {
    #region Methods
    [Test]
    [Category("All")]
    [Category("Player")]
    public void AddResources_VariousResourceKinds_CountsAreUpdated()
    {
      // Arrange
      var player = new Player();

      // Act
      player.AddResources(new ResourceClutch(5, 4, 3, 2, 1));

      // Assert
      player.BrickCount.ShouldBe(5);
      player.GrainCount.ShouldBe(4);
      player.LumberCount.ShouldBe(3);
      player.OreCount.ShouldBe(2);
      player.WoolCount.ShouldBe(1);
    }

    [Test]
    [Category("All")]
    [Category("Player")]
    public void RemoveResources_VariousResourceKinds_CountsAreUpdated()
    {
      // Arrange
      var player = new Player();

      // Act
      player.AddResources(new ResourceClutch(5, 4, 3, 2, 1));
      player.RemoveResources(new ResourceClutch(4, 3, 2, 1, 0));

      // Assert
      player.BrickCount.ShouldBe(1);
      player.GrainCount.ShouldBe(1);
      player.LumberCount.ShouldBe(1);
      player.OreCount.ShouldBe(1);
      player.WoolCount.ShouldBe(1);
    }

    [Test]
    [Category("All")]
    [Category("Player")]
    public void RemoveResources_ResultingTotalsWillBeBeLowerThanZero_ThrowsMeaningfulException()
    {
      // Arrange
      var player = new Player();

      // Act
      player.AddResources(new ResourceClutch(1, 0, 0, 0, 0));
      Action action = () => { player.RemoveResources(new ResourceClutch(2, 0, 0, 0, 0)); };

      // Assert
      Should.Throw<ArithmeticException>(action).Message.ShouldBe("No resource count can be negative.");
    }

    [Test]
    [Category("All")]
    [Category("Player")]
    public void Load_NameOnlyInStream_PlayerPropertiesAreCorrect()
    {
      // Arrange
      var player = new Player();
      var playerId = Guid.NewGuid();
      var content = "<player id=\"" + playerId + "\" name=\"Player\" />";
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      using (var memoryStream = new MemoryStream(contentBytes))
      {
        player.Load(memoryStream);
      }

      // Assert
      player.Id.ShouldBe(playerId);
      player.Name.ShouldBe("Player");
      player.BrickCount.ShouldBe(0);
      player.GrainCount.ShouldBe(0);
      player.LumberCount.ShouldBe(0);
      player.OreCount.ShouldBe(0);
      player.WoolCount.ShouldBe(0);
    }

    [Test]
    [Category("All")]
    [Category("Player")]
    public void Load_NoIdInStream_ThrowsMeaningfulException()
    {
      // Arrange
      var player = new Player();
      var playerId = Guid.NewGuid();
      var content = "<player name=\"Player\" brick=\"1\" grain=\"2\" lumber=\"3\" ore=\"4\" wool=\"5\" />";
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      Action action = () =>
      {
        using (var memoryStream = new MemoryStream(contentBytes))
        {
          player.Load(memoryStream);
        }
      };

      // Assert
      Should.Throw<Exception>(action).Message.ShouldBe("No id found for player in stream.");
    }

    [Test]
    [Category("All")]
    [Category("Player")]
    public void Load_NoNameInStream_ThrowsMeaningfulException()
    {
      // Arrange
      var player = new Player();
      var playerId = Guid.NewGuid();
      var content = "<player id=\"" + playerId + "\" brick=\"1\" grain=\"2\" lumber=\"3\" ore=\"4\" wool=\"5\" />";
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      Action action = () =>
      {
        using (var memoryStream = new MemoryStream(contentBytes))
        {
          player.Load(memoryStream);
        }
      };

      // Assert
      Should.Throw<Exception>(action).Message.ShouldBe("No name found for player in stream.");
    }
    #endregion 
  }
}
