
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class ResourceProvider_UnitTests
  {
    #region Methods
    [Test]
    public void EqualsOperator_OneParameterIsNull_ReturnsFalse()
    {
      var r1 = new ResourceProvider();

      (r1 == (ResourceProvider)null).ShouldBeFalse();
    }

    [Test]
    public void EqualsOperator_BothParametersAreNull_ReturnsTrue()
    {
      ((ResourceProvider)null == (ResourceProvider)null).ShouldBeTrue();
    }

    [Test]
    public void EqualsOperator_ResourceProviderWithDifferentResourceTypes_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Grain, 2);

      (r1 == r2).ShouldBeFalse();
    }

    [Test]
    public void EqualsOperator_ResourceProviderWithDifferentProductionNumbers_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Brick, 3);

      (r1 == r2).ShouldBeFalse();
    }

    [Test]
    public void EqualsOperator_ResourceProviderWithSameResourceTypeAndProductionNumbers_ReturnsTrue()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Brick, 2);

      (r1 == r2).ShouldBeTrue();
    }

    [Test]
    public void EqualsOperator_ResourceProviderIsSame_ReturnsTrue()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = r1;

      (r1 == r2).ShouldBeTrue();
    }

    [Test]
    public void Equals_ParameterIsOfDifferentType_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      r1.Equals(4).ShouldBeFalse(); // Will box value type.
    }

    [Test]
    public void Equals_ParameterIsNull_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      r1.Equals(null).ShouldBeFalse();
    }

    [Test]
    public void Equals_ParameterIsSame_Returnstrue()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = r1;
      r1.Equals(r2).ShouldBeTrue();
    }

    [Test]
    public void Equals_ResourceProviderWithDifferentResourceTypes_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Grain, 2);

      r1.Equals(r2).ShouldBeFalse();
    }

    [Test]
    public void Equals_ResourceProviderWithDifferentProductionNumbers_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Brick, 3);

      r1.Equals(r2).ShouldBeFalse();
    }

    [Test]
    public void Equals_ResourceProviderWithSameResourceTypeAndProductionNumbers_ReturnsTrue()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Brick, 2);

      r1.Equals(r2).ShouldBeTrue();
    }

    [Test]
    public void NotEqualsOperator_OneParameterIsNull_ReturnsTrue()
    {
      var r1 = new ResourceProvider();

      (r1 != (ResourceProvider)null).ShouldBeTrue();
    }

    [Test]
    public void NotEqualsOperator_BothParametersAreNull_ReturnsFalse()
    {
      ((ResourceProvider)null != (ResourceProvider)null).ShouldBeFalse();
    }

    [Test]
    public void NotEqualsOperator_ResourceProviderWithDifferentResourceTypes_ReturnsTrue()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Grain, 2);

      (r1 != r2).ShouldBeTrue();
    }

    [Test]
    public void NotEqualsOperator_ResourceProviderWithDifferentProductionNumbers_ReturnsTrue()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Brick, 3);

      (r1 != r2).ShouldBeTrue();
    }

    [Test]
    public void NotEqualsOperator_ResourceProviderWithSameResourceTypeAndProductionNumbers_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = new ResourceProvider(ResourceTypes.Brick, 2);

      (r1 != r2).ShouldBeFalse();
    }

    [Test]
    public void NotEqualsOperator_ResourceProviderIsSame_ReturnsFalse()
    {
      var r1 = new ResourceProvider(ResourceTypes.Brick, 2);
      var r2 = r1;

      (r1 != r2).ShouldBeFalse();
    }

    [Test]
    public void GetHashCode_TwoResourceProviderInstances_HashCodesAreEqual()
    {
      var r1 = new ResourceProvider(ResourceTypes.Wool, 12).GetHashCode();
      var r2 = new ResourceProvider(ResourceTypes.Wool, 12).GetHashCode();

      r1.ShouldBe(r2);
    }

    [Test]
    public void GetHashCode_AllPossibleResourceProviderCombinations_HashCodesAreUnique()
    {
      var hashCodes = new List<Int32>();

      foreach (var type in Enum.GetValues(typeof(ResourceTypes)))
      {
        foreach (var production in new UInt32[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })
        {
          var hashCode = new ResourceProvider((ResourceTypes)type, production).GetHashCode();

          if (hashCodes.Contains(hashCode))
          {
            throw new Exception(String.Format("Duplicate hash code found for {0} and {1}.", type, production));
          }

          hashCodes.Add(hashCode);
        }
      }
    }

    [Test]
    public void ResourceProvider_NoParameters_NoProductionPossible()
    {
      var r1 = new ResourceProvider();

      r1.Type.ShouldBe(ResourceTypes.None);
    }

    [Test]
    public void ResourceProvider_ProductionBelowTwoDiceRange_ThrowsException()
    {
      Action action = () => { new ResourceProvider(ResourceTypes.Brick, 1); };

      action.ShouldThrow<ArgumentOutOfRangeException>().Message.ShouldBe("Parameter 'productionNumber' must be within range 2..12");
    }

    [Test]
    public void ResourceProvider_ProductionAboveTwoDiceRange_ThrowsException()
    {
      Action action = () => { new ResourceProvider(ResourceTypes.Brick, 13); };

      action.ShouldThrow<ArgumentOutOfRangeException>().Message.ShouldBe("Parameter 'productionNumber' must be within range 2..12");
    }
    #endregion
  }

  [TestFixture]
  public class ResourceClutch_UnitTests
  {
    #region Methods
    [Test]
    [Category("ResourceClutch")]
    public void EqualsOperator_BothParametersAreZero_ReturnsTrue()
    {
      (ResourceClutch.Zero == ResourceClutch.Zero).ShouldBeTrue();
    }

    [Test]
    [Category("ResourceClutch")]
    public void EqualsOperator_ResourceCountsAreDifferent_ReturnsFalse()
    {
      var r1 = new ResourceClutch(1, 2, 3, 4, 5);
      var r2 = new ResourceClutch(1, 2, 3, 4, 0);

      (r1 == r2).ShouldBeFalse();
    }

    [Test]
    [Category("ResourceClutch")]
    public void EqualsOperator_ResourceCountsAreSame_ReturnsTrue()
    {
      var r1 = new ResourceClutch(1, 2, 3, 4, 5);
      var r2 = new ResourceClutch(1, 2, 3, 4, 5);

      (r1 == r2).ShouldBeTrue();
    }
    #endregion
  }
}
