﻿
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
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
      errorDetails.Message.ShouldBe("Monopoly development card parameter is null.");
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
    #endregion 
  }
}