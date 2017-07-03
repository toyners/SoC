
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

    public Action<GameBoardUpdate> GameSetupUpdateEvent { get; set; }

    public Action<ErrorDetails> ErrorRaisedEvent { get; set; }
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

      this.InitialBoardSetupEvent?.Invoke(this.gameBoardManager.Data);
      this.gamePhase = GamePhases.StartGameSetup;
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

    public Boolean TryJoiningGame()
    {
      return this.TryJoiningGame(null);
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

    public void CompleteLaunchGame()
    {
      throw new NotImplementedException();
    }

    public Boolean StartGameSetup()
    {
      if (this.gamePhase != GamePhases.StartGameSetup)
      {
        return false;
      }

      this.players = SetupOrderCreator.Create(this.players, this.dice);
   
      this.playerIndex = 0;
      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(this.gameBoardManager.Data, 1, null);
      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);
      this.gamePhase = GamePhases.ContinueGameSetup;

      return true;
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

      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(gameBoardData, 1, null);
      this.playerIndex = this.players.Length - 1;
      gameBoardUpdate = this.ContinueSetupForComputerPlayers(gameBoardData, -1, gameBoardUpdate);

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

      var gameBoardData = this.gameBoardManager.Data;
      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(gameBoardData, -1, null);
      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);
    }

    private GameBoardUpdate ContinueSetupForComputerPlayers(GameBoardData gameBoardData, Int32 step, GameBoardUpdate gameBoardUpdate)
    {
      while (this.playerIndex >= 0 && this.playerIndex < this.players.Length)
      {
        var player = this.players[this.playerIndex];
        this.playerIndex += step;

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

    private void TryRaiseRoadPlacingExceptions(GameBoardData.VerificationResults verificationResults, Road road)
    {
      if (verificationResults.Status == GameBoardData.VerificationStatus.RoadIsInvalid)
      {
        var errorDetails = new ErrorDetails("Cannot place road at [" + road.Location1 + ", " + road.Location2 + "]. This is outside of board range (0 - 53).");
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
