
namespace Jabberwocky.SoC.Library
{
  using System;
  using Enums;
  using Interfaces;

  public class GameControllerFactory
  {
    private IDiceRollerFactory diceRollerFactory;

    public GameControllerFactory(IDiceRollerFactory diceRollerFactory)
    {
      this.diceRollerFactory = diceRollerFactory;
    }

    public GameControllerFactory() : this(new DiceRollerFactory()) { }

    public IGameController Create(GameOptions gameOptions, GameControllerSetup gameControllerSetup)
    {
      this.VerifyControllerSetup(gameControllerSetup);

      if (gameOptions == null || gameOptions.Connection == GameConnectionTypes.Local)
      {
        return new LocalGameController(this.diceRollerFactory.Create());
      }

      throw new NotImplementedException();
    }

    private void VerifyControllerSetup(GameControllerSetup gameControllerSetup)
    {
      if (gameControllerSetup == null)
      {
        throw new ArgumentNullException("Parameter 'gameControllerSetup' is null.", (Exception)null);
      }

      String missingEventHandlers = String.Empty;

      if (gameControllerSetup.GameJoinedEventHandler == null)
      {
        missingEventHandlers = this.AddToMissingEventHandlers(missingEventHandlers, "GameJoinedEventHandler");
      }

      if (gameControllerSetup.InitialBoardSetupEventHandler == null)
      {
        missingEventHandlers = this.AddToMissingEventHandlers(missingEventHandlers, "InitialBoardSetupEventHandler");
      }

      if (gameControllerSetup.LoggedInEventHandler == null)
      {
        missingEventHandlers = this.AddToMissingEventHandlers(missingEventHandlers, "LoggedInEventHandler");
      }

      if (gameControllerSetup.StartInitialTurnEventHandler == null)
      {
        missingEventHandlers = this.AddToMissingEventHandlers(missingEventHandlers, "StartInitialTurnEventHandler");
      }

      if (missingEventHandlers.Length > 0)
      {
        throw new NullReferenceException("The following Event Handlers are not set: " + missingEventHandlers);
      }
    }

    private String AddToMissingEventHandlers(String missingEventHandlers, String missingEventHandler)
    {
      if (missingEventHandlers.Length > 0)
      {
        missingEventHandlers += ", ";
      }

      return missingEventHandlers + missingEventHandler;
    }
  }

  public class GameControllerSetup
  {
    public Action<PlayerBase[]> GameJoinedEventHandler;

    public Action<GameBoards.GameBoardData> InitialBoardSetupEventHandler { get; set; }

    public Action<ClientAccount> LoggedInEventHandler { get; set; }

    public Action<Guid> StartInitialTurnEventHandler { get; set; }
  }
}
