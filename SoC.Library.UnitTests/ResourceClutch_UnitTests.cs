
namespace Jabberwocky.SoC.Library.UnitTests
{
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class ResourceClutch_UnitTests
  {
    #region Methods
    [Test]
    [Category("All")]
    [Category("ResourceClutch")]
    public void EqualsOperator_BothParametersAreZero_ReturnsTrue()
    {
      (ResourceClutch.Zero == ResourceClutch.Zero).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("ResourceClutch")]
    public void EqualsOperator_ResourceCountsAreDifferent_ReturnsFalse()
    {
      var r1 = new ResourceClutch(1, 2, 3, 4, 5);
      var r2 = new ResourceClutch(1, 2, 3, 4, 0);

      (r1 == r2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("ResourceClutch")]
    public void NotEqualsOperator_ResourceCountsAreDifferent_ReturnsTrue()
    {
      var r1 = new ResourceClutch(1, 2, 3, 4, 5);
      var r2 = new ResourceClutch(1, 2, 3, 4, 0);

      (r1 != r2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("ResourceClutch")]
    public void EqualsOperator_ResourceCountsAreSame_ReturnsTrue()
    {
      var r1 = new ResourceClutch(1, 2, 3, 4, 5);
      var r2 = new ResourceClutch(1, 2, 3, 4, 5);

      (r1 == r2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("ResourceClutch")]
    public void NotEqualsOperator_ResourceCountsAreSame_ReturnsFalse()
    {
      var r1 = new ResourceClutch(1, 2, 3, 4, 5);
      var r2 = new ResourceClutch(1, 2, 3, 4, 5);

      (r1 != r2).ShouldBeFalse();
    }
    #endregion
  }
}
