
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("PlayerWithLargestArmyChangedEvent")]
  public class PlayerWithLargestArmyChangedEvent_UnitTests
  {
    #region Methods
    [Test]
    public void Equals_ParameterIsNull_ReturnsFalse()
    {
      var parameter = new PlayerWithLargestArmyChangedEvent(Guid.NewGuid(), Guid.NewGuid());
      parameter.Equals(null).ShouldBeFalse();
    }

    [Test]
    public void Equals_ParameterIsSameObject_ReturnsTrue()
    {
      var parameter1 = new PlayerWithLargestArmyChangedEvent(Guid.NewGuid(), Guid.NewGuid());
      var parameter2 = parameter1;

      parameter1.Equals(parameter2).ShouldBeTrue();
    }

    [Test]
    public void Equals_ParameterIsDifferentObjectWithSamePlayerIds_ReturnsTrue()
    {
      var previousPlayerId = Guid.NewGuid();
      var newPlayerId = Guid.NewGuid();
      var parameter1 = new PlayerWithLargestArmyChangedEvent(previousPlayerId, newPlayerId);
      var parameter2 = new PlayerWithLargestArmyChangedEvent(previousPlayerId, newPlayerId);

      parameter1.Equals(parameter2).ShouldBeTrue();
    }

    [Test]
    public void Equals_ParameterIsDifferentObjectWithDifferentPreviousPlayerId_ReturnsFalse()
    {
      var newPlayerId = Guid.NewGuid();
      var parameter1 = new PlayerWithLargestArmyChangedEvent(Guid.NewGuid(), newPlayerId);
      var parameter2 = new PlayerWithLargestArmyChangedEvent(Guid.NewGuid(), newPlayerId);

      parameter1.Equals(parameter2).ShouldBeFalse();
    }

    [Test]
    public void Equals_ParameterIsDifferentObjectWithDifferentNewPlayerId_ReturnsFalse()
    {
      var previousPlayerId = Guid.NewGuid();
      var parameter1 = new PlayerWithLargestArmyChangedEvent(previousPlayerId, Guid.NewGuid());
      var parameter2 = new PlayerWithLargestArmyChangedEvent(previousPlayerId, Guid.NewGuid());

      parameter1.Equals(parameter2).ShouldBeFalse();
    }
    #endregion 
  }
}
