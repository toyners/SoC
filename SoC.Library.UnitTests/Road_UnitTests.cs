
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
  }
}
