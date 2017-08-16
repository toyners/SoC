
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Player_UnitTests
  {
    #region Methods
    [Test]
    [Category("Player")]
    public void RemoveResources_VariousResourceKinds_CountsAreUpdated()
    {
      // Arrange
      var player = new Player();
      var resourceClutch = new ResourceClutch(1, 2, 3, 4, 5);

      // Act
      player.RemoveResources(resourceClutch);

      // Assert
      player.BrickCount.ShouldBe(1);
      player.GrainCount.ShouldBe(2);
      player.LumberCount.ShouldBe(3);
      player.OreCount.ShouldBe(4);
      player.WoolCount.ShouldBe(5);
    }
    #endregion 
  }
}
