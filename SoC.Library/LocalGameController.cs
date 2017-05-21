
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
    private PlayerBase[] players;
    private Boolean quitting;
    private Task sessionTask;
    #endregion

    public LocalGameController(IDice dice, IComputerPlayerFactory computerPlayerFactory)
    {
      this.dice = dice;
      this.computerPlayerFactory = computerPlayerFactory;
    }

    public Guid GameId { get; private set; }

    #region Events
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

      this.gameBoardManager = new GameBoardManager(BoardSizes.Standard);
      var gameBoardData = this.gameBoardManager.Data;
      for (Int32 i = 0; i < this.players.Length; i++)
      {
        gameBoardData.Settlements.Add(this.players[i].Id, null);
        gameBoardData.Roads.Add(this.players[i].Id, null);
      }

      this.InitialBoardSetupEvent?.Invoke(gameBoardData);

      this.players = SetupOrderCreator.Create(this.players, this.dice);
      //this.StartInitialTurnEvent.Invoke();

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

      this.players = this.CreatePlayers(gameOptions);
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

    private void RunGame(GameOptions gameOptions)
    {

      /*this.players = this.CreatePlayers(gameOptions);
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
      }*/
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

    public UInt32[] GetInitialSetupRoundOrder()
    {
      // Roll dice for each player
      var rollsByPlayer = new Dictionary<UInt32, UInt32>();
      var rolls = new List<UInt32>(this.players.Length);
      UInt32 index = 0;
      for (; index < this.players.Length; index++)
      {
        UInt32 roll = this.dice.RollTwoDice();
        while (rolls.Contains(roll))
        {
          roll = this.dice.RollTwoDice();
        }

        rollsByPlayer.Add(roll, index);
        rolls.Add(roll);
      }

      // Reverse sort the rolls
      rolls.Sort((x, y) => { if (x < y) return 1; if (x > y) return -1; return 0; });

      // Produce order based on descending dice roll order
      UInt32[] setupPassOrder = new UInt32[this.players.Length];
      index = 0;
      foreach (var roll in rolls)
      {
        setupPassOrder[index++] = rollsByPlayer[roll];
      }

      return setupPassOrder;
    }
    #endregion
  }
}
