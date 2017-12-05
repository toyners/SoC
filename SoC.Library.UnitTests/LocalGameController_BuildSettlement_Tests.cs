
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuildSettlement")]
  public class LocalGameController_BuildSettlement_Tests : LocalGameController_UnitTests
  {
    #region Methods
    [Test]
    public void BuildSettlement_ValidScenario_SettlementBuiltEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(5, 0, 5, 0, 0));

      Boolean settlementBuilt = false;
      localGameController.SettlementBuiltEvent = () => { settlementBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 3);

      // Assert
      settlementBuilt.ShouldBeTrue();
    }
    #endregion 
  }
}
