
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
      StartGameSetup,
      ContinueGameSetup,
      CompleteGameSetup,
      FinalisePlayerTurnOrder,
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
    private Int32 playerIndex;
    private IPlayer[] players;
    private Player mainPlayer;
    private Boolean quitting;
    private Task sessionTask;
    private ResourceUpdate gameSetupResources;
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

    public Action<PlayerDataView[]> GameJoinedEvent { get; set; }

    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }

    public Action<ClientAccount> LoggedInEvent { get; set; }

    public Action<GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }

    public Action<GameBoardUpdate> GameSetupUpdateEvent { get; set; }

    public Action<ErrorDetails> ErrorRaisedEvent { get; set; }

    public Action<Guid> StartPlayerTurnEvent { get; set; }

    public Action<UInt32> DiceRollEvent { get; set; }

    public Action<ResourceUpdate> ResourcesCollectedEvent { get; set; }
    public Action<PlayerDataView[]> ResourcesLostEvent { get; set; }
    public Action<ResourceUpdate> GameSetupResourcesEvent { get; set; }

    public Action<PlayerDataView[]> TurnOrderFinalisedEvent { get; set; }
    
    #endregion

    #region Methods
    public void LaunchGame()
    {
      if (this.gamePhase != GamePhases.WaitingLaunch)
      {
        var errorDetails = new ErrorDetails("Cannot call 'LaunchGame' without joining game.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      this.InitialBoardSetupEvent?.Invoke(this.gameBoardManager.Data);
      this.gamePhase = GamePhases.StartGameSetup;
    }

    public void Quit()
    {
      this.gamePhase = GamePhases.Quitting;
    }

    public void JoinGame()
    {
      this.JoinGame(null);
    }

    public void JoinGame(GameOptions gameOptions)
    {
      if (this.gamePhase != GamePhases.Initial)
      {
        var errorDetails = new ErrorDetails("Cannot call 'JoinGame' more than once.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (gameOptions == null)
      {
        gameOptions = new GameOptions { MaxPlayers = 1, MaxAIPlayers = 3 };
      }

      this.CreatePlayers(gameOptions);
      var playerData = this.CreatePlayerDataViews();
      this.GameJoinedEvent?.Invoke(playerData);
      this.gamePhase = GamePhases.WaitingLaunch;
    }

    private PlayerDataView[] CreatePlayerDataViews()
    {
      var playerDataViews = new PlayerDataView[this.players.Length];

      for (var index = 0; index < playerDataViews.Length; index++)
      {
        playerDataViews[index] = this.players[index].GetDataView();
      }

      return playerDataViews;
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

    public Boolean StartGameSetup()
    {
      if (this.gamePhase != GamePhases.StartGameSetup)
      {
        return false;
      }

      this.players = PlayerTurnOrderCreator.Create(this.players, this.dice);
   
      this.playerIndex = 0;
      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(this.gameBoardManager.Data);
      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);
      this.gamePhase = GamePhases.ContinueGameSetup;

      return true;
    }

    public void StartGamePlay()
    {
      var turnToken = Guid.NewGuid();
      this.StartPlayerTurnEvent?.Invoke(turnToken);

      var resourceRoll = this.dice.RollTwoDice();
      this.DiceRollEvent?.Invoke(resourceRoll);

      if (resourceRoll != 7)
      {
        var turnResources = this.CollectTurnResources(resourceRoll);
        this.ResourcesCollectedEvent?.Invoke(turnResources);
      }
      else
      {
        throw new NotImplementedException();
      }
    }

    public void ContinueGameSetup(UInt32 settlementLocation, Road road)
    {
      if (this.gamePhase != GamePhases.ContinueGameSetup)
      {
        var errorDetails = new ErrorDetails("Cannot call 'ContinueGameSetup' until 'StartGameSetup' has completed.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (!this.VerifyStartingInfrastructurePlacementRequest(settlementLocation, road))
      {
        return;
      }

      var gameBoardData = this.gameBoardManager.Data;
      gameBoardData.PlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, road);

      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(gameBoardData);

      this.playerIndex = this.players.Length - 1;
      gameBoardUpdate = this.CompleteSetupForComputerPlayers(gameBoardData, gameBoardUpdate);

      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);
      this.gamePhase = GamePhases.CompleteGameSetup;
    }

    public void CompleteGameSetup(UInt32 settlementLocation, Road road)
    {
      if (this.gamePhase != GamePhases.CompleteGameSetup)
      {
        var errorDetails = new ErrorDetails("Cannot call 'CompleteGameSetup' until 'ContinueGameSetup' has completed.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (!this.VerifyStartingInfrastructurePlacementRequest(settlementLocation, road))
      {
        return;
      }

      this.gameBoardManager.Data.PlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, road);

      this.CollectInitialResourcesForPlayer(this.mainPlayer.Id, settlementLocation);

      var gameBoardData = this.gameBoardManager.Data;
      GameBoardUpdate gameBoardUpdate = this.CompleteSetupForComputerPlayers(gameBoardData, null);
      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);

      this.GameSetupResourcesEvent?.Invoke(this.gameSetupResources);
      this.gamePhase = GamePhases.FinalisePlayerTurnOrder;
    }

    public void FinalisePlayerTurnOrder()
    {
      if (this.gamePhase != GamePhases.FinalisePlayerTurnOrder)
      {
        var errorDetails = new ErrorDetails("Cannot call 'FinalisePlayerTurnOrder' until 'CompleteGameSetup' has completed.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      // Set the order for the main game loop
      this.players = PlayerTurnOrderCreator.Create(this.players, this.dice);
      var playerData = this.CreatePlayerDataViews();
      this.TurnOrderFinalisedEvent?.Invoke(playerData);
    }

    private ResourceUpdate CollectTurnResources(UInt32 diceRoll)
    {
      var resourceUpdate = new ResourceUpdate();
      resourceUpdate.Resources = this.gameBoardManager.Data.GetResourcesForRoll(diceRoll);
      return resourceUpdate;
    }

    private void CollectInitialResourcesForPlayer(Guid playerId, UInt32 settlementLocation)
    {
      if (this.gameSetupResources == null)
      {
        this.gameSetupResources = new ResourceUpdate();
      }

      var resourceCounts = this.gameBoardManager.Data.GetResourcesForLocation(settlementLocation);
      this.gameSetupResources.Resources.Add(playerId, resourceCounts);
    }

    private GameBoardUpdate ContinueSetupForComputerPlayers(GameBoardData gameBoardData)
    {
      GameBoardUpdate gameBoardUpdate = null;

      while (this.playerIndex < this.players.Length)
      {
        var player = this.players[this.playerIndex++];

        if (!this.IsComputerPlayer(player))
        {
          return gameBoardUpdate;
        }

        if (gameBoardUpdate == null)
        {
          gameBoardUpdate = new GameBoardUpdate
          {
            NewSettlements = new Dictionary<UInt32, Guid>(),
            NewRoads = new Dictionary<Road, Guid>()
          };
        }

        var computerPlayer = (IComputerPlayer)player;
        var chosenSettlementIndex = computerPlayer.ChooseSettlementLocation(gameBoardData);
        gameBoardData.PlaceSettlement(computerPlayer.Id, chosenSettlementIndex);
        gameBoardUpdate.NewSettlements.Add(chosenSettlementIndex, computerPlayer.Id);

        var chosenRoad = computerPlayer.ChooseRoad(gameBoardData);
        gameBoardData.PlaceRoad(computerPlayer.Id, chosenRoad);
        gameBoardUpdate.NewRoads.Add(chosenRoad, computerPlayer.Id);
      }

      return gameBoardUpdate;
    }

    private GameBoardUpdate CompleteSetupForComputerPlayers(GameBoardData gameBoardData, GameBoardUpdate gameBoardUpdate)
    {
      while (this.playerIndex >= 0)
      {
        var player = this.players[this.playerIndex--];

        if (!this.IsComputerPlayer(player))
        {
          return gameBoardUpdate;
        }

        if (gameBoardUpdate == null)
        {
          gameBoardUpdate = new GameBoardUpdate
          {
            NewSettlements = new Dictionary<UInt32, Guid>(),
            NewRoads = new Dictionary<Road, Guid>()
          };
        }

        var computerPlayer = (IComputerPlayer)player;
        var chosenSettlementIndex = computerPlayer.ChooseSettlementLocation(gameBoardData);
        gameBoardData.PlaceSettlement(computerPlayer.Id, chosenSettlementIndex);
        gameBoardUpdate.NewSettlements.Add(chosenSettlementIndex, computerPlayer.Id);

        this.CollectInitialResourcesForPlayer(computerPlayer.Id, chosenSettlementIndex);
        
        var chosenRoad = computerPlayer.ChooseRoad(gameBoardData);
        gameBoardData.PlaceRoad(computerPlayer.Id, chosenRoad);
        gameBoardUpdate.NewRoads.Add(chosenRoad, computerPlayer.Id);
      }

      return gameBoardUpdate;
    }

    private void TryRaiseRoadPlacingExceptions(GameBoardData.VerificationResults verificationResults, Road road)
    {
      if (verificationResults.Status == GameBoardData.VerificationStatus.RoadIsOffBoard)
      {
        var errorDetails = new ErrorDetails("Cannot place road at [" + road.Location1 + ", " + road.Location2 + "]. This is outside of board range (0 - 53).");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.NoDirectConnection)
      {
        var errorDetails = new ErrorDetails("Cannot place road at [" + road.Location1 + ", " + road.Location2 + "]. There is no direct connection between those points.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.NotConnectedToExisting)
      {
        var errorDetails = new ErrorDetails("Cannot place road at [" + road.Location1 + ", " + road.Location2 + "]. No connection to a player owned road or settlement.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }
    }

    private void TryRaiseSettlementPlacingExceptions(GameBoardData.VerificationResults verificationResults, UInt32 settlementLocation)
    {
      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationIsInvalid)
      {
        var errorDetails = new ErrorDetails("Cannot place settlement at [" + settlementLocation + "]. This is outside of board range (0 - 53).");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.TooCloseToSettlement)
      {
        var errorDetails = new ErrorDetails("Cannot place settlement: Too close to player " + verificationResults.PlayerId + " at location " + verificationResults.LocationIndex);
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationIsOccupied)
      {
        var errorDetails = new ErrorDetails("Cannot place settlement: Location " + settlementLocation + " already owned by player " + verificationResults.PlayerId);
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }
    }

    private Boolean VerifyRoadPlacementRequest(Road road)
    {
      var verificationResults = this.gameBoardManager.Data.CanPlaceRoad(this.mainPlayer.Id, road);
      this.TryRaiseRoadPlacingExceptions(verificationResults, road);

      return verificationResults.Status == GameBoardData.VerificationStatus.Valid;
    }

    private Boolean VerifySettlementPlacementRequest(UInt32 settlementLocation)
    {
      var verificationResults = this.gameBoardManager.Data.CanPlaceSettlement(settlementLocation);
      this.TryRaiseSettlementPlacingExceptions(verificationResults, settlementLocation);

      return verificationResults.Status == GameBoardData.VerificationStatus.Valid;
    }

    private Boolean VerifyStartingInfrastructurePlacementRequest(UInt32 settlementLocation, Road road)
    {
      var verificationResults = this.gameBoardManager.Data.CanPlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, road);
      this.TryRaiseSettlementPlacingExceptions(verificationResults, settlementLocation);
      this.TryRaiseRoadPlacingExceptions(verificationResults, road);
      
      return verificationResults.Status == GameBoardData.VerificationStatus.Valid;
    }
    #endregion
  }
}
