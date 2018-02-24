
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  public static class LocalGameControllerTestSetup
  {
    public static void LaunchGameAndCompleteSetup(LocalGameController localGameController)
    {
      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(LocalGameControllerTestCreator.MainSettlementOneLocation, LocalGameControllerTestCreator.MainRoadOneEnd);
      localGameController.CompleteGameSetup(LocalGameControllerTestCreator.MainSettlementTwoLocation, LocalGameControllerTestCreator.MainRoadTwoEnd);
      localGameController.FinalisePlayerTurnOrder();
    }
  }
}
