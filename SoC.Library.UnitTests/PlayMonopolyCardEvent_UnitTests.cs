
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("PlayMonopolyCardEvent")]
  public class PlayMonopolyCardEvent_UnitTests
  {
    #region Methods
    [Test]
    public void Equals_ParameterIsNull_ReturnsFalse()
    {
      var parameter = new PlayMonopolyCardEvent(Guid.NewGuid(), null);
      parameter.Equals(null).ShouldBeFalse();
    }

    [Test]
    public void Equals_ParameterIsSameObject_ReturnsTrue()
    {
      var parameter1 = new PlayMonopolyCardEvent(Guid.NewGuid(), null);
      var parameter2 = parameter1;

      parameter1.Equals(parameter2).ShouldBeTrue();
    }

    [Test]
    public void Equals_ParameterIsDifferentObjectWithSamePlayerId_ReturnsTrue()
    {
      var id = Guid.NewGuid();
      var parameter1 = new PlayMonopolyCardEvent(id, null);
      var parameter2 = new PlayMonopolyCardEvent(id, null);

      parameter1.Equals(parameter2).ShouldBeTrue();
    }

    [Test]
    public void Equals_ParameterIsDifferentObjectWithDifferentPlayerId_ReturnsFalse()
    {
      var parameter1 = new PlayMonopolyCardEvent(Guid.NewGuid(), null);
      var parameter2 = new PlayMonopolyCardEvent(Guid.NewGuid(), null);

      parameter1.Equals(parameter2).ShouldBeFalse();
    }

    [Test]
    public void Equals_ParameterIsDifferentType_ReturnsFalse()
    {
      var parameter1 = new PlayMonopolyCardEvent(Guid.NewGuid(), null);
      var parameter2 = new Object();

      parameter1.Equals(parameter2).ShouldBeFalse();
    }

    [Test]
    public void Equals_NoResourcesOnBothEvents_ReturnsTrue()
    {
      var playerId = Guid.NewGuid();
      ((new PlayMonopolyCardEvent(playerId, null)).Equals(new PlayMonopolyCardEvent(playerId, null))).ShouldBeTrue();
    }

    [Test]
    public void Equals_OneResourcesOnOneEvent_ReturnsFalse()
    {
      var playerId = Guid.NewGuid();
      var resourceTransactionList = new ResourceTransactionList();
      resourceTransactionList.Add(new ResourceTransaction(playerId, Guid.NewGuid(), ResourceClutch.OneBrick));
      ((new PlayMonopolyCardEvent(playerId, resourceTransactionList)).Equals(new PlayMonopolyCardEvent(playerId, null))).ShouldBeFalse();
    }

    [Test]
    public void Equals_SameResourcesOnBothEvents_ReturnsTrue()
    {
      var playerId = Guid.NewGuid();
      var resourceTransactionList = new ResourceTransactionList();
      resourceTransactionList.Add(new ResourceTransaction(playerId, Guid.NewGuid(), ResourceClutch.OneBrick));
      ((new PlayMonopolyCardEvent(playerId, resourceTransactionList)).Equals(new PlayMonopolyCardEvent(playerId, resourceTransactionList))).ShouldBeTrue();
    }

    [Test]
    public void Equals_DifferentResourcesOnEachEvent_ReturnsFalse()
    {
      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      var firstResourceTransactionList = new ResourceTransactionList();
      firstResourceTransactionList.Add(new ResourceTransaction(playerId, opponentId, ResourceClutch.OneBrick));

      var secondResourceTransactionList = new ResourceTransactionList();
      secondResourceTransactionList.Add(new ResourceTransaction(playerId, opponentId, ResourceClutch.OneGrain));
      ((new PlayMonopolyCardEvent(playerId, firstResourceTransactionList)).Equals(new PlayMonopolyCardEvent(playerId, secondResourceTransactionList))).ShouldBeFalse();
    }
    #endregion 
  }
}
