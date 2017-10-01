
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Road_UnitTests
  {
    [Test]
    [Category("Road")]
    public void Cstr_LocationsAreSame_ThrowMeaningfulException()
    {
      Action action = () => { new Road(0u, 0u); };

      Should.Throw<ArgumentException>(action).Message.ShouldBe("Locations cannot be the same.");
    }

    public void Cstr_ValidRoadParameters_NoExceptionThrown()
    {
      Action action = () => { new Road(0, 1); };

      action.ShouldNotThrow();
    }

    [Test]
    [Category("Road")]
    public void Equals_OtherObjectIsNotRoad_ReturnFalse()
    {
      var road1 = new Road(0, 1);
      var road2 = new System.Drawing.Point(0, 1);

      road1.Equals(road2).ShouldBeFalse();
    }

    [Test]
    [Category("Road")]
    public void EqualityOperator_LocationsAreSame_ReturnTrue()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(0, 1);

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    [Category("Road")]
    public void EqualityOperator_LocationsAreDifferent_ReturnFalse()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 2);

      (road1 == road2).ShouldBeFalse();
    }

    [Test]
    [Category("Road")]
    public void EqualityOperator_LocationsAreDiametricallyIdentical_ReturnTrue()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 0);

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    [Category("Road")]
    public void InequalityOperator_LocationsAreSame_ReturnFalse()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(0, 1);

      (road1 != road2).ShouldBeFalse();
    }

    [Test]
    [Category("Road")]
    public void InequalityOperator_LocationsAreDifferent_ReturnTrue()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 2);

      (road1 != road2).ShouldBeTrue();
    }

    [Test]
    [Category("Road")]
    public void InequalityOperator_LocationsAreDiametricallyIdentical_ReturnFalse()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 0);

      (road1 != road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Road")]
    public void IsConnected_RoadsShareALocation_ReturnTrue()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 0);

      road1.IsConnected(road2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("Road")]
    public void IsConnected_RoadsDoNotShareALocation_ReturnFalse()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(2, 3);

      road1.IsConnected(road2).ShouldBeFalse();
    }
  }
}
