
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;
  using GameBoards;
  using Interfaces;

  public class LocalGameController : IGameController
  {
    public enum GamePhases
    {
      Initial,
      WaitingLaunch,
      StartGameSetup,
      ChooseResourceFromOpponent,
      ContinueGameSetup,
      CompleteGameSetup,
      FinalisePlayerTurnOrder,
      StartGamePlay,
      SetRobberLocation,
      DropResources,
      Quitting,
      NextStep,
    }

    #region Fields
    private IPlayerPool playerPool;
    private Guid curentPlayerTurnToken;
    private IDice dice;
    private GameBoardManager gameBoardManager;
    private IGameSession gameSession;
    private Int32 playerIndex;
    private IPlayer[] players;
    private Dictionary<Guid, IPlayer> playersById;
    private IPlayer mainPlayer;
    private ResourceUpdate gameSetupResources;
    private Int32 resourcesToDrop;
    private Dictionary<Guid, Int32> robbingChoices;
    #endregion

    public LocalGameController(IDice dice, IPlayerPool computerPlayerFactory, GameBoardManager gameBoardManager)
    {
      this.dice = dice;
      this.playerPool = computerPlayerFactory;
      this.gameBoardManager = gameBoardManager;
      this.GamePhase = GamePhases.Initial;
    }

    #region Properties
    public Guid GameId { get; private set; }
    public GamePhases GamePhase { get; private set; }
    #endregion

    #region Events
    public Action<GameBoardUpdate> BoardUpdatedEvent { get; set; }
    public Action<PlayerDataView[]> GameJoinedEvent { get; set; }
    public Action<PlayerDataView[], GameBoardData> GameLoadedEvent { get; set; }
    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }
    public Action<ClientAccount> LoggedInEvent { get; set; }
    public Action<GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }
    public Action<GameBoardUpdate> GameSetupUpdateEvent { get; set; }
    public Action<ErrorDetails> ErrorRaisedEvent { get; set; }
    public Action<Guid> StartPlayerTurnEvent { get; set; }
    public Action<UInt32> DiceRollEvent { get; set; }
    public Action<ResourceUpdate> ResourcesCollectedEvent { get; set; }
    public Action<ResourceClutch> ResourcesGainedEvent { get; set; }
    public Action<ResourceUpdate> ResourcesLostEvent { get; set; }
    public Action<Int32> RobberEvent { get; set; }
    public Action<Dictionary<Guid, Int32>> RobbingChoicesEvent { get; set; }
    public Action<ResourceUpdate> GameSetupResourcesEvent { get; set; }
    public Action<PlayerDataView[]> TurnOrderFinalisedEvent { get; set; }
    #endregion

    #region Methods
    public void BuildRoad(UInt32 startIndex, UInt32 endIndex)
    {
      throw new NotImplementedException();
    }

    public void ChooseResourceFromOpponent(Guid opponentId, Int32 resourceIndex)
    {
      if (this.GamePhase == GamePhases.NextStep)
      {
        var message = "Cannot call 'ChooseResourceFromOpponent' when 'RobbingChoicesEvent' is not raised.";
        var errorDetails = new ErrorDetails(message);
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (this.GamePhase != GamePhases.ChooseResourceFromOpponent)
      {
        var message = "Cannot call 'ChooseResourceFromOpponent' until 'SetRobberLocation' has completed.";
        var errorDetails = new ErrorDetails(message);
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (!this.robbingChoices.ContainsKey(opponentId))
      {
        var message = "Cannot pick resource card from invalid opponent.";
        var errorDetails = new ErrorDetails(message);
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      var resourceCount = this.robbingChoices[opponentId];
      if (resourceIndex < 0 || resourceIndex >= resourceCount)
      {
        var message = "Cannot pick resource card at position " + resourceIndex + ". Resource card range is 0.." + (resourceCount - 1);
        var errorDetails = new ErrorDetails(message);
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      var player = this.playersById[opponentId];
      var resources = this.CollapsePlayerResourcesToList(player);
      var randomisedResources = this.RandomiseResourceList(resources);
      var gainedResources = this.CreateGainedResources(randomisedResources, resourceIndex);
      
      player.RemoveResources(gainedResources);
      this.mainPlayer.AddResources(gainedResources);

      this.ResourcesGainedEvent?.Invoke(gainedResources);
    }

    public void LaunchGame()
    {
      if (this.GamePhase != GamePhases.WaitingLaunch)
      {
        var errorDetails = new ErrorDetails("Cannot call 'LaunchGame' without joining game.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      this.InitialBoardSetupEvent?.Invoke(this.gameBoardManager.Data);
      this.GamePhase = GamePhases.StartGameSetup;
    }

    /// <summary>
    /// Load the game controller data from stream.
    /// </summary>
    /// <param name="stream">Stream containing game controller data.</param>
    public void Load(Stream stream)
    {
      try
      {
        var loadedPlayers = new List<IPlayer>();
        using (var reader = XmlReader.Create(stream, new XmlReaderSettings { CloseInput = false, IgnoreWhitespace = true, IgnoreComments = true }))
        {
          while (!reader.EOF)
          {
            if (reader.Name == "player" && reader.NodeType == XmlNodeType.Element)
            {
              var player = this.playerPool.CreatePlayer(reader);
              loadedPlayers.Add(player);
            }

            reader.Read();
          }
        }

        if (loadedPlayers.Count > 0)
        {
          this.mainPlayer = loadedPlayers[0];
          this.players = new IPlayer[loadedPlayers.Count];
          this.players[0] = this.mainPlayer;
          this.playersById = new Dictionary<Guid, IPlayer>(this.players.Length);
          this.playersById.Add(this.mainPlayer.Id, this.mainPlayer);

          for (var index = 1; index < loadedPlayers.Count; index++)
          {
            var player = loadedPlayers[index];
            this.players[index] = player;
            this.playersById.Add(player.Id, player);
          }
        }

        this.GameLoadedEvent?.Invoke(this.CreatePlayerDataViews(), this.gameBoardManager.Data);
      }
      catch (Exception e)
      {
        throw new Exception("Exception thrown during board loading.", e);
      }
    }

    public void Quit()
    {
      this.GamePhase = GamePhases.Quitting;
    }

    public void JoinGame()
    {
      this.JoinGame(null);
    }

    public void JoinGame(GameOptions gameOptions)
    {
      if (this.GamePhase != GamePhases.Initial)
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
      this.GamePhase = GamePhases.WaitingLaunch;
    }

    public void Save(String v)
    {
      throw new NotImplementedException();
    }

    public Boolean StartGameSetup()
    {
      if (this.GamePhase != GamePhases.StartGameSetup)
      {
        return false;
      }

      this.players = PlayerTurnOrderCreator.Create(this.players, this.dice);

      this.playerIndex = 0;
      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(this.gameBoardManager.Data);
      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);
      this.GamePhase = GamePhases.ContinueGameSetup;

      return true;
    }

    public void StartGamePlay()
    {
      this.playerIndex = 0;
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
        ResourceUpdate resourcesDroppedByComputerPlayers = null;

        for (var index = 0; index < this.players.Length; index++)
        {
          var player = this.players[index];

          if (!player.IsComputer)
          {
            continue;
          }

          if (player.ResourcesCount > 7)
          {
            var computerPlayer = (IComputerPlayer)player;
            var resourcesToDropByComputerPlayer = computerPlayer.ChooseResourcesToDrop();

            if (resourcesDroppedByComputerPlayers == null)
            {
              resourcesDroppedByComputerPlayers = new ResourceUpdate();
            }

            resourcesDroppedByComputerPlayers.Resources.Add(computerPlayer.Id, resourcesToDropByComputerPlayer);
          }
        }

        this.GamePhase = GamePhases.SetRobberLocation;

        if (this.mainPlayer.ResourcesCount > 7)
        {
          this.resourcesToDrop = this.mainPlayer.ResourcesCount / 2;
          this.GamePhase = GamePhases.DropResources;
        }

        if (resourcesDroppedByComputerPlayers != null)
        {
          this.ResourcesLostEvent?.Invoke(resourcesDroppedByComputerPlayers);
        }

        this.RobberEvent?.Invoke(this.resourcesToDrop);
        return;
      }
    }

    public void ContinueGameSetup(UInt32 settlementLocation, Road road)
    {
      if (this.GamePhase != GamePhases.ContinueGameSetup)
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
      this.GamePhase = GamePhases.CompleteGameSetup;
    }

    public void CompleteGameSetup(UInt32 settlementLocation, Road road)
    {
      if (this.GamePhase != GamePhases.CompleteGameSetup)
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
      this.GamePhase = GamePhases.FinalisePlayerTurnOrder;
    }

    public void DropResources(ResourceClutch resourceClutch)
    {
      this.mainPlayer.RemoveResources(resourceClutch);
    }

    public void FinalisePlayerTurnOrder()
    {
      if (this.GamePhase != GamePhases.FinalisePlayerTurnOrder)
      {
        var errorDetails = new ErrorDetails("Cannot call 'FinalisePlayerTurnOrder' until 'CompleteGameSetup' has completed.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      // Set the order for the main game loop
      this.players = PlayerTurnOrderCreator.Create(this.players, this.dice);
      var playerData = this.CreatePlayerDataViews();
      this.TurnOrderFinalisedEvent?.Invoke(playerData);
      this.GamePhase = GamePhases.StartGamePlay;
    }

    public void SetRobberLocation(UInt32 location)
    {
      if (this.GamePhase != GamePhases.SetRobberLocation)
      {
        var resourceDropErrorDetails = new ErrorDetails(String.Format("Cannot set robber location until expected resources ({0}) have been dropped via call to DropResources method.", this.resourcesToDrop));
        this.ErrorRaisedEvent?.Invoke(resourceDropErrorDetails);
        return;
      }

      var playerIds = this.gameBoardManager.Data.GetPlayersForHex(location);
      if (this.PlayerIdsIsEmptyOrOnlyContainsMainPlayer(playerIds))
      {
        this.GamePhase = GamePhases.NextStep;
        this.RobbingChoicesEvent?.Invoke(null);
        return;
      }

      this.robbingChoices = new Dictionary<Guid, Int32>();
      foreach(var playerId in playerIds)
      {
        this.robbingChoices.Add(playerId, this.playersById[playerId].ResourcesCount);
      }

      this.GamePhase = GamePhases.ChooseResourceFromOpponent;
      this.RobbingChoicesEvent?.Invoke(this.robbingChoices);
    }

    private void AddResourcesToList(List<ResourceTypes> resources, ResourceTypes resourceType, Int32 total)
    {
      for (var i = 0; i < total; i++)
      {
        resources.Add(resourceType);
      }
    }

    private List<ResourceTypes> CollapsePlayerResourcesToList(IPlayer player)
    {
      var resources = new List<ResourceTypes>(player.ResourcesCount);
      foreach (ResourceTypes resourceType in Enum.GetValues(typeof(ResourceTypes)))
      {
        switch (resourceType)
        {
          case ResourceTypes.Brick: this.AddResourcesToList(resources, ResourceTypes.Brick, player.BrickCount); break;
          case ResourceTypes.Grain: this.AddResourcesToList(resources, ResourceTypes.Grain, player.GrainCount); break;
          case ResourceTypes.Lumber: this.AddResourcesToList(resources, ResourceTypes.Lumber, player.LumberCount); break;
          case ResourceTypes.Ore: this.AddResourcesToList(resources, ResourceTypes.Ore, player.OreCount); break;
          case ResourceTypes.Wool: this.AddResourcesToList(resources, ResourceTypes.Wool, player.WoolCount); break;
        }
      }

      return resources;
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

      var resources = this.gameBoardManager.Data.GetResourcesForLocation(settlementLocation);
      this.gameSetupResources.Resources.Add(playerId, resources);
    }

    private GameBoardUpdate ContinueSetupForComputerPlayers(GameBoardData gameBoardData)
    {
      GameBoardUpdate gameBoardUpdate = null;

      while (this.playerIndex < this.players.Length)
      {
        var player = this.players[this.playerIndex++];

        if (!player.IsComputer)
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

        if (!player.IsComputer)
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

    private ResourceClutch CreateGainedResources(List<ResourceTypes> resources, Int32 resourceIndex)
    {
      var gainedResources = ResourceClutch.Zero;
      var chosenResourceType = resources[resourceIndex];

      switch (chosenResourceType)
      {
        case ResourceTypes.Brick: gainedResources.BrickCount = 1; break;
        case ResourceTypes.Grain: gainedResources.GrainCount = 1; break;
        case ResourceTypes.Lumber: gainedResources.LumberCount = 1; break;
        case ResourceTypes.Ore: gainedResources.OreCount = 1; break;
        case ResourceTypes.Wool: gainedResources.WoolCount = 1; break;
      }

      return gainedResources;
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
      this.mainPlayer = this.playerPool.Create();
      this.players = new IPlayer[gameOptions.MaxAIPlayers + 1];
      this.players[0] = this.mainPlayer;
      this.playersById = new Dictionary<Guid, IPlayer>(this.players.Length);
      this.playersById.Add(this.mainPlayer.Id, this.mainPlayer);

      var index = 1;
      while ((gameOptions.MaxAIPlayers--) > 0)
      {
        var player = this.playerPool.Create();
        this.players[index] = player;
        this.playersById.Add(player.Id, player);
        index++;
      }
    }

    private Boolean IsComputerPlayer(IPlayer player)
    {
      return player is IComputerPlayer;
    }

    private Boolean PlayerIdsIsEmptyOrOnlyContainsMainPlayer(Guid[] playerIds)
    {
      return playerIds == null || playerIds.Length == 0 ||
             (playerIds.Length == 1 && playerIds[0] == this.mainPlayer.Id);
    }

    private List<ResourceTypes> RandomiseResourceList(List<ResourceTypes> resources)
    {
      var randomisedResources = new List<ResourceTypes>(resources.Count);
      Random random = new Random();
      while (resources.Count > 0)
      {
        var index = random.Next(resources.Count - 1);
        randomisedResources.Add(resources[index]);
        resources.RemoveAt(index);
      }

      return randomisedResources;
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
