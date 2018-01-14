
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using Enums;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
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
      localGameController.UseMonopolyCard(new TurnToken(), null, ResourceKinds.Brick);

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
      localGameController.UseMonopolyCard(turnToken, null, ResourceKinds.Brick);

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
      testInstances.Dice.AddSequence(new[] { 8u });

      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);

      return testInstances;
    }
    #endregion 
  }
}
