
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Player_UnitTests
  {
    #region Methods
    [Test]
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
    [Category("Player")]
    public void Load_NameAndCountsReadFromStream_PlayerPropertiesAreCorrect()
    {
      // Arrange
      var player = new Player();
      var playerId = player.Id;

      // Act
      player.Load(null);

      // Assert
      player.Id.ShouldBe(playerId);
      player.Name.ShouldBe("Player");
      player.BrickCount.ShouldBe(1);
      player.BrickCount.ShouldBe(2);
      player.BrickCount.ShouldBe(3);
      player.BrickCount.ShouldBe(4);
      player.BrickCount.ShouldBe(5);
    }
    #endregion 
  }
}
