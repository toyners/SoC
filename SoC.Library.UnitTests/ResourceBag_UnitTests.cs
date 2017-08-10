
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class ResourceBag_UnitTests
  {
    #region Methods
    [TestFixtureSetUp]
    public void SetupBeforeAllTests()
    {
    }

    [SetUp]
    public void SetupBeforeEachTest()
    {
    }

    [Test]
    public void Add_VariousTypes_CountsAreCorrect()
    {
      // Arrange
      var resourceBag = new ResourceBag();
      var resourceClutch = new ResourceClutch(1, 2, 3, 4, 5);

      // Act
      resourceBag.Add(resourceClutch);

      // Assert
      resourceBag.BrickCount.ShouldBe(1);
      resourceBag.GrainCount.ShouldBe(2);
      resourceBag.LumberCount.ShouldBe(3);
      resourceBag.OreCount.ShouldBe(4);
      resourceBag.WoolCount.ShouldBe(5);
    }
    #endregion 
  }
}
