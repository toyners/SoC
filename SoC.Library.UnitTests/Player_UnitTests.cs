
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
    public void LoseRandomResource_NoResources_ReturnsZeroResources()
    {
      // Arrange
      var player = new Player();
      var mockDice = Substitute.For<INumberGenerator>();

      // Act
      var result = player.LoseRandomResource(mockDice);

      // Assert
      result.ShouldBe(ResourceClutch.Zero);
      player.ResourcesCount.ShouldBe(0);
    }

    [Test]
    public void LoseRandomResource_OneResource_ReturnsOnlyResource()
    {
      // Arrange
      var player = new Player();
      player.AddResources(ResourceClutch.OneLumber);
      var mockDice = Substitute.For<INumberGenerator>();
      mockDice.GetRandomNumberBetweenZeroAndMaximum(1).Returns(0);

      // Act
      var result = player.LoseRandomResource(mockDice);

      // Assert
      result.ShouldBe(ResourceClutch.OneLumber);
      player.ResourcesCount.ShouldBe(0);
    }

    [Test]
    [TestCase(0, 0, 1, 1, 1, 1)]
    [TestCase(1, 1, 0, 1, 1, 1)]
    [TestCase(2, 1, 1, 0, 1, 1)]
    [TestCase(3, 1, 1, 1, 0, 1)]
    [TestCase(4, 1, 1, 1, 1, 0)]
    public void LoseRandomResource_OneOfEachResource_ReturnsExpectedResource(Int32 index, Int32 expectedBrickCount, Int32 expectedGrainCount, Int32 expectedLumberCount, Int32 expectedOreCount, Int32 expectedWoolCount)
    {
      // Arrange
      var player = new Player();
      player.AddResources(ResourceClutch.OneOfEach);
      var mockDice = Substitute.For<INumberGenerator>();
      mockDice.GetRandomNumberBetweenZeroAndMaximum(player.ResourcesCount).Returns(index);

      var expectedResourceClutch = new ResourceClutch(
        player.BrickCount - expectedBrickCount,
        player.GrainCount - expectedGrainCount,
        player.LumberCount - expectedLumberCount,
        player.OreCount - expectedOreCount,
        player.WoolCount - expectedWoolCount);     

      // Act
      var result = player.LoseRandomResource(mockDice);

      // Assert
      var expectedTotal = expectedBrickCount + expectedGrainCount + expectedLumberCount + expectedOreCount + expectedWoolCount;
      result.ShouldBe(expectedResourceClutch);
      player.ResourcesCount.ShouldBe(expectedTotal);
      player.BrickCount.ShouldBe(expectedBrickCount);
      player.GrainCount.ShouldBe(expectedGrainCount);
      player.LumberCount.ShouldBe(expectedLumberCount);
      player.OreCount.ShouldBe(expectedOreCount);
      player.WoolCount.ShouldBe(expectedWoolCount);
    }

    [Test]
    [TestCase(-1)]
    [TestCase(5)]
    public void LoseResource_OutOfBounds_ThrowsMeaningfulException(Int32 index)
    {
      // Arrange
      var player = new Player();
      player.AddResources(ResourceClutch.OneOfEach);

      // Act
      Action action = () => { player.LoseResource(index); };

      // Assert
      action.ShouldThrow<IndexOutOfRangeException>().Message.ShouldBe("Index " + index + " is out of bounds (0.." + (player.ResourcesCount - 1) + ").");
    }

    [Test]
    [TestCase(0, 1, 0, 0, 0, 0)]
    [TestCase(1, 0, 1, 0, 0, 0)]
    [TestCase(2, 0, 0, 1, 0, 0)]
    [TestCase(3, 0, 0, 0, 1, 0)]
    [TestCase(4, 0, 0, 0, 0, 1)]
    public void LoseResource_SafeIndex_ReturnsExpectedResource(Int32 index, Int32 expectedBrickCount, Int32 expectedGrainCount, Int32 expectedLumberCount, Int32 expectedOreCount, Int32 expectedWoolCount)
    {
      // Arrange
      var player = new Player();
      player.AddResources(ResourceClutch.OneOfEach);

      var expectedResourceClutch = new ResourceClutch(expectedBrickCount, expectedGrainCount, expectedLumberCount, expectedOreCount, expectedWoolCount);

      // Act
      var actualResourceClutch = player.LoseResource(index);

      // Assert
      actualResourceClutch.ShouldBe(expectedResourceClutch);
    }
    #endregion 
  }
}
