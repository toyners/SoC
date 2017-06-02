
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
    private Dictionary<PlayerBase, IComputerPlayer> computerPlayers;
    private Guid curentPlayerTurnToken;
    private IDice dice;
    private GameBoardManager gameBoardManager;
    private GamePhases gamePhase;
    private IGameSession gameSession;
    private PlayerBase[] players;
    private PlayerData mainPlayer;
    private Boolean quitting;
    private Task sessionTask;
    #endregion

    public LocalGameController(IDice dice, IComputerPlayerFactory computerPlayerFactory, GameBoardManager gameBoardManager)
    {
      this.dice = dice;
      this.computerPlayerFactory = computerPlayerFactory;
      this.gameBoardManager = gameBoardManager;
    }

    public Guid GameId { get; private set; }

    #region Events
    public Action<GameBoardUpdate> BoardUpdatedEvent { get; set; }

    public Action<PlayerBase[]> GameJoinedEvent { get; set; }

    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }

    public Action<ClientAccount> LoggedInEvent { get; set; }

    public Action<Guid, GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }
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
      for (Int32 i = 0; i < this.players.Length; i++)
      {
        gameBoardData.Roads.Add(this.players[i].Id, null);
      }

      this.InitialBoardSetupEvent?.Invoke(gameBoardData);

      this.players = SetupOrderCreator.Create(this.players, this.dice);

      var mainPlayerHasHadTurn = false;
      GameBoardUpdate gameBoardUpdate = null;
      foreach (var player in this.players)
      {
        if (this.IsComputerPlayer(player))
        {
          if (gameBoardUpdate == null)
          {
            gameBoardUpdate = new GameBoardUpdate
            {
              NewSettlements = new Dictionary<PlayerBase, List<Location>>(),
              NewRoads = new Dictionary<PlayerBase, List<Trail>>()
            };
          }

          var computerPlayer = this.computerPlayers[player];
          var chosenSettlement = computerPlayer.ChooseSettlementLocation(gameBoardData);
          var chosenRoad = computerPlayer.ChooseRoad(gameBoardData);

          gameBoardUpdate.NewSettlements.Add(player, new List<Location> { chosenSettlement });
          gameBoardUpdate.NewRoads.Add(player, new List<Trail> { chosenRoad });
        }
        else
        {
          if (gameBoardUpdate != null)
          {
            this.StartInitialSetupTurnEvent?.Invoke(Guid.Empty, gameBoardUpdate);
            gameBoardUpdate = null;
          }
        }
      }

      /*var firstPlayer = this.players[0];
      if (this.IsComputerPlayer(firstPlayer))
      {
        var computerPlayer = this.computerPlayers[firstPlayer];
        var chosenSettlement = computerPlayer.ChooseSettlementLocation(gameBoardData);
        var chosenRoad = computerPlayer.ChooseRoad(gameBoardData);
        var gameBoardUpdate = new GameBoardUpdate
        {
          NewSettlements = new Dictionary<PlayerBase, List<Location>>
          {
            { firstPlayer, new List<Location> { chosenSettlement } }
          },
          NewRoads = new Dictionary<PlayerBase, List<Trail>>
          {
            { firstPlayer, new List<Trail> { chosenRoad } }
          }
        };
        
        this.StartInitialSetupTurnEvent?.Invoke(this.computerPlayers[firstPlayer].Id, gameBoardUpdate);
      }
      else
      {
        // Human player is first to complete initial setup turn so no outstanding updates
        this.StartInitialSetupTurnEvent?.Invoke(this.players[0].Id, null);
      }*/

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

    public void StartJoiningGame(GameOptions gameOptions)
    {
      if (gameOptions == null)
      {
        gameOptions = new GameOptions { MaxPlayers = 1, MaxAIPlayers = 3 };
      }

      this.CreatePlayers(gameOptions);
      this.gamePhase = GamePhases.WaitingLaunch;
      this.GameJoinedEvent?.Invoke(players);
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

    private Boolean IsComputerPlayer(PlayerBase player)
    {
      return this.computerPlayers.ContainsKey(player);
    }

    private void WaitForGameLaunch()
    {
      while (this.gamePhase == GamePhases.WaitingLaunch)
      {
        Thread.Sleep(50);
        this.cancellationToken.ThrowIfCancellationRequested();
      }
    }

    private void CreatePlayers(GameOptions gameOptions)
    {
      this.mainPlayer = new PlayerData();
      this.computerPlayers = new Dictionary<PlayerBase, IComputerPlayer>();
      this.players = new PlayerBase[gameOptions.MaxAIPlayers + 1];
      this.players[0] = this.mainPlayer;
      var index = 1;
      while ((gameOptions.MaxAIPlayers--) > 0)
      {
        this.players[index] = new PlayerView();
        this.computerPlayers.Add(this.players[index], this.computerPlayerFactory.Create());
        index++;
      }
    }
    #endregion
  }
}
