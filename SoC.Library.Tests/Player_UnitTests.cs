
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("Player")]
  public class Player_UnitTests
  {
    #region Methods
    [Test]
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
    [TestCase(-1)]
    [TestCase(5)]
    public void LoseResourceAtIndex_OutOfBounds_ThrowsMeaningfulException(Int32 index)
    {
      // Arrange
      var player = new Player();
      player.AddResources(ResourceClutch.OneOfEach);

      // Act
      Action action = () => { player.LoseResourceAtIndex(index); };

      // Assert
      action.ShouldThrow<IndexOutOfRangeException>().Message.ShouldBe("Index " + index + " is out of bounds (0.." + (player.ResourcesCount - 1) + ").");
    }

    [Test]
    [TestCase(0, 1, 0, 0, 0, 0)]
    [TestCase(1, 0, 1, 0, 0, 0)]
    [TestCase(2, 0, 0, 1, 0, 0)]
    [TestCase(3, 0, 0, 0, 1, 0)]
    [TestCase(4, 0, 0, 0, 0, 1)]
    public void LoseResourceAtIndex_SafeIndex_ReturnsExpectedResource(Int32 index, Int32 expectedBrickCount, Int32 expectedGrainCount, Int32 expectedLumberCount, Int32 expectedOreCount, Int32 expectedWoolCount)
    {
      // Arrange
      var player = new Player();
      player.AddResources(ResourceClutch.OneOfEach);

      var expectedResourceClutch = new ResourceClutch(expectedBrickCount, expectedGrainCount, expectedLumberCount, expectedOreCount, expectedWoolCount);

      // Act
      var actualResourceClutch = player.LoseResourceAtIndex(index);

      // Assert
      actualResourceClutch.ShouldBe(expectedResourceClutch);
      player.ResourcesCount.ShouldBe(4);
      player.BrickCount.ShouldBe(1 - expectedBrickCount);
      player.GrainCount.ShouldBe(1 - expectedGrainCount);
      player.LumberCount.ShouldBe(1 - expectedLumberCount);
      player.OreCount.ShouldBe(1 - expectedOreCount);
      player.WoolCount.ShouldBe(1 - expectedWoolCount);
    }

    [Test]
    [TestCase(0, ResourceTypes.Brick)]
    [TestCase(1, ResourceTypes.Brick)]
    [TestCase(10, ResourceTypes.Brick)]
    [TestCase(0, ResourceTypes.Grain)]
    [TestCase(1, ResourceTypes.Grain)]
    [TestCase(10, ResourceTypes.Grain)]
    [TestCase(0, ResourceTypes.Lumber)]
    [TestCase(1, ResourceTypes.Lumber)]
    [TestCase(10, ResourceTypes.Lumber)]
    [TestCase(0, ResourceTypes.Ore)]
    [TestCase(1, ResourceTypes.Ore)]
    [TestCase(10, ResourceTypes.Ore)]
    [TestCase(0, ResourceTypes.Wool)]
    [TestCase(1, ResourceTypes.Wool)]
    [TestCase(10, ResourceTypes.Wool)]
    public void LoseResourcesOfType_OpponentHasResourcesOfType_ReturnsExpectedResource(Int32 count, ResourceTypes resourceType)
    {
      // Arrange
      var player = new Player();
      player.AddResources(new ResourceClutch(count, count, count, count, count));

      ResourceClutch expectedResourceClutch = ResourceClutch.Zero;

      switch (resourceType)
      {
        case ResourceTypes.Brick: expectedResourceClutch = new ResourceClutch(count, 0, 0, 0, 0); break;
        case ResourceTypes.Grain: expectedResourceClutch = new ResourceClutch(0, count, 0, 0, 0); break;
        case ResourceTypes.Lumber: expectedResourceClutch = new ResourceClutch(0, 0, count, 0, 0); break;
        case ResourceTypes.Ore: expectedResourceClutch = new ResourceClutch(0, 0, 0, count, 0); break;
        case ResourceTypes.Wool: expectedResourceClutch = new ResourceClutch(0, 0, 0, 0, count); break;
      }

      // Act
      var actualResourceClutch = player.LoseResourcesOfType(resourceType);

      // Assert
      actualResourceClutch.ShouldBe(expectedResourceClutch);
      player.ResourcesCount.ShouldBe(count * 4);

      switch (resourceType)
      {
        case ResourceTypes.Brick: player.BrickCount.ShouldBe(0); break;
        case ResourceTypes.Grain: player.GrainCount.ShouldBe(0); break;
        case ResourceTypes.Lumber: player.LumberCount.ShouldBe(0); break;
        case ResourceTypes.Ore: player.OreCount.ShouldBe(0); break;
        case ResourceTypes.Wool: player.WoolCount.ShouldBe(0); break;
      }
    }
    #endregion 
  }
}
