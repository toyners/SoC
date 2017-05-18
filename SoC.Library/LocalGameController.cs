
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using GameBoards;
  using Interfaces;

  public class LocalGameController : IGameController
  {
    private enum GamePhases
    {
      Initial,
      WaitingLaunch,
      Quitting,
    }

    #region Fields
    private CancellationToken cancellationToken;
    private CancellationTokenSource cancellationTokenSource;
    private Guid curentPlayerTurnToken;
    private IDiceRoller diceRoller;
    private GameBoardManager gameBoardManager;
    private GamePhases gamePhase;
    private IGameSession gameSession;
    private PlayerBase[] players;
    private Boolean quitting;
    private Task sessionTask;
    #endregion

    public LocalGameController(IDiceRoller diceRoller)
    {
      this.diceRoller = diceRoller;
    }

    public Guid GameId { get; private set; }

    #region Events
    public Action<PlayerBase[]> GameJoinedEvent { get; set; }

    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }

    public Action<ClientAccount> LoggedInEvent { get; set; }

    public Action<Guid> StartInitialTurnEvent { get; set; }
    #endregion

    public void AcceptOffer(Offer offer)
    {
      throw new NotImplementedException();
    }

    public void BuildRoad(Location startingLocation, Location finishingLocation)
    {
      throw new NotImplementedException();
    }

    public DevelopmentCard BuyDevelopmentCard()
    {
      throw new NotImplementedException();
    }

    public void LaunchGame()
    {
      throw new NotImplementedException();
    }

    public ICollection<Offer> MakeOffer(Offer offer)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(Location location)
    {
      throw new NotImplementedException();
    }

    public void Quit()
    {
      this.gamePhase = GamePhases.Quitting;
    }

    public void StartJoiningGame(GameOptions gameOptions)
    {
      if (gameOptions == null)
      {
        gameOptions = new GameOptions { MaxPlayers = 1, MaxAIPlayers = 3 };
      }

      this.players = this.CreatePlayers(gameOptions);
      this.gamePhase = GamePhases.WaitingLaunch;
      this.GameJoinedEvent.Invoke(players);
      //this.RunGame(gameOptions);
    }

    public void StartJoiningGame(GameOptions gameOptions, Guid accountToken)
    {
      throw new NotImplementedException();
    }

    public void StartLogIntoAccount(String username, String password)
    {
      throw new NotImplementedException();
    }

    public ResourceTypes TradeResourcesAtPort(Location location)
    {
      throw new NotImplementedException();
    }

    public ResourceTypes TradeResourcesWithBank()
    {
      throw new NotImplementedException();
    }

    public void UpgradeToCity(Location location)
    {
      throw new NotImplementedException();
    }

    private Guid GetTurnToken()
    {
      return Guid.NewGuid();
    }

    private void RunGame(GameOptions gameOptions)
    {

      this.players = this.CreatePlayers(gameOptions);
      this.GameJoinedEvent.Invoke(players);

      if (this.gamePhase == GamePhases.WaitingLaunch)
      {
        return;
      }

      //this.WaitForGameLaunch();
      
      while (true)
      {
        this.gameBoardManager = new GameBoardManager(BoardSizes.Standard);
        this.InitialBoardSetupEvent.Invoke(this.gameBoardManager.Data);

        this.gameSession = new GameSession();

        this.curentPlayerTurnToken = this.GetTurnToken();
        this.StartInitialTurnEvent.Invoke(this.curentPlayerTurnToken);
      }
    }

    private void WaitForGameLaunch()
    {
      while (this.gamePhase == GamePhases.WaitingLaunch)
      {
        Thread.Sleep(50);
        this.cancellationToken.ThrowIfCancellationRequested();
      }
    }

    private PlayerBase[] CreatePlayers(GameOptions gameOptions)
    {
      var players = new PlayerBase[gameOptions.MaxPlayers + gameOptions.MaxAIPlayers];

      var index = 0;
      while ((gameOptions.MaxPlayers--) > 0)
      {
        players[index++] = new PlayerData();
      }

      while ((gameOptions.MaxAIPlayers--) > 0)
      {
        players[index++] = new PlayerView();
      }

      return players;
    }
  }
}
