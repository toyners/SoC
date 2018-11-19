
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Connection_UnitTests
  {
    [Test]
    [Category("All")]
    [Category("Connection")]
    public void Cstr_LocationsAreSame_ThrowMeaningfulException()
    {
      Action action = () => { new Connection(0u, 0u); };

      Should.Throw<ArgumentException>(action).Message.ShouldBe("Locations cannot be the same.");
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void Cstr_ValidRoadParameters_NoExceptionThrown()
    {
      Action action = () => { new Connection(0, 1); };

      action.ShouldNotThrow();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void Equals_OtherObjectIsNotRoad_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = new System.Drawing.Point(0, 1);

      road1.Equals(road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void EqualityOperator_ConnectionsAreSameObject_ReturnTrue()
    {
      var road1 = new Connection(0, 1);
      var road2 = road1;

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void EqualityOperator_OneConnectionIsNull_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = (Connection)null;

      (road1 == road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void EqualityOperator_OtherConnectionIsNull_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = (Connection)null;

      (road2 == road1).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void EqualityOperator_BothConnectionsAreNull_ReturnTrue()
    {
      ((Connection)null == (Connection)null).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void EqualityOperator_LocationsAreSame_ReturnTrue()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(0, 1);

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void EqualityOperator_LocationsAreDifferent_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(1, 2);

      (road1 == road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void EqualityOperator_LocationsAreDiametricallyIdentical_ReturnTrue()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(1, 0);

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void InequalityOperator_LocationsAreSame_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(0, 1);

      (road1 != road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void InequalityOperator_LocationsAreDifferent_ReturnTrue()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(1, 2);

      (road1 != road2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void InequalityOperator_LocationsAreDiametricallyIdentical_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(1, 0);

      (road1 != road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    [TestCase(0u, 1u, 1u, 2u)]
    [TestCase(1u, 0u, 1u, 2u)]
    public void IsConnected_RoadsShareALocation_ReturnTrue(UInt32 location1, UInt32 location2, UInt32 location3, UInt32 location4)
    {
      var road1 = new Connection(location1, location2);
      var road2 = new Connection(location3, location4);

      road1.IsConnected(road2).ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void IsConnected_RoadsDoNotShareALocation_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(2, 3);

      road1.IsConnected(road2).ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("Connection")]
    public void IsConnected_SameRoad_ReturnFalse()
    {
      var road1 = new Connection(0, 1);
      var road2 = new Connection(1, 0);

      road1.IsConnected(road2).ShouldBeFalse();
    }
  }
}
