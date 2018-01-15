
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using System.Collections.Generic;
  using Enums;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.UseMonopolyCard")]
  public class LocalGameController_UseMonopolyCard_Tests
  {
    #region Methods
    [Test]
    public void UseMonopolyCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseMonopolyCard(new TurnToken(), null, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseMonopolyCard_CardIsNull_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseMonopolyCard(turnToken, null, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Development card parameter is null.");
    }

    [Test]
    public void UseMonopolyCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      localGameController.StartGamePlay();
      localGameController.BuyDevelopmentCard(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseMonopolyCard(turnToken, (MonopolyDevelopmentCard)purchasedDevelopmentCard, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that has been purchased this turn.");
    }

    [Test]
    public void UseMonopolyCard_UseDevelopmentCardMoreThanOnce_MeaningfulErrorIsReceived()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      // Buy the knight cards
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Play one knight card each turn for the next two turns
      localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play the same development card more than once.");
    }

    [Test]
    public void UseMonopolyCard_UseDevelopmentCard_ReceiveAllResourcesOfRequestedTypeFromOpponents()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard);
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;
      var secondOpponent = testInstances.SecondOpponent;
      var thirdOpponent = testInstances.ThirdOpponent;

      player.AddResources(ResourceClutch.DevelopmentCard);
      firstOpponent.AddResources(ResourceClutch.OneOfEach);
      secondOpponent.AddResources(new ResourceClutch(2, 0, 0, 0, 0));
      thirdOpponent.AddResources(new ResourceClutch(0, 1, 1, 1, 1));

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceClutch gainedResources = ResourceClutch.Zero;
      localGameController.ResourcesGainedEvent = (ResourceClutch r) => { gainedResources = r; };

      localGameController.StartGamePlay();

      // Buy the monopoly card
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);

      // Assert
      gainedResources.ShouldBe(ResourceClutch.OneBrick * 3);
      player.ResourcesCount.ShouldBe(6);
      player.BrickCount.ShouldBe(3);
      firstOpponent.ResourcesCount.ShouldBe(7);
      firstOpponent.BrickCount.ShouldBe(0);
      secondOpponent.ResourcesCount.ShouldBe(3);
      secondOpponent.BrickCount.ShouldBe(0);
      thirdOpponent.ResourcesCount.ShouldBe(7);
      thirdOpponent.BrickCount.ShouldBe(0);
    }

    [Test]
    public void Scenario_OpponentUsesMonopolyCardAndGetsResourcesFromPlayer()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard);
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;
      var secondOpponent = testInstances.SecondOpponent;
      var thirdOpponent = testInstances.ThirdOpponent;

      // Clear initial resources
      player.RemoveAllResources();
      firstOpponent.RemoveAllResources();
      thirdOpponent.RemoveAllResources();

      player.AddResources(ResourceClutch.OneBrick);
      firstOpponent.AddResources(ResourceClutch.DevelopmentCard);
      firstOpponent.AddBuyDevelopmentCardChoice(1).EndTurn()
        .AddPlaceMonopolyCardAction(new PlayMonopolyCardAction { MonopolyType = ResourceTypes.Brick }).EndTurn();

      secondOpponent.AddResources(new ResourceClutch(2, 1, 1, 1, 1));

      var turn = 0;
      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      var playerActions = new Dictionary<String, List<GameEvent>>();
      var keys = new List<String>();
      localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) =>
      {
        var key = turn + "-" + g.ToString();
        keys.Add(key);
        playerActions.Add(key, e);
      };

      localGameController.StartGamePlay();
      localGameController.EndTurn(turnToken); // Opponent buys development cards
      localGameController.EndTurn(turnToken); // Opponent plays monopoly cards

      // Assert
      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);
      var lostResourcesByPlayerId = new Dictionary<Guid, ResourceClutch>();
      lostResourcesByPlayerId.Add(player.Id, ResourceClutch.OneBrick);
      lostResourcesByPlayerId.Add(secondOpponent.Id, ResourceClutch.OneBrick * 2);

      var expectedPlayMonopolyCardEvent = new PlayMonopolyCardEvent(firstOpponent.Id, lostResourcesByPlayerId);
      var expectedResourceLostEvent = new ResourceLostEvent(firstOpponent.Id, ResourceClutch.OneOre);

      playerActions.Count.ShouldBe(2);
      keys.Count.ShouldBe(playerActions.Count);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayMonopolyCardEvent);

      player.ResourcesCount.ShouldBe(2);
      firstOpponent.ResourcesCount.ShouldBe(4);
    }

    private void AssertThatPlayerActionsForTurnAreCorrect(List<GameEvent> actualEvents, params GameEvent[] expectedEvents)
    {
      actualEvents.Count.ShouldBe(expectedEvents.Length);
      for (var index = 0; index < actualEvents.Count; index++)
      {
        actualEvents[index].ShouldBe(expectedEvents[index], "Index is " + index);
      }
    }

    private IDevelopmentCardHolder CreateMockCardDevelopmentCardHolder(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      var developmentCardHolder = Substitute.For<IDevelopmentCardHolder>();
      var developmentCards = new Queue<DevelopmentCard>();
      developmentCards.Enqueue(firstDevelopmentCard);
      foreach (var developmentCard in otherDevelopmentCards)
      {
        developmentCards.Enqueue(developmentCard);
      }

      DevelopmentCard card;
      developmentCardHolder
        .TryGetNextCard(out card)
        .Returns(x =>
        {
          if (developmentCards.Count > 0)
          {
            x[0] = developmentCards.Dequeue();
            return true;
          }

          x[0] = null;
          return false;
        });

      developmentCardHolder.HasCards.Returns(x => { return developmentCards.Count > 0; });
      return developmentCardHolder;
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup()
    {
      return this.TestSetup(new DevelopmentCardHolder());
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(IDevelopmentCardHolder developmentCardHolder)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(developmentCardHolder);
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);
      testInstances.Dice.AddSequence(new[] { 8u });
      return testInstances;
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      var developmentCardHolder = this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards);
      return this.TestSetup(developmentCardHolder);
    }
    #endregion 
  }
}
