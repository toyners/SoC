
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameEvents;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameEvent")]
  public class GameEvent_UnitTests
  {
    #region Methods
    [Test]
    public void Equals_ParameterIsNull_ReturnsFalse()
    {
      var parameter = new GameEvent(Guid.NewGuid());
      parameter.Equals(null).ShouldBeFalse();
    }

    [Test]
    public void Equals_ParameterIsSameObject_ReturnsTrue()
    {
      var parameter1 = new GameEvent(Guid.NewGuid());
      var parameter2 = parameter1;

      parameter1.Equals(parameter2).ShouldBeTrue();
    }

    [Test]
    public void Equals_ParameterIsDifferentObjectWithSamePlayerId_ReturnsTrue()
    {
      var id = Guid.NewGuid();
      var parameter1 = new GameEvent(id);
      var parameter2 = new GameEvent(id);

      parameter1.Equals(parameter2).ShouldBeTrue();
    }
    
    [Test]
    public void Equals_ParameterIsDifferentObjectWithDifferentPlayerId_ReturnsFalse()
    {
      var parameter1 = new GameEvent(Guid.NewGuid());
      var parameter2 = new GameEvent(Guid.NewGuid());

      parameter1.Equals(parameter2).ShouldBeFalse();
    }

    [Test]
    public void Equals_ParameterIsDifferentType_ReturnsFalse()
    {
      var parameter1 = new GameEvent(Guid.NewGuid());
      var parameter2 = new Object();

      parameter1.Equals(parameter2).ShouldBeFalse();
    }
    #endregion 
  }
}
