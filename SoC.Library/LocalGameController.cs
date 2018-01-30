
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;
  using GameBoards;
  using GameEvents;
  using Interfaces;

  public class LocalGameController : IGameController
  {
    #region Enums
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
      SetRobberHex,
      DropResources,
      Quitting,
      NextStep,
    }
    #endregion

    #region Fields
    private IPlayerPool playerPool;
    private Boolean cardPlayedThisTurn;
    private HashSet<DevelopmentCard> cardsPlayed;
    private HashSet<DevelopmentCard> cardsPurchasedThisTurn;
    private INumberGenerator dice;
    private GameBoardData gameBoard;
    private Int32 playerIndex;
    private IPlayer[] players;
    private Dictionary<Guid, IPlayer> playersById;
    private IPlayer[] computerPlayers;
    private IPlayer playerWithLargestArmy;
    private IPlayer mainPlayer;
    private ResourceUpdate gameSetupResources;
    private Int32 resourcesToDrop;
    private UInt32 robberHex;
    private Dictionary<Guid, Int32> robbingChoices;
    private TurnToken currentTurnToken;
    private IPlayer currentPlayer;
    private IDevelopmentCardHolder developmentCardHolder;
    #endregion

    #region Construction
    public LocalGameController(INumberGenerator dice, IPlayerPool computerPlayerFactory, GameBoardData gameBoard, IDevelopmentCardHolder developmentCardHolder)
    {
      this.dice = dice;
      this.playerPool = computerPlayerFactory;
      this.gameBoard = gameBoard;
      this.developmentCardHolder = developmentCardHolder;
      this.GamePhase = GamePhases.Initial;
      this.cardsPlayed = new HashSet<DevelopmentCard>();
      this.cardsPurchasedThisTurn = new HashSet<DevelopmentCard>();
    }
    #endregion

    #region Properties
    public Guid GameId { get; private set; }
    public GamePhases GamePhase { get; private set; }
    #endregion

    #region Events
    public Action<GameBoardUpdate> BoardUpdatedEvent { get; set; }
    public Action CityBuiltEvent { get; set; }
    public Action<DevelopmentCard> DevelopmentCardPurchasedEvent { get; set; }
    public Action<UInt32> DiceRollEvent { get; set; }
    public Action<ErrorDetails> ErrorRaisedEvent { get; set; }
    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }
    public Action<PlayerDataView[]> GameJoinedEvent { get; set; }
    public Action<PlayerDataView[], GameBoardData> GameLoadedEvent { get; set; }
    public Action<ResourceUpdate> GameSetupResourcesEvent { get; set; }
    public Action<GameBoardUpdate> GameSetupUpdateEvent { get; set; }
    public Action<Guid, Guid> LargestArmyEvent { get; set; }
    public Action<ClientAccount> LoggedInEvent { get; set; }
    public Action<Guid> LongestRoadBuiltEvent { get; set; }
    public Action<Guid, List<GameEvent>> OpponentActionsEvent { get; set; }
    public Action<Dictionary<Guid, ResourceCollection[]>> ResourcesCollectedEvent { get; set; }
    public Action<ResourceUpdate> ResourcesLostEvent { get; set; }
    public Action<ResourceTransactionList> ResourcesTransferredEvent { get; set; }
    public Action RoadSegmentBuiltEvent { get; set; }
    public Action<Int32> RobberEvent { get; set; }
    public Action<Dictionary<Guid, Int32>> RobbingChoicesEvent { get; set; }
    public Action SettlementBuiltEvent { get; set; }
    public Action<GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }
    public Action<TurnToken> StartPlayerTurnEvent { get; set; }
    public Action<PlayerDataView[]> TurnOrderFinalisedEvent { get; set; }
    #endregion

    #region Methods
    public void BuildCity(TurnToken turnToken, UInt32 location)
    {
      if (this.currentTurnToken != turnToken)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token not recognised."));
        return;
      }

      if (!this.VerifyBuildCityRequest(location))
      {
        return;
      }

      this.BuildCity(location);
    }

    public void BuildRoadSegment(TurnToken turnToken, UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      if (this.currentTurnToken != turnToken)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token not recognised."));
        return;
      }

      if (!this.VerifyBuildRoadSegmentRequest(roadStartLocation, roadEndLocation))
      {
        return;
      }

      this.BuildRoadSegment(roadStartLocation, roadEndLocation);
    }

    public void BuildSettlement(TurnToken turnToken, UInt32 location)
    {
      if (turnToken != this.currentTurnToken)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token not recognised."));
        return;
      }

      if (!this.VerifyBuildSettlementRequest(location))
      {
        return;
      }

      this.gameBoard.PlaceSettlement(this.currentPlayer.Id, location);
      this.currentPlayer.PlaceSettlement();
      this.SettlementBuiltEvent?.Invoke();
    }

    public void BuyDevelopmentCard(TurnToken turnToken)
    {
      if (turnToken != this.currentTurnToken)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token not recognised."));
        return;
      }

      if (!this.CanBuyDevelopmentCard())
      {
        this.TryRaiseDevelopmentCardBuyingError();
        return;
      }

      DevelopmentCard developmentCard = this.BuyDevelopmentCard();      
      this.DevelopmentCardPurchasedEvent?.Invoke(developmentCard);
    }

    public void ChooseResourceFromOpponent(Guid opponentId)
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

      var opponent = this.playersById[opponentId];
      var takenResource = this.GetResourceFromPlayer(opponent);
      this.mainPlayer.AddResources(takenResource);

      var resourceTransactionList = new ResourceTransactionList();
      resourceTransactionList.Add(new ResourceTransaction(this.mainPlayer.Id, opponentId, takenResource));
      this.ResourcesTransferredEvent?.Invoke(resourceTransactionList);
    }

    public void CompleteGameSetup(UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      if (this.GamePhase != GamePhases.CompleteGameSetup)
      {
        var errorDetails = new ErrorDetails("Cannot call 'CompleteGameSetup' until 'ContinueGameSetup' has completed.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (!this.VerifyStartingInfrastructurePlacementRequest(settlementLocation, roadEndLocation))
      {
        return;
      }

      this.gameBoard.PlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, roadEndLocation);
      this.mainPlayer.PlaceStartingInfrastructure();
      this.CollectInitialResourcesForPlayer(this.mainPlayer.Id, settlementLocation);
      this.mainPlayer.AddResources(this.gameSetupResources.Resources[this.mainPlayer.Id]);

      GameBoardUpdate gameBoardUpdate = this.CompleteSetupForComputerPlayers(this.gameBoard, null);
      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);

      this.GameSetupResourcesEvent?.Invoke(this.gameSetupResources);
      this.GamePhase = GamePhases.FinalisePlayerTurnOrder;
    }

    public void ContinueGameSetup(UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      if (this.GamePhase != GamePhases.ContinueGameSetup)
      {
        var errorDetails = new ErrorDetails("Cannot call 'ContinueGameSetup' until 'StartGameSetup' has completed.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      if (!this.VerifyStartingInfrastructurePlacementRequest(settlementLocation, roadEndLocation))
      {
        return;
      }

      var gameBoardData = this.gameBoard;
      gameBoardData.PlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, roadEndLocation);
      this.mainPlayer.PlaceStartingInfrastructure();

      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(gameBoardData);

      this.playerIndex = this.players.Length - 1;
      gameBoardUpdate = this.CompleteSetupForComputerPlayers(gameBoardData, gameBoardUpdate);

      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);
      this.GamePhase = GamePhases.CompleteGameSetup;
    }

    public void DropResources(ResourceClutch resourceClutch)
    {
      this.mainPlayer.RemoveResources(resourceClutch);
    }

    public void EndTurn(TurnToken turnToken)
    {
      if (turnToken != this.currentTurnToken)
      {
        return;
      }

      this.EndTurn();
      this.ChangeToNextPlayer();

      while (this.currentPlayer.IsComputer)
      {
        var computerPlayer = this.currentPlayer as IComputerPlayer;
        var events = new List<GameEvent>();
        PlayerAction playerAction;

        while ((playerAction = computerPlayer.GetPlayerAction()) != PlayerAction.EndTurn)
        {
          switch (playerAction)
          {
            case PlayerAction.BuildCity:
            {
              var location = computerPlayer.ChooseCityLocation(this.gameBoard);
              this.BuildCity(location);
              break;
            }

            case PlayerAction.BuildRoad:
            {
              UInt32 startRoadLocation, endRoadLocation;
              computerPlayer.ChooseRoad(this.gameBoard, out startRoadLocation, out endRoadLocation);
              this.BuildRoadSegment(startRoadLocation, endRoadLocation);
              break;
            }

            case PlayerAction.BuyDevelopmentCard:
            {
              var developmentCard = this.BuyDevelopmentCard();
              computerPlayer.AddDevelopmentCard(developmentCard);
              events.Add(new BuyDevelopmentCardEvent(computerPlayer.Id));
              break;
            }

            case PlayerAction.PlayKnightCard:
            {
              var knightCard = computerPlayer.ChooseKnightCard();
              var newRobberHex = computerPlayer.ChooseRobberLocation();
              this.PlayKnightDevelopmentCard(knightCard, newRobberHex);
              events.Add(new PlayKnightCardEvent(computerPlayer.Id));

              var playersOnHex = this.gameBoard.GetPlayersForHex(newRobberHex);
              if (playersOnHex != null)
              {
                var otherPlayers = this.GetPlayersFromIds(playersOnHex);
                var robbedPlayer = computerPlayer.ChoosePlayerToRob(otherPlayers);
                var takenResource = this.GetResourceFromPlayer(robbedPlayer);

                computerPlayer.AddResources(takenResource);
                var resourceTransaction = new ResourceTransaction(computerPlayer.Id, robbedPlayer.Id, takenResource);
                var resourceLostEvent = new ResourceTransactionEvent(computerPlayer.Id, resourceTransaction);
                events.Add(resourceLostEvent);
              }

              var playerWithMostKnightCards = this.DeterminePlayerWithMostKnightCards();
              if (playerWithMostKnightCards == computerPlayer && this.playerWithLargestArmy != computerPlayer)
              {
                var oldPlayerId = (this.playerWithLargestArmy != null ? this.playerWithLargestArmy.Id : Guid.Empty);
                events.Add(new PlayerWithLargestArmyChangedEvent(oldPlayerId, computerPlayer.Id));
                this.playerWithLargestArmy = computerPlayer;
              }

              break;
            }

            case PlayerAction.PlayMonopolyCard:
            {
              var monopolyCard = computerPlayer.ChooseMonopolyCard();
              var resourceType = computerPlayer.ChooseResourceTypeToRob();
              var opponents = this.GetOpponentsForPlayer(computerPlayer);
              var resourceTransations = this.GetAllResourcesFromOpponentsOfType(computerPlayer, opponents, resourceType);
              if (resourceTransations != null)
              {
                this.AddResourcesToCurrentPlayer(computerPlayer, resourceTransations);
              }

              events.Add(new PlayMonopolyCardEvent(computerPlayer.Id, resourceTransations));
              break;
            }

            case PlayerAction.PlayYearOfPlentyCard:
            {
              var yearOfPlentyCard = computerPlayer.ChooseYearOfPlentyCard();
              var resourcesCollected = computerPlayer.ChooseResouresToCollectFromBank();
              computerPlayer.AddResources(resourcesCollected);

              var resourceTransaction = new ResourceTransaction(computerPlayer.Id, this.playerPool.GetBankId(), resourcesCollected);
              var resourceTransactions = new ResourceTransactionList();
              resourceTransactions.Add(resourceTransaction);

              events.Add(new PlayYearOfPlentyCardEvent(computerPlayer.Id, resourceTransactions));
              break;
            }

            default: throw new NotImplementedException("Player action '" + playerAction + "' is not recognised.");
          }
        }

        if (events.Count > 0)
        {
          this.OpponentActionsEvent?.Invoke(computerPlayer.Id, events);
        }

        this.EndTurn();
        this.ChangeToNextPlayer();
      }

      this.currentTurnToken = new TurnToken();
      this.StartPlayerTurnEvent?.Invoke(this.currentTurnToken);

      var resourceRoll = this.dice.RollTwoDice();
      this.DiceRollEvent?.Invoke(resourceRoll);

      var turnResources = this.CollectTurnResources(resourceRoll);
      this.ResourcesCollectedEvent?.Invoke(turnResources);

      foreach (var kv in turnResources)
      {
        var player = this.playersById[kv.Key];
        foreach (var resourceCollection in kv.Value)
        {
          player.AddResources(resourceCollection.Resources);
        }
      }
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

    public void LaunchGame()
    {
      if (this.GamePhase != GamePhases.WaitingLaunch)
      {
        var errorDetails = new ErrorDetails("Cannot call 'LaunchGame' without joining game.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      this.InitialBoardSetupEvent?.Invoke(this.gameBoard);
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

            if (reader.Name == "resources" && reader.NodeType == XmlNodeType.Element)
            {
              this.gameBoard.LoadHexResources(reader);
            }

            if (reader.Name == "production" && reader.NodeType == XmlNodeType.Element)
            {
              this.gameBoard.LoadHexProduction(reader);
            }

            if (reader.Name == "settlements" && reader.NodeType == XmlNodeType.Element)
            {
              this.gameBoard.ClearSettlements();
            }

            if (reader.Name == "settlement" && reader.NodeType == XmlNodeType.Element)
            {
              var playerId = Guid.Parse(reader.GetAttribute("playerid"));
              var location = UInt32.Parse(reader.GetAttribute("location"));

              this.gameBoard.PlaceSettlementOnBoard(playerId, location);
            }

            if (reader.Name == "roads" && reader.NodeType == XmlNodeType.Element)
            {
              this.gameBoard.ClearRoads();
            }

            if (reader.Name == "road" && reader.NodeType == XmlNodeType.Element)
            {
              var playerId = Guid.Parse(reader.GetAttribute("playerid"));
              var start = UInt32.Parse(reader.GetAttribute("start"));
              var end = UInt32.Parse(reader.GetAttribute("end"));

              this.gameBoard.PlaceRoadSegmentOnBoard(playerId, start, end);
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
        else
        {
          this.CreatePlayers(new GameOptions());
        }

        var playerDataViews = this.CreatePlayerDataViews();

        this.GameLoadedEvent?.Invoke(playerDataViews, this.gameBoard);
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

    public void SetRobberHex(UInt32 location)
    {
      if (this.GamePhase != GamePhases.SetRobberHex)
      {
        var resourceDropErrorDetails = new ErrorDetails(String.Format("Cannot set robber location until expected resources ({0}) have been dropped via call to DropResources method.", this.resourcesToDrop));
        this.ErrorRaisedEvent?.Invoke(resourceDropErrorDetails);
        return;
      }

      var playerIds = this.gameBoard.GetPlayersForHex(location);
      if (this.PlayerIdsIsEmptyOrOnlyContainsMainPlayer(playerIds))
      {
        this.GamePhase = GamePhases.NextStep;
        this.RobbingChoicesEvent?.Invoke(null);
        return;
      }

      this.robbingChoices = new Dictionary<Guid, Int32>();
      foreach (var playerId in playerIds)
      {
        this.robbingChoices.Add(playerId, this.playersById[playerId].ResourcesCount);
      }

      this.GamePhase = GamePhases.ChooseResourceFromOpponent;
      this.RobbingChoicesEvent?.Invoke(this.robbingChoices);
    }

    public void StartGamePlay()
    {
      if (this.GamePhase != GamePhases.StartGamePlay)
      {
        var errorDetails = new ErrorDetails("Cannot call 'StartGamePlay' until 'FinalisePlayerTurnOrder' has completed.");
        this.ErrorRaisedEvent?.Invoke(errorDetails);
        return;
      }

      this.playerIndex = 0;
      this.currentPlayer = this.players[this.playerIndex];
      this.currentTurnToken = new TurnToken();
      this.StartPlayerTurnEvent?.Invoke(this.currentTurnToken);

      var resourceRoll = this.dice.RollTwoDice();
      this.DiceRollEvent?.Invoke(resourceRoll);

      if (resourceRoll != 7)
      {
        var turnResources = this.CollectTurnResources(resourceRoll);
        this.ResourcesCollectedEvent?.Invoke(turnResources);

        foreach (var kv in turnResources)
        {
          var player = this.playersById[kv.Key];
          foreach (var resourceCollection in kv.Value)
          {
            player.AddResources(resourceCollection.Resources);
          }
        }
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

        this.GamePhase = GamePhases.SetRobberHex;

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

    public Boolean StartGameSetup()
    {
      if (this.GamePhase != GamePhases.StartGameSetup)
      {
        return false;
      }

      this.players = PlayerTurnOrderCreator.Create(this.players, this.dice);

      this.playerIndex = 0;
      GameBoardUpdate gameBoardUpdate = this.ContinueSetupForComputerPlayers(this.gameBoard);
      this.GameSetupUpdateEvent?.Invoke(gameBoardUpdate);
      this.GamePhase = GamePhases.ContinueGameSetup;

      return true;
    }

    /// <summary>
    /// Trade resources with the bank at a 4-to-1 ratio. Errors will be returned if the transaction cannot be completed.
    /// </summary>
    /// <param name="turnToken">Token of the current turn.</param>
    /// <param name="paymentResourceType">Resource type that the player is giving. Must have at least 4.</param>
    /// <param name="requestedResourceType">Resource type that the player wants to receive.</param>
    public void TradeWithBank(TurnToken turnToken, ResourceTypes paymentResourceType, ResourceTypes requestedResourceType)
    {
      if (turnToken == null)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token is null."));
        return;
      }

      if (turnToken != this.currentTurnToken)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token not recognised."));
        return;
      }
      
      throw new NotImplementedException();
    }

    public void UseKnightCard(TurnToken turnToken, KnightDevelopmentCard developmentCard, UInt32 newRobberHex)
    {
      if (!this.VerifyParametersForUsingDevelopmentCard(turnToken, developmentCard))
      {
        return;
      }

      if (!this.VerifyPlacementOfRobber(newRobberHex))
      {
        return;
      }

      this.PlayKnightDevelopmentCard(developmentCard, newRobberHex);
      this.RaiseLargestArmyEventIfPlayerHasLargestArmy();
    }

    public void UseKnightCard(TurnToken turnToken, KnightDevelopmentCard developmentCard, UInt32 newRobberHex, Guid playerId)
    {
      if (!this.VerifyParametersForUsingDevelopmentCard(turnToken, developmentCard))
      {
        return;
      }

      if (!this.VerifyParametersForResourceTransactionWhenUsingKnightCard(newRobberHex, playerId))
      {
        return;
      }

      this.PlayKnightDevelopmentCard(developmentCard, newRobberHex);
      this.CompleteResourceTransactionBetweenPlayers(this.playersById[playerId]);
      this.RaiseLargestArmyEventIfPlayerHasLargestArmy();
    }

    public void UseMonopolyCard(TurnToken turnToken, MonopolyDevelopmentCard monopolyCard, ResourceTypes resourceType)
    {
      if (!this.VerifyParametersForUsingDevelopmentCard(turnToken, monopolyCard))
      {
        return;
      }

      var opponents = this.GetOpponentsForPlayer(this.mainPlayer);
      var resourceTransactions = this.GetAllResourcesFromOpponentsOfType(this.mainPlayer, opponents, resourceType);
      if (resourceTransactions != null)
      {
        this.AddResourcesToCurrentPlayer(this.mainPlayer, resourceTransactions);
      }

      this.PlayDevelopmentCard(monopolyCard);

      this.ResourcesTransferredEvent?.Invoke(resourceTransactions);
    }

    public void UseYearOfPlentyCard(TurnToken turnToken, YearOfPlentyDevelopmentCard yearOfPlentyCard, ResourceTypes firstChoice, ResourceTypes secondChoice)
    {
      if (!this.VerifyParametersForUsingDevelopmentCard(turnToken, yearOfPlentyCard))
      {
        return;
      }

      var resources = ResourceClutch.Zero;
      foreach (var resourceChoice in new[] { firstChoice, secondChoice })
      { 
        switch (resourceChoice)
        {
          case ResourceTypes.Brick: resources.BrickCount++; break;
          case ResourceTypes.Lumber: resources.LumberCount++; break;
          case ResourceTypes.Grain: resources.GrainCount++; break;
          case ResourceTypes.Ore: resources.OreCount++; break;
          case ResourceTypes.Wool: resources.WoolCount++; break;
        }
      }

      this.mainPlayer.AddResources(resources);

      this.PlayDevelopmentCard(yearOfPlentyCard);

      var resourceTransactions = new ResourceTransactionList();
      resourceTransactions.Add(new ResourceTransaction(this.mainPlayer.Id, this.playerPool.GetBankId(), resources));
      this.ResourcesTransferredEvent?.Invoke(resourceTransactions);
    }

    private void AddResourcesToList(List<ResourceTypes> resources, ResourceTypes resourceType, Int32 total)
    {
      for (var i = 0; i < total; i++)
      {
        resources.Add(resourceType);
      }
    }

    private void AddResourcesToCurrentPlayer(IPlayer player, ResourceTransactionList resourceTransactions)
    {
      for (var i = 0; i < resourceTransactions.Count; i++)
      {
        player.AddResources(resourceTransactions[i].Resources);
      }
    }

    private void AddResourcesToPlayer(ResourceTransactionList resourceTransactions)
    {
      for (var i = 0; i < resourceTransactions.Count; i++)
      {
        var resourceTransaction = resourceTransactions[i];
        var player = this.playersById[resourceTransaction.ReceivingPlayerId];
        player.AddResources(resourceTransaction.Resources);
      }
    }

    private void BuildCity(UInt32 location)
    {
      this.gameBoard.PlaceCity(this.currentPlayer.Id, location);
      this.currentPlayer.PlaceCity();
      this.CityBuiltEvent?.Invoke();
    }

    private void BuildRoadSegment(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      this.gameBoard.PlaceRoadSegment(this.currentPlayer.Id, roadStartLocation, roadEndLocation);
      this.currentPlayer.PlaceRoadSegment();
      this.RoadSegmentBuiltEvent?.Invoke();

      this.RaiseLongestRoadBuiltEventIfRelevant();
    }

    private DevelopmentCard BuyDevelopmentCard()
    {
      DevelopmentCard developmentCard;
      this.developmentCardHolder.TryGetNextCard(out developmentCard);
      this.currentPlayer.PayForDevelopmentCard();
      this.cardsPurchasedThisTurn.Add(developmentCard);
      return developmentCard;
    }

    private Boolean CanBuildCity()
    {
      return this.currentPlayer.GrainCount >= 2 && this.currentPlayer.OreCount >= 3 && this.currentPlayer.RemainingCities > 0;
    }

    private Boolean CanBuildRoadSegment()
    {
      return this.currentPlayer.BrickCount > 0 &&
        this.currentPlayer.LumberCount > 0 &&
        this.currentPlayer.RemainingRoadSegments > 0;
    }

    private Boolean CanBuildSettlement()
    {
      return this.currentPlayer.BrickCount > 0 && this.currentPlayer.LumberCount > 0 &&
          this.currentPlayer.GrainCount > 0 && this.currentPlayer.WoolCount > 0 &&
          this.currentPlayer.RemainingSettlements > 0;
    }

    private Boolean CanBuyDevelopmentCard()
    {
      return this.currentPlayer.GrainCount >= 1 && this.currentPlayer.OreCount >= 1 && this.currentPlayer.WoolCount >= 1
        && this.developmentCardHolder.HasCards;
    }

    private void ChangeToNextPlayer()
    {
      this.playerIndex++;
      if (this.playerIndex == this.players.Length)
      {
        this.playerIndex = 0;
      }

      this.currentPlayer = this.players[this.playerIndex];
    }

    private Dictionary<Guid, ResourceCollection[]> CollectTurnResources(UInt32 diceRoll)
    {
      return this.gameBoard.GetResourcesForRoll(diceRoll);
    }

    private void CollectInitialResourcesForPlayer(Guid playerId, UInt32 settlementLocation)
    {
      if (this.gameSetupResources == null)
      {
        this.gameSetupResources = new ResourceUpdate();
      }

      var resources = this.gameBoard.GetResourcesForLocation(settlementLocation);
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
            NewSettlements = new List<Tuple<UInt32, Guid>>(),
            NewRoads = new List<Tuple<UInt32, UInt32, Guid>>()
          };
        }

        var computerPlayer = (IComputerPlayer)player;
        UInt32 chosenSettlementLocation, chosenRoadSegmentEndLocation;
        computerPlayer.ChooseInitialInfrastructure(gameBoardData, out chosenSettlementLocation, out chosenRoadSegmentEndLocation);
        gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, chosenSettlementLocation, chosenRoadSegmentEndLocation);
        
        gameBoardUpdate.NewSettlements.Add(new Tuple<UInt32, Guid>(chosenSettlementLocation, computerPlayer.Id));
        gameBoardUpdate.NewRoads.Add(new Tuple<UInt32, UInt32, Guid>(chosenSettlementLocation, chosenRoadSegmentEndLocation, computerPlayer.Id));
      }

      return gameBoardUpdate;
    }

    private void CompleteResourceTransactionBetweenPlayers(IPlayer playerToTakeResourceFrom)
    {
      var takenResource = this.GetResourceFromPlayer(playerToTakeResourceFrom);
      this.mainPlayer.AddResources(takenResource);

      var resourceTransactionList = new ResourceTransactionList();
      resourceTransactionList.Add(new ResourceTransaction(this.mainPlayer.Id, playerToTakeResourceFrom.Id, takenResource));

      this.ResourcesTransferredEvent?.Invoke(resourceTransactionList);
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
            NewSettlements = new List<Tuple<UInt32, Guid>>(),
            NewRoads = new List<Tuple<UInt32, UInt32, Guid>>()
          };
        }

        var computerPlayer = (IComputerPlayer)player;
        UInt32 chosenSettlementLocation, chosenRoadSegmentEndLocation;
        computerPlayer.ChooseInitialInfrastructure(gameBoardData, out chosenSettlementLocation, out chosenRoadSegmentEndLocation);
        gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, chosenSettlementLocation, chosenRoadSegmentEndLocation);

        gameBoardUpdate.NewSettlements.Add(new Tuple<UInt32, Guid>(chosenSettlementLocation, computerPlayer.Id));
        gameBoardUpdate.NewRoads.Add(new Tuple<UInt32, UInt32, Guid>(chosenSettlementLocation, chosenRoadSegmentEndLocation, computerPlayer.Id));

        this.CollectInitialResourcesForPlayer(computerPlayer.Id, chosenSettlementLocation);
        computerPlayer.AddResources(this.gameSetupResources.Resources[computerPlayer.Id]);
      }

      return gameBoardUpdate;
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
      this.mainPlayer = this.playerPool.CreatePlayer(true);
      this.players = new IPlayer[gameOptions.MaxAIPlayers + 1];
      this.players[0] = this.mainPlayer;
      this.playersById = new Dictionary<Guid, IPlayer>(this.players.Length);
      this.playersById.Add(this.mainPlayer.Id, this.mainPlayer);
      this.computerPlayers = new IPlayer[gameOptions.MaxAIPlayers];

      var index = 1;
      while ((gameOptions.MaxAIPlayers--) > 0)
      {
        var computerPlayer = this.playerPool.CreatePlayer(false);
        this.players[index] = computerPlayer;
        this.playersById.Add(computerPlayer.Id, computerPlayer);
        this.computerPlayers[index - 1] = computerPlayer;
        index++;
      }
    }

    private IPlayer DeterminePlayerWithMostKnightCards()
    {
      IPlayer playerWithMostKnightCards = null;
      UInt32 workingKnightCardCount = 3;

      foreach (var player in this.players)
      {
        if (player.KnightCards > workingKnightCardCount)
        {
          playerWithMostKnightCards = player;
          workingKnightCardCount = player.KnightCards;
        }
        else if (player.KnightCards == workingKnightCardCount)
        {
          playerWithMostKnightCards = (playerWithMostKnightCards == null ? player : null);
        }
      }

      return playerWithMostKnightCards;
    }

    private void EndTurn()
    {
      this.cardsPurchasedThisTurn.Clear();
      this.cardPlayedThisTurn = false;
    }

    private ResourceTransactionList GetAllResourcesFromOpponentsOfType(IPlayer player, IEnumerable<IPlayer> opponents, ResourceTypes resourceType)
    {
      ResourceTransactionList transactionList = null;
      foreach (var opponent in opponents)
      {
        var resources = opponent.LoseResourcesOfType(resourceType);

        if (resources != ResourceClutch.Zero)
        {
          if (transactionList == null)
          {
            transactionList = new ResourceTransactionList();
          }

          transactionList.Add(new ResourceTransaction(player.Id, opponent.Id, resources));
        }
      }

      return transactionList;
    }

    private IEnumerable<IPlayer> GetOpponentsForPlayer(IPlayer player)
    {
      var opponents = new List<IPlayer>(3);
      foreach (var opponent in this.players)
      {
        if (opponent != player)
        {
          opponents.Add(opponent);
        }
      }

      return opponents;
    }

    private IEnumerable<IPlayer> GetPlayersFromIds(Guid[] playerIds)
    {
      var playerList = new List<IPlayer>();
      foreach (var playerId in playerIds)
      {
        playerList.Add(this.playersById[playerId]);
      }

      return playerList;
    }

    private ResourceClutch GetResourceFromPlayer(IPlayer player)
    {
      var resourceIndex = this.dice.GetRandomNumberBetweenZeroAndMaximum(player.ResourcesCount);
      return player.LoseResourceAtIndex(resourceIndex);
    }

    private void HandlePlaceRoadError(GameBoardData.VerificationStatus status)
    {
      var message = String.Empty;
      switch (status)
      {
        case GameBoardData.VerificationStatus.RoadIsOffBoard: message = "Cannot place road segment because board location is not valid."; break;
        case GameBoardData.VerificationStatus.RoadIsOccupied: message = "Cannot place road segment because road segment already exists."; break;
        case GameBoardData.VerificationStatus.NoDirectConnection: message = "Cannot build road segment because no direct connection between start location and end location."; break;
        case GameBoardData.VerificationStatus.RoadNotConnectedToExistingRoad: message = "Cannot place road segment because it is not connected to an existing road segment."; break;
        default: message = "Road build segment status not recognised: " + status; break;
      }

      this.ErrorRaisedEvent?.Invoke(new ErrorDetails(message));
    }

    private void PlayDevelopmentCard(DevelopmentCard developmentCard)
    {
      this.cardPlayedThisTurn = true;
      this.cardsPlayed.Add(developmentCard);
    }

    private Boolean PlayerIdsIsEmptyOrOnlyContainsMainPlayer(Guid[] playerIds)
    {
      return playerIds == null || playerIds.Length == 0 ||
             (playerIds.Length == 1 && playerIds[0] == this.mainPlayer.Id);
    }

    private void PlayKnightDevelopmentCard(KnightDevelopmentCard developmentCard, UInt32 newRobberHex)
    {
      this.PlayDevelopmentCard(developmentCard);
      this.currentPlayer.PlaceKnightDevelopmentCard();
      this.robberHex = newRobberHex;
    }

    private void RaiseLargestArmyEventIfPlayerHasLargestArmy()
    {
      var playerWithMostKnightCards = this.DeterminePlayerWithMostKnightCards();
      if (playerWithMostKnightCards == this.mainPlayer && this.playerWithLargestArmy != this.mainPlayer)
      {
        var oldPlayerId = (this.playerWithLargestArmy != null ? this.playerWithLargestArmy.Id : Guid.Empty);
        this.LargestArmyEvent?.Invoke(oldPlayerId, playerWithMostKnightCards.Id);
        this.playerWithLargestArmy = playerWithMostKnightCards;
      }
    }

    private void RaiseLongestRoadBuiltEventIfRelevant()
    {
      if (this.currentPlayer.RoadSegmentsBuilt < 5)
      {
        return;
      }

      Guid longestRoadPlayerId = Guid.Empty;
      UInt32[] road = null;
      if (this.gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road) && road.Length > 5)
      {
        this.LongestRoadBuiltEvent?.Invoke(this.currentPlayer.Id);
      }
    }

    private void TryRaiseCityBuildingError()
    {
      if (this.currentPlayer.RemainingCities == 0)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. All cities already built."));
        return;
      }

      if (this.currentPlayer.GrainCount < Constants.GrainForBuildingCity && this.currentPlayer.OreCount < Constants.OreForBuildingCity)
      {
        var missingGrainCount = (Constants.GrainForBuildingCity - this.currentPlayer.GrainCount);
        var missingOreCount = (Constants.OreForBuildingCity - this.currentPlayer.OreCount);
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Missing " + missingGrainCount + " grain and " + missingOreCount + " ore."));
        return;
      }

      if (this.currentPlayer.GrainCount < Constants.GrainForBuildingCity)
      {
        var missingGrainCount = (Constants.GrainForBuildingCity - this.currentPlayer.GrainCount);
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Missing " + missingGrainCount + " grain."));
        return;
      }

      if (this.currentPlayer.OreCount < Constants.OreForBuildingCity)
      {
        var missingOreCount = (Constants.OreForBuildingCity - this.currentPlayer.OreCount);
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Missing " + missingOreCount + " ore."));
        return;
      }
    }

    private void TryRaiseCityPlacingError(GameBoardData.VerificationResults verificationResults, UInt32 location)
    {
      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationForCityIsInvalid)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Location " + location + " is outside of board range (0 - 53)."));
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationIsNotOwned)
      {
        var player = this.playersById[verificationResults.PlayerId];
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Location " + location + " is owned by player '" + player.Name + "'."));
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationIsNotSettled)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. No settlement at location " + location + "."));
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationIsAlreadyCity)
      {
        var player = this.playersById[verificationResults.PlayerId];
        if (player == this.currentPlayer)
        {
          this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. There is already a city at location " + location + " that belongs to you."));
        }
        else
        {
          this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. There is already a city at location " + location + " belonging to '" + player.Name + "'."));
        }

        return;
      }
    }

    private void TryRaiseDevelopmentCardBuyingError()
    {
      if (!this.developmentCardHolder.HasCards)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. No more cards available"));
        return;
      }

      if (this.currentPlayer.GrainCount < 1 && this.currentPlayer.OreCount < 1 && this.currentPlayer.WoolCount < 1)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain and 1 ore and 1 wool."));
        return;
      }

      if (this.currentPlayer.GrainCount < 1 && this.currentPlayer.OreCount < 1)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain and 1 ore."));
        return;
      }

      if (this.currentPlayer.GrainCount < 1 && this.currentPlayer.WoolCount < 1)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain and 1 wool."));
        return;
      }

      if (this.currentPlayer.OreCount < 1 && this.currentPlayer.WoolCount < 1)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 ore and 1 wool."));
        return;
      }

      if (this.currentPlayer.GrainCount < 1)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain."));
        return;
      }

      if (this.currentPlayer.OreCount < 1)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 ore."));
        return;
      }

      if (this.currentPlayer.WoolCount < 1)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 wool."));
        return;
      }
    }

    private void TryRaiseRoadSegmentBuildingError()
    {
      if (this.currentPlayer.RemainingRoadSegments == 0)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. All road segments already built."));
        return;
      }

      if (this.currentPlayer.BrickCount == 0 && this.currentPlayer.LumberCount == 0)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Missing 1 brick and 1 lumber."));
        return;
      }

      if (this.currentPlayer.BrickCount == 0)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Missing 1 brick."));
        return;
      }

      if (this.currentPlayer.LumberCount == 0)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Missing 1 lumber."));
        return;
      }
    }

    private void TryRaiseRoadSegmentPlacingError(GameBoardData.VerificationResults verificationResults, UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      if (verificationResults.Status == GameBoardData.VerificationStatus.RoadIsOffBoard)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Locations " + settlementLocation + " and/or " + roadEndLocation + " are outside of board range (0 - 53)."));
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.NoDirectConnection)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. No direct connection between locations [" + settlementLocation + ", " + roadEndLocation + "]."));
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.RoadIsOccupied)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Road segment between " + settlementLocation + " and " + roadEndLocation + " already exists."));
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.RoadNotConnectedToExistingRoad)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Road segment [" + settlementLocation + ", " + roadEndLocation + "] not connected to existing road segment."));
        return;
      }
    }
    
    private void TryRaiseSettlementBuildingError()
    {
      if (this.ErrorRaisedEvent == null)
      {
        return;
      }

      String message = null;
      if (this.currentPlayer.RemainingSettlements == 0)
      {
        message = "Cannot build settlement. All settlements already built.";
      }
      else
      {
        message = "Cannot build settlement. Missing ";

        if (this.currentPlayer.BrickCount == 0)
        {
          message += "1 brick and ";
        }

        if (this.currentPlayer.GrainCount == 0)
        {
          message += "1 grain and ";
        }

        if (this.currentPlayer.LumberCount == 0)
        {
          message += "1 lumber and ";
        }

        if (this.currentPlayer.WoolCount == 0)
        {
          message += "1 wool and ";
        }

        message = message.Substring(0, message.Length - " and ".Length);
        message += ".";
      }

      this.ErrorRaisedEvent(new ErrorDetails(message));
    }

    private void TryRaiseSettlementPlacingError(GameBoardData.VerificationResults verificationResults, UInt32 settlementLocation)
    {
      if (this.ErrorRaisedEvent == null)
      {
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationForSettlementIsInvalid)
      {
        this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Location " + settlementLocation + " is outside of board range (0 - 53)."));
        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.TooCloseToSettlement)
      {
        var player = this.playersById[verificationResults.PlayerId];
        if (player == this.currentPlayer)
        {
          this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Too close to own settlement at location " + verificationResults.LocationIndex + "."));
        }
        else
        {
          this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Too close to player '" + player.Name + "' at location " + verificationResults.LocationIndex + "."));
        }

        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.LocationIsOccupied)
      {
        var player = this.playersById[verificationResults.PlayerId];
        if (player == this.currentPlayer)
        {
          this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Location " + verificationResults.LocationIndex + " already settled by you."));
        }
        else
        {
          this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Location " + settlementLocation + " already settled by player '" + player.Name + "'."));
        }

        return;
      }

      if (verificationResults.Status == GameBoardData.VerificationStatus.SettlementNotConnectedToExistingRoad)
      {
        this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Location " + verificationResults.LocationIndex + " not connected to existing road."));
        return;
      }
    }

    private Boolean VerifyBuildCityRequest(UInt32 location)
    {
      if (!this.CanBuildCity())
      {
        this.TryRaiseCityBuildingError();
        return false;
      }

      var placeCityResults = this.gameBoard.CanPlaceCity(this.currentPlayer.Id, location);
      if (placeCityResults.Status != GameBoardData.VerificationStatus.Valid)
      {
        this.TryRaiseCityPlacingError(placeCityResults, location);
        return false;
      }

      return true;
    }

    private Boolean VerifyBuildRoadSegmentRequest(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      if (!this.CanBuildRoadSegment())
      {
        this.TryRaiseRoadSegmentBuildingError();
        return false;
      }

      var placeRoadStatus = this.gameBoard.CanPlaceRoad(this.currentPlayer.Id, roadStartLocation, roadEndLocation);
      if (placeRoadStatus.Status != GameBoardData.VerificationStatus.Valid)
      {
        this.TryRaiseRoadSegmentPlacingError(placeRoadStatus, roadStartLocation, roadEndLocation);
        return false;
      }

      return true;
    }

    private Boolean VerifyBuildSettlementRequest(UInt32 settlementLocation)
    {
      if (!this.CanBuildSettlement())
      {
        this.TryRaiseSettlementBuildingError();
        return false;
      }

      var canPlaceSettlementResults = this.gameBoard.CanPlaceSettlement(this.mainPlayer.Id, settlementLocation);
      if (canPlaceSettlementResults.Status != GameBoardData.VerificationStatus.Valid)
      {
        this.TryRaiseSettlementPlacingError(canPlaceSettlementResults, settlementLocation);
        return false;
      }

      return true;
    }

    private Boolean VerifyParametersForResourceTransactionWhenUsingKnightCard(UInt32 newRobberHex, Guid playerId)
    {
      if (!this.VerifyPlacementOfRobber(newRobberHex))
      {
        return false;
      }

      var playerIdsOnHex = new List<Guid>(this.gameBoard.GetPlayersForHex(newRobberHex));
      if (!playerIdsOnHex.Contains(playerId))
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Player Id (" + playerId + ") does not match with any players on hex " + newRobberHex + "."));
        return false;
      }

      return true;
    }

    private Boolean VerifyParametersForUsingDevelopmentCard(TurnToken turnToken, DevelopmentCard developmentCard)
    {
      if (turnToken != this.currentTurnToken)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token not recognised."));
        return false;
      }

      if (developmentCard == null)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Development card parameter is null."));
        return false;
      }

      if (cardsPurchasedThisTurn.Contains(developmentCard))
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot use development card that has been purchased this turn."));
        return false;
      }

      if (this.cardPlayedThisTurn)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot play more than one development card in a turn."));
        return false;
      }

      if (this.cardsPlayed.Contains(developmentCard))
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot play the same development card more than once."));
        return false;
      }

      return true;
    }

    private Boolean VerifyPlacementOfRobber(UInt32 newRobberHex)
    {
      if (!this.gameBoard.CanPlaceRobber(newRobberHex))
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot move robber to hex " + newRobberHex + " because it is out of bounds (0.. " + (GameBoardData.StandardBoardHexCount - 1) + ")."));
        return false;
      }

      if (newRobberHex == this.robberHex)
      {
        this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot place robber back on present hex (" + this.robberHex + ")."));
        return false;
      }

      return true;
    }

    private Boolean VerifyStartingInfrastructurePlacementRequest(UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      var verificationResults = this.gameBoard.CanPlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, roadEndLocation);
      this.TryRaiseSettlementPlacingError(verificationResults, settlementLocation);
      this.TryRaiseRoadSegmentPlacingError(verificationResults, settlementLocation, roadEndLocation);
      
      return verificationResults.Status == GameBoardData.VerificationStatus.Valid;
    }
    #endregion
  }
}
