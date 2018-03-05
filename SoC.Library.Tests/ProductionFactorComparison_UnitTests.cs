
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using LocalGameController_Tests;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("ProductionFactorComparison")]
  public class ProductionFactorComparison_UnitTests
  {
    #region Methods
    [Test]
    public void ProductionFactorComparison_ReturnsLocationList()
    {
      var list = new List<UInt32>(new UInt32[] { 2, 12, 3, 11, 4, 10, 5, 9, 6, 8 });
      list.Sort((pf1, pf2) =>
      {
        return ProductionFactorComparison.Compare(pf1, pf2);
      });

      list.ShouldContainExact(new UInt32[] { 8, 6, 9, 5, 10, 4, 11, 3, 12, 2 });
    }

    [Test]
    [TestCase(7u, 2u, "pf1")]
    [TestCase(1u, 2u, "pf1")]
    [TestCase(13u, 2u, "pf1")]
    [TestCase(2u, 7u, "pf2")]
    [TestCase(2u, 1u, "pf2")]
    [TestCase(2u, 13u, "pf2")]
    public void ProductionFactorComparison_ValueIsInvalid_ThrowsMeaningfulException(UInt32 pf1, UInt32 pf2, String parameterName)
    {
      Action action = () => { var i = ProductionFactorComparison.Compare(pf1, pf2); };

      action.ShouldThrow<ArgumentOutOfRangeException>().Message.ShouldBe("Value cannot be 7 or less than 2 or greater than 12.\r\nParameter name: " + parameterName);
    }
    #endregion
  }
}
