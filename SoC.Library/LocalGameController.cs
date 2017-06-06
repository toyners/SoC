
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
    private IComputerPlayerFactory computerPlayerFactory;
    private Guid curentPlayerTurnToken;
    private IDice dice;
    private GameBoardManager gameBoardManager;
    private GamePhases gamePhase;
    private IGameSession gameSession;
    private UInt32 playerIndex;
    private IPlayer[] players;
    private Player mainPlayer;
    private Boolean quitting;
    private Task sessionTask;
    #endregion

    public LocalGameController(IDice dice, IComputerPlayerFactory computerPlayerFactory, GameBoardManager gameBoardManager)
    {
      this.dice = dice;
      this.computerPlayerFactory = computerPlayerFactory;
      this.gameBoardManager = gameBoardManager;
      this.gamePhase = GamePhases.Initial;
    }

    public Guid GameId { get; private set; }

    #region Events
    public Action<GameBoardUpdate> BoardUpdatedEvent { get; set; }

    public Action<PlayerDataBase[]> GameJoinedEvent { get; set; }

    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }

    public Action<ClientAccount> LoggedInEvent { get; set; }

    public Action<GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }
    #endregion

    #region Methods
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

    public Boolean TryLaunchGame()
    {
      if (this.gamePhase != GamePhases.WaitingLaunch)
      {
        return false;
      }

      var gameBoardData = this.gameBoardManager.Data;
      this.InitialBoardSetupEvent?.Invoke(gameBoardData);

      this.players = SetupOrderCreator.Create(this.players, this.dice);
      GameBoardUpdate gameBoardUpdate = null;
      this.playerIndex = 0;
      for (; this.playerIndex < this.players.Length; this.playerIndex++)
      {
        var player = this.players[this.playerIndex];
        if (!this.IsComputerPlayer(player))
        {
          this.StartInitialSetupTurnEvent?.Invoke(gameBoardUpdate);
          break;
        }

        if (gameBoardUpdate == null)
        {
          gameBoardUpdate = new GameBoardUpdate
          {
            NewSettlements = new Dictionary<UInt32, Guid>(),
            NewRoads = new Dictionary<Trail, Guid>()
          };
        }

        var computerPlayer = (IComputerPlayer)player;
        var chosenSettlementIndex = computerPlayer.ChooseSettlementLocation(gameBoardData);
        gameBoardData.PlaceStartingSettlement(computerPlayer.Id, chosenSettlementIndex);
        var chosenRoad = computerPlayer.ChooseRoad(gameBoardData);

        gameBoardUpdate.NewSettlements.Add(chosenSettlementIndex, player.Id);
        gameBoardUpdate.NewRoads.Add(chosenRoad, player.Id);
      }

      return true;
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

    public Boolean TryJoiningGame(GameOptions gameOptions)
    {
      if (this.gamePhase != GamePhases.Initial)
      {
        return false;
      }

      if (gameOptions == null)
      {
        gameOptions = new GameOptions { MaxPlayers = 1, MaxAIPlayers = 3 };
      }

      this.CreatePlayers(gameOptions);
      var playerData = this.CreateDataFromPlayers();
      this.GameJoinedEvent?.Invoke(playerData);
      this.gamePhase = GamePhases.WaitingLaunch;

      return true;
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

    private PlayerDataBase[] CreateDataFromPlayers()
    {
      var playerData = new PlayerDataBase[this.players.Length];
      playerData[0] = new PlayerData();

      for (var index = 1; index < playerData.Length; index++)
      {
        playerData[index] = new PlayerDataView();
      }

      return playerData;
    }

    private void CreatePlayers(GameOptions gameOptions)
    {
      this.mainPlayer = new Player();
      this.players = new IPlayer[gameOptions.MaxAIPlayers + 1];
      this.players[0] = this.mainPlayer;
      var index = 1;
      while ((gameOptions.MaxAIPlayers--) > 0)
      {
        this.players[index] = this.computerPlayerFactory.Create();
        index++;
      }
    }

    private Guid GetTurnToken()
    {
      return Guid.NewGuid();
    }

    private Boolean IsComputerPlayer(IPlayer player)
    {
      return player is IComputerPlayer;
    }

    private void WaitForGameLaunch()
    {
      while (this.gamePhase == GamePhases.WaitingLaunch)
      {
        Thread.Sleep(50);
        this.cancellationToken.ThrowIfCancellationRequested();
      }
    }
    #endregion
  }
}
