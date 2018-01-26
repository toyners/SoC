
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using Interfaces;
  using MockGameBoards;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.UseYearOfPlentyCard")]
  public class LocalGameController_UseYearOfPlentyCard_Tests
  {
    #region Methods
    [Test]
    public void UseYearOfPlentyCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetupWithDefaultCardHolder();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseYearOfPlentyCard(new TurnToken(), null, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseYearOfPlentyCard_CardIsNull_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetupWithDefaultCardHolder();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseYearOfPlentyCard(turnToken, null, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Development card parameter is null.");
    }

    [Test]
    public void UseYearOfPlentyCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
      var testInstances = this.TestSetupWithExplictDevelopmentCards(yearOfPlentyCard);
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
      localGameController.UseYearOfPlentyCard(turnToken, (YearOfPlentyDevelopmentCard)purchasedDevelopmentCard, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that has been purchased this turn.");
    }

    [Test]
    public void UseYearOfPlentyCard_UseDevelopmentCardMoreThanOnce_MeaningfulErrorIsReceived()
    {
      // Arrange
      var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
      var testInstances = this.TestSetupWithExplictDevelopmentCards(yearOfPlentyCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u, 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      // Buy the year of plenty card
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Play the same year of plenty card each turn for the next two turns
      localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Brick);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play the same development card more than once.");
    }

    [Test]
    public void UseYearOfPlentyCard_UseDevelopmentCard_ResourcesAreCollected()
    {
      // Arrange
      var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
      var testInstances = this.TestSetupWithExplictGameBoard(yearOfPlentyCard, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;

      testInstances.Dice.AddSequence(new[] { 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList resources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resources = r; };

      localGameController.StartGamePlay();

      // Buy the year of plenty card
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      var expected = new ResourceTransactionList();
      expected.Add(new ResourceTransaction(player.Id, Guid.Empty, ResourceClutch.OneBrick * 2));

      resources.ShouldNotBeNull();
      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(resources, expected);
      player.ResourcesCount.ShouldBe(2);
      player.BrickCount.ShouldBe(2);
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

    private LocalGameControllerTestCreator.TestInstances TestSetupWithDefaultCardHolder()
    {
      return this.TestSetupWithExplicitDevelopmentCardHolder(new DevelopmentCardHolder());
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplicitDevelopmentCardHolder(IDevelopmentCardHolder developmentCardHolder)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(developmentCardHolder);
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);
      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered
      return testInstances;
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplictDevelopmentCards(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      var developmentCardHolder = this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards);
      return this.TestSetupWithExplicitDevelopmentCardHolder(developmentCardHolder);
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplictGameBoard(DevelopmentCard developmentCard, GameBoardData gameBoard)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(
        this.CreateMockCardDevelopmentCardHolder(developmentCard),
        gameBoard);
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered

      return testInstances;
    }
    #endregion 
  }
}
