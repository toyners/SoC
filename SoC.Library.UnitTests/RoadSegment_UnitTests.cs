
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class RoadSegment_UnitTests
  {
    [Test]
    [Category("RoadSegment")]
    public void Cstr_LocationsAreSame_ThrowMeaningfulException()
    {
      Action action = () => { new RoadSegment(0u, 0u); };

      Should.Throw<ArgumentException>(action).Message.ShouldBe("Locations cannot be the same.");
    }

    public void Cstr_ValidRoadParameters_NoExceptionThrown()
    {
      Action action = () => { new RoadSegment(0, 1); };

      action.ShouldNotThrow();
    }

    [Test]
    [Category("RoadSegment")]
    public void Equals_OtherObjectIsNotRoad_ReturnFalse()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new System.Drawing.Point(0, 1);

      road1.Equals(road2).ShouldBeFalse();
    }

    [Test]
    [Category("RoadSegment")]
    public void EqualityOperator_LocationsAreSame_ReturnTrue()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(0, 1);

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    [Category("RoadSegment")]
    public void EqualityOperator_LocationsAreDifferent_ReturnFalse()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(1, 2);

      (road1 == road2).ShouldBeFalse();
    }

    [Test]
    [Category("RoadSegment")]
    public void EqualityOperator_LocationsAreDiametricallyIdentical_ReturnTrue()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(1, 0);

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    [Category("RoadSegment")]
    public void InequalityOperator_LocationsAreSame_ReturnFalse()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(0, 1);

      (road1 != road2).ShouldBeFalse();
    }

    [Test]
    [Category("RoadSegment")]
    public void InequalityOperator_LocationsAreDifferent_ReturnTrue()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(1, 2);

      (road1 != road2).ShouldBeTrue();
    }

    [Test]
    [Category("RoadSegment")]
    public void InequalityOperator_LocationsAreDiametricallyIdentical_ReturnFalse()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(1, 0);

      (road1 != road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("RoadSegment")]
    [TestCase(0u, 1u, 1u, 2u)]
    [TestCase(1u, 0u, 1u, 2u)]
    public void IsConnected_RoadsShareALocation_ReturnTrue(UInt32 location1, UInt32 location2, UInt32 location3, UInt32 location4)
    {
      var road1 = new RoadSegment(location1, location2);
      var road2 = new RoadSegment(location3, location4);

      road1.IsConnected(road2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("RoadSegment")]
    public void IsConnected_RoadsDoNotShareALocation_ReturnFalse()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(2, 3);

      road1.IsConnected(road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("RoadSegment")]
    public void IsConnected_SameRoad_ReturnFalse()
    {
      var road1 = new RoadSegment(0, 1);
      var road2 = new RoadSegment(1, 0);

      road1.IsConnected(road2).ShouldBeFalse();
    }
  }
}
