
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class Road_UnitTests
  {
    [Test]
    public void Cstr_LocationsAreSame_ThrowMeaningfulException()
    {
      Action action = () => { new Road(0u, 0u); };

      Should.Throw<ArgumentException>(action).Message.ShouldBe("Locations cannot be the same");
    }

    [Test]
    public void Equals_LocationsAreSame_ReturnTrue()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(0, 1);

      (road1 == road2).ShouldBeTrue();
    }

    [Test]
    public void Equals_LocationsAreDifferent_ReturnFalse()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 2);

      (road1 == road2).ShouldBeFalse();
    }

    [Test]
    public void NotEquals_LocationsAreDifferent_ReturnTrue()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 2);

      (road1 != road2).ShouldBeTrue();
    }

    [Test]
    public void NotEquals_LocationsAreDiametricallyIdentical_ReturnTrue()
    {
      var road1 = new Road(0, 1);
      var road2 = new Road(1, 0);

      (road1 == road2).ShouldBeTrue();
    }
  }
}
