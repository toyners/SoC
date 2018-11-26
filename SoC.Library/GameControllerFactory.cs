
namespace Jabberwocky.SoC.Library
{
  using System;
  using Enums;
  using Interfaces;
    using Jabberwocky.SoC.Library.PlayerData;

    public class GameControllerFactory
  {
    private IPlayerPool computerPlayerFactory;
    private IDiceFactory diceRollerFactory;

    #region Construction
    public GameControllerFactory(IDiceFactory diceRollerFactory, IPlayerPool computerPlayerFactory)
    {
      this.diceRollerFactory = diceRollerFactory;
      this.computerPlayerFactory = computerPlayerFactory;
    }

    public GameControllerFactory() : this(new DiceFactory(), new PlayerPool()) { }
    #endregion

    #region Methods
    public IGameController Create(GameOptions gameOptions, GameControllerSetup gameControllerSetup)
    {
      this.VerifyControllerSetup(gameControllerSetup);

      IGameController gameController = null;
      if (gameOptions == null || gameOptions.Connection == GameConnectionTypes.Local)
      {
        gameController = new LocalGameController(this.diceRollerFactory.Create(), this.computerPlayerFactory, new GameBoards.GameBoard(BoardSizes.Standard), null);
      }
      else
      {
        throw new NotImplementedException();
      }

      gameController.GameJoinedEvent = gameControllerSetup.GameJoinedEventHandler;
      gameController.InitialBoardSetupEvent = gameControllerSetup.InitialBoardSetupEventHandler;
      gameController.LoggedInEvent = gameControllerSetup.LoggedInEventHandler;
      gameController.StartInitialSetupTurnEvent = gameControllerSetup.StartInitialSetupTurnEvent;

      return gameController;
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

      if (gameControllerSetup.StartInitialSetupTurnEvent == null)
      {
        missingEventHandlers = this.AddToMissingEventHandlers(missingEventHandlers, "StartInitialSetupTurnEvent");
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
    #endregion
  }

  public class GameControllerSetup
  {
    public Action<PlayerDataBase[]> GameJoinedEventHandler;
    public Action<GameBoards.GameBoard> InitialBoardSetupEventHandler;
    public Action<ClientAccount> LoggedInEventHandler;
    public Action<GameBoards.GameBoardUpdate> StartInitialSetupTurnEvent;
  }
}
