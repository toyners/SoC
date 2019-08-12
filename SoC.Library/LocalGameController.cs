
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using GameBoards;
    using GameEvents;
    using Interfaces;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.PlayerActions;
    using Jabberwocky.SoC.Library.PlayerData;
    using Jabberwocky.SoC.Library.Store;
    using Newtonsoft.Json;

    public class LocalGameController : IGameController
    {
        #region Enums
        public enum GamePhases
        {
            Initial,
            WaitingLaunch,
            StartGameSetup,
            ContinueGameSetup,
            CompleteGameSetup,
            StartGamePlay,
            ContinueGamePlay,
            SetRobberHex,
            ChooseResourceFromOpponent,
            DropResources,
            Quit,
            GameOver
        }

        [Flags]
        public enum BuildStatuses
        {
            Successful = 0,
            NoSettlements,
            NotEnoughResourcesForSettlement,
            NoRoads,
            NotEnoughResourcesForRoad,
            NoCities,
            NotEnoughResourcesForCity
        }

        public enum BuyStatuses
        {
            Successful = 0,
            NoCards,
            NotEnoughResources
        }
        #endregion

        #region Fields
        private IPlayerFactory playerPool;
        private bool cardPlayedThisTurn;
        private HashSet<DevelopmentCard> cardsPlayed;
        private HashSet<DevelopmentCard> cardsPurchasedThisTurn;
        private INumberGenerator numberGenerator;
        private GameBoard gameBoard;
        private int playerIndex;
        private IPlayer[] players;
        private Dictionary<Guid, IPlayer> playersById;
        private IComputerPlayer[] computerPlayers;
        private IPlayer playerWithLargestArmy;
        private IPlayer playerWithLongestRoad;
        private bool provideFullPlayerData;
        private IPlayer mainPlayer;
        private int resourcesToDrop;
        private uint robberHex;
        private Dictionary<Guid, int> robbingChoices;
        private GameToken currentTurnToken;
        private IPlayer currentPlayer;
        private IDevelopmentCardHolder developmentCardHolder;
        private uint dice1, dice2;
        #endregion

        #region Construction
        public LocalGameController(INumberGenerator dice, IPlayerFactory playerPool, bool provideFullPlayerData = false)
            : this(dice, playerPool, new GameBoard(BoardSizes.Standard), new DevelopmentCardHolder(), provideFullPlayerData) { }

        public LocalGameController(INumberGenerator dice, IPlayerFactory computerPlayerFactory, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder, bool provideFullPlayerData = false)
        {
            this.numberGenerator = dice;
            this.playerPool = computerPlayerFactory;
            this.gameBoard = gameBoard;
            this.developmentCardHolder = developmentCardHolder;
            this.GamePhase = GamePhases.Initial;
            this.cardsPlayed = new HashSet<DevelopmentCard>();
            this.cardsPurchasedThisTurn = new HashSet<DevelopmentCard>();
            this.provideFullPlayerData = provideFullPlayerData;
        }
        #endregion

        #region Properties
        [JsonProperty]
        public Guid GameId { get; private set; }
        public GamePhases GamePhase { get; private set; }
        #endregion

        #region Events
        public Action<CityPlacedEvent> CityBuiltEvent { get; set; }
        public Action<DevelopmentCard> DevelopmentCardPurchasedEvent { get; set; }
        public Action<DevelopmentCardBoughtEvent> DevelopmentCardBoughtEvent { get; set; }
        public Action<Guid, uint, uint> DiceRollEvent { get; set; }
        public Action<ErrorDetails> ErrorRaisedEvent { get; set; }
        public Action<GameBoardSetup> InitialBoardSetupEvent { get; set; }
        public Action<List<GameEvent>> GameEvents { get; set; }
        public Action<PlayerDataBase[]> GameJoinedEvent { get; set; }
        public Action<PlayerDataBase[], GameBoard> GameLoadedEvent { get; set; }
        public Action<Guid> GameOverEvent { get; set; }
        public Action<KnightCardPlayedEvent> KnightCardPlayedEvent { get; set; }
        public Action<Guid, Guid> LargestArmyEvent { get; set; }
        public Action<ClientAccount> LoggedInEvent { get; set; }
        public Action<Guid, Guid> LongestRoadBuiltEvent { get; set; }
        public Action<MakeDirectTradeOfferEvent> MakeDirectTradeOfferEvent { get; set; }
        public Action<PlayKnightCardEvent> PlayKnightCardEvent { get; set; } // TODO: Use KnightCardPlayedEvent instead
        public Action<ResourceUpdateEvent> ResourcesLostEvent { get; set; }
        public Action<ResourcesCollectedEvent> ResourcesCollectedEvent { get; set; }
        public Action<ResourceTransactionList> ResourcesTransferredEvent { get; set; }
        public Action<RoadSegmentPlacedEvent> RoadSegmentBuiltEvent { get; set; }
        public Action<int> RobberEvent { get; set; }
        public Action<Dictionary<Guid, int>> RobbingChoicesEvent { get; set; }
        public Action<SettlementPlacedEvent> SettlementBuiltEvent { get; set; }
        public Action<GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }
        public Action<GameToken> StartPlayerTurnEvent { get; set; }
        public Action<Guid> StartOpponentTurnEvent { get; set; }
        public Action<PlayerDataBase[]> TurnOrderFinalisedEvent { get; set; }

        public Action<GameToken> StartPlayerTurnEvent2 { get; set; }
        #endregion

        #region Methods
        public void BuildCity(GameToken turnToken, uint location)
        {
            if (!this.VerifyTurnToken(turnToken) || !this.VerifyBuildCityRequest(location))
            {
                return;
            }

            this.BuildCity(location);
            this.CityBuiltEvent?.Invoke(new CityPlacedEvent(this.mainPlayer.Id, location));
            this.CheckMainPlayerIsWinner();
        }

        public void BuildRoadSegment(GameToken turnToken, uint roadStartLocation, uint roadEndLocation)
        {
            if (!this.VerifyTurnToken(turnToken) || !this.VerifyBuildRoadSegmentRequest(roadStartLocation, roadEndLocation))
            {
                return;
            }

            this.BuildRoadSegment(roadStartLocation, roadEndLocation);
            var playerData = this.CreateSinglePlayerData(this.mainPlayer);
            this.RoadSegmentBuiltEvent?.Invoke(new RoadSegmentPlacedEvent(this.mainPlayer.Id, roadStartLocation, roadEndLocation));

            Guid previousPlayerWithLongestRoadId;
            if (this.PlayerHasJustBuiltTheLongestRoad(out previousPlayerWithLongestRoadId))
            {
                this.LongestRoadBuiltEvent?.Invoke(previousPlayerWithLongestRoadId, this.mainPlayer.Id);
            }
        
            this.CheckMainPlayerIsWinner();
        }

        public void BuildSettlement(GameToken turnToken, uint location)
        {
            if (!this.VerifyTurnToken(turnToken) || !this.VerifyBuildSettlementRequest(location))
            {
                return;
            }

            this.BuildSettlement(location);
            this.SettlementBuiltEvent?.Invoke(new SettlementPlacedEvent(this.mainPlayer.Id, location));
            this.CheckMainPlayerIsWinner();
        }

        public void BuyDevelopmentCard(GameToken turnToken)
        {
            if (!this.VerifyTurnToken(turnToken) || !this.VerifyBuyDevelopmentCardRequest())
            {
                return;
            }

            DevelopmentCard developmentCard = this.BuyDevelopmentCard();
            this.currentPlayer.HeldCards.Add(developmentCard);
            //this.DevelopmentCardPurchasedEvent?.Invoke(developmentCard); TODO: Reuse this name for below event
            this.DevelopmentCardBoughtEvent?.Invoke(new DevelopmentCardBoughtEvent(this.currentPlayer.Id));
        }

        public BuildStatuses CanBuildCity()
        {
            BuildStatuses result = BuildStatuses.Successful;
            if (this.mainPlayer.Resources < ResourceClutch.City)
                result |= BuildStatuses.NotEnoughResourcesForCity;

            if (this.mainPlayer.RemainingCities == 0)
                result |= BuildStatuses.NoCities;

            return result;
        }

        public BuildStatuses CanBuildRoadSegment()
        {
            BuildStatuses result = BuildStatuses.Successful;
            if (this.mainPlayer.Resources < ResourceClutch.RoadSegment)
                result |= BuildStatuses.NotEnoughResourcesForRoad;

            if (this.mainPlayer.RemainingRoadSegments == 0)
                result |= BuildStatuses.NoRoads;

            return result;
        }

        public BuildStatuses CanBuildSettlement()
        {
            BuildStatuses result = BuildStatuses.Successful;
            if (this.mainPlayer.Resources < ResourceClutch.Settlement)
                result |= BuildStatuses.NotEnoughResourcesForSettlement;

            if (this.mainPlayer.RemainingSettlements == 0)
                result |= BuildStatuses.NoSettlements;

            return result;
        }

        public BuyStatuses CanBuyDevelopmentCard()
        {
            if (!this.developmentCardHolder.HasCards)
                return BuyStatuses.NoCards;

            if (this.mainPlayer.Resources < ResourceClutch.DevelopmentCard)
                return BuyStatuses.NotEnoughResources;

            return BuyStatuses.Successful;
        }

        public void ChooseResourceFromOpponent(Guid opponentId)
        {
            if (this.GamePhase == GamePhases.ContinueGamePlay)
            {
                var message = "Cannot call 'ChooseResourceFromOpponent' when no robbing choices are available.";
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

        // 05 Complete game setup
        public void CompleteGameSetup(uint settlementLocation, uint roadEndLocation)
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
            var initialResources = this.GetInitialResourcesForPlayer(settlementLocation);
            this.mainPlayer.AddResources(initialResources.Resources);

            var gameEvents = new List<GameEvent>();
            //gameEvents.Add(new ResourcesCollectedEvent(this.mainPlayer.Id, new[] { initialResources }));
            this.CompleteSetupForComputerPlayers(gameEvents);

            this.GameEvents?.Invoke(gameEvents);
            this.GamePhase = GamePhases.StartGamePlay;
        }

        public void ContinueGamePlay()
        {
            if (this.GamePhase != GamePhases.ContinueGamePlay)
            {
                var errorDetails = new ErrorDetails("Can only call 'ContinueGamePlay' when loading from file.");
                this.ErrorRaisedEvent?.Invoke(errorDetails);
                return;
            }

            var playerData = this.CreatePlayerData();
            this.GameJoinedEvent?.Invoke(playerData);

            var gameBoardSetup = new GameBoardSetup(this.gameBoard);
            this.InitialBoardSetupEvent?.Invoke(gameBoardSetup);

            this.currentTurnToken = new GameToken();
            this.StartPlayerTurnEvent?.Invoke(this.currentTurnToken);

            this.DiceRollEvent?.Invoke(this.mainPlayer.Id, this.dice1, this.dice2);
        }

        // 04 Continue game setup
        public void ContinueGameSetup(uint settlementLocation, uint roadEndLocation)
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

            this.gameBoard.PlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, roadEndLocation);
            this.mainPlayer.PlaceStartingInfrastructure();

            var gameEvents = this.ContinueSetupForComputerPlayers();

            this.playerIndex = this.players.Length - 1;
            this.CompleteSetupForComputerPlayers(gameEvents);

            this.GameEvents?.Invoke(gameEvents);
            this.GamePhase = GamePhases.CompleteGameSetup;
        }

        public void DropResources(ResourceClutch resourceClutch)
        {
            // TODO: Validate the parameter - the total should match the expected resources to drop
            // when robber roll occurred.
            this.mainPlayer.RemoveResources(resourceClutch);
        }

        private bool startOfTurn = true;
        public void EndPlayerTurn(GameToken turnToken)
        {
            // TODO: Validate turn token

            this.actionRequests.Enqueue(new EndOfTurnAction(Guid.Empty));
        }

        private bool ProcessTurnStart(IPlayer player)
        {
            this.ClearDevelopmentCardProcessingForTurn();
            this.currentTurnToken = new GameToken();

            this.StartOpponentTurnEvent?.Invoke(player.Id);

            // Roll dice
            this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
            this.DiceRollEvent?.Invoke(player.Id, this.dice1, this.dice2);
            var roll = this.dice1 + this.dice2;

            // Collect resources OR handle robber
            var moveRobber = false;
            if (roll == 7)
            {
                this.DropResourcesForPlayers();
                moveRobber = true;
            }
            else
            {
                this.CollectResourcesAtStartOfTurn(roll);
            }

            return moveRobber;
        }

        private void DropResourcesForPlayers()
        {
            // TODO: Refactoring - improve names and wrap in method
            Dictionary<Guid, ResourceClutch> droppedResourcesByPlayerId = null;
            foreach (var cp in this.computerPlayers)
            {
                var d = cp.GetDropResourcesAction();
                if (d != null)
                {
                    if (droppedResourcesByPlayerId == null)
                        droppedResourcesByPlayerId = new Dictionary<Guid, ResourceClutch>();

                    cp.RemoveResources(d.Resources);
                    droppedResourcesByPlayerId.Add(cp.Id, d.Resources);
                }
            }

            if (droppedResourcesByPlayerId != null)
                this.ResourcesLostEvent?.Invoke(new ResourceUpdateEvent(droppedResourcesByPlayerId));
        }

        private IPlayer GetNextPlayer()
        {
            throw new NotImplementedException();
        }

        public void EndTurn(GameToken turnToken)
        {
            if (turnToken != this.currentTurnToken)
            {
                return;
            }

            this.actionRequests.Enqueue(new EndOfTurnAction(Guid.Empty));

            /*this.ClearDevelopmentCardProcessingForTurn();
            this.ChangeToNextPlayerTurn();

            while (this.currentPlayer.IsComputer)
            {
                var computerPlayer = (IComputerPlayer)this.currentPlayer;
                var events = new List<GameEvent>();

                this.StartOpponentTurnEvent?.Invoke(computerPlayer.Id);

                this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
                this.DiceRollEvent?.Invoke(computerPlayer.Id, this.dice1, this.dice2);
                //var rolledDiceEvent = new DiceRollEvent(computerPlayer.Id, this.dice1, this.dice2);
                //events.Add(rolledDiceEvent);
                var roll = this.dice1 + this.dice2;

                var moveRobber = false;
                if (roll == 7)
                {
                    // TODO: Refactoring - improve names and wrap in method
                    Dictionary<Guid, ResourceClutch> droppedResourcesByPlayerId = null;
                    foreach (var cp in this.computerPlayers)
                    {
                        var d = cp.GetDropResourcesAction();
                        if (d != null)
                        {
                            if (droppedResourcesByPlayerId == null)
                                droppedResourcesByPlayerId = new Dictionary<Guid, ResourceClutch>();

                            cp.RemoveResources(d.Resources);
                            droppedResourcesByPlayerId.Add(cp.Id, d.Resources);
                        }
                    }

                    if (droppedResourcesByPlayerId != null)
                        this.ResourcesLostEvent?.Invoke(new ResourceUpdateEvent(droppedResourcesByPlayerId));

                    moveRobber = true;
                }
                else
                {
                    this.CollectResourcesAtStartOfTurn(roll);
                }

                computerPlayer.BuildInitialPlayerActions(null, moveRobber);

                ComputerPlayerAction playerAction;
                while ((playerAction = computerPlayer.GetPlayerAction()) != null && computerPlayer.VictoryPoints < 10)
                {
                    if (playerAction is BuildCityAction buildCityAction)
                    {
                        this.BuildCity(buildCityAction.CityLocation);
                        this.CityBuiltEvent?.Invoke(new CityBuiltEvent(computerPlayer.Id, buildCityAction.CityLocation));
                        this.CheckComputerPlayerIsWinner(computerPlayer, events);
                    }
                    else if (playerAction is BuildRoadSegmentAction buildRoadSegmentAction)
                    {
                        this.BuildRoadSegment(buildRoadSegmentAction.StartLocation, buildRoadSegmentAction.EndLocation);
                        this.RoadSegmentBuiltEvent?.Invoke(new RoadSegmentBuiltEvent(computerPlayer.Id, buildRoadSegmentAction.StartLocation, buildRoadSegmentAction.EndLocation));
                        if (this.PlayerHasJustBuiltTheLongestRoad(out Guid previousPlayerWithLongestRoadId))
                        {
                            events.Add(new LongestRoadBuiltEvent(computerPlayer.Id, previousPlayerWithLongestRoadId));
                        }

                        this.CheckComputerPlayerIsWinner(computerPlayer, events);
                    }
                    else if (playerAction is BuildSettlementAction buildSettlementAction)
                    {
                        this.BuildSettlement(buildSettlementAction.SettlementLocation);
                        this.SettlementBuiltEvent?.Invoke(new SettlementBuiltEvent(computerPlayer.Id, buildSettlementAction.SettlementLocation));
                        this.CheckComputerPlayerIsWinner(computerPlayer, events);
                    }
                    else if (playerAction is BuyDevelopmentCardAction)
                    {
                        var developmentCard = this.BuyDevelopmentCard();
                        computerPlayer.AddDevelopmentCard(developmentCard);
                        this.DevelopmentCardBoughtEvent?.Invoke(new DevelopmentCardBoughtEvent(computerPlayer.Id));
                    }
                    else if (playerAction is DropResourcesAction dropResourcesAction)
                    {
                        computerPlayer.RemoveResources(dropResourcesAction.Resources);
                    }
                    else if (playerAction is MakeDirectTradeOfferAction makeDirectTradeOfferAction)
                    {
                        this.MakeDirectTradeOfferEvent?.Invoke(new MakeDirectTradeOfferEvent(computerPlayer.Id, makeDirectTradeOfferAction.WantedResources));
                    }
                    else if (playerAction is PlaceRobberAction placeRobberAction)
                    {
                        this.robberHex = placeRobberAction.RobberHex;
                    }
                    else if (playerAction is PlayKnightCardAction playKnightCardAction)
                    {
                        var knightCard = computerPlayer.GetKnightCard();
                        this.PlayKnightDevelopmentCard(knightCard, playKnightCardAction.NewRobberHex);
                        this.KnightCardPlayedEvent?.Invoke(new KnightCardPlayedEvent(computerPlayer.Id, playKnightCardAction.NewRobberHex));

                        if (playKnightCardAction.PlayerId.HasValue)
                        {
                            var robbedPlayer = this.playersById[playKnightCardAction.PlayerId.Value];
                            var takenResource = this.GetResourceFromPlayer(robbedPlayer);
                            computerPlayer.AddResources(takenResource);
                            var resourceTransaction = new ResourceTransaction(computerPlayer.Id, robbedPlayer.Id, takenResource);
                            var resourceLostEvent = new ResourceTransactionEvent(computerPlayer.Id, resourceTransaction);
                            events.Add(resourceLostEvent);
                        }

                        if (this.PlayerHasJustBuiltTheLargestArmy(out Guid previousPlayerId))
                        {
                            events.Add(new LargestArmyChangedEvent(this.playerWithLargestArmy.Id, previousPlayerId));
                        }

                        this.CheckComputerPlayerIsWinner(computerPlayer, events);
                    }
                    else if (playerAction is SelectResourceFromPlayerAction selectResourceFromPlayerAction)
                    {
                        var opponent = this.playersById[selectResourceFromPlayerAction.PlayerId];
                        var takenResource = this.GetResourceFromPlayer(opponent);
                        computerPlayer.AddResources(takenResource);

                        var resourceTransactionList = new ResourceTransactionList();
                        resourceTransactionList.Add(new ResourceTransaction(computerPlayer.Id, opponent.Id, takenResource));
                        events.Add(new ResourceTransactionEvent(computerPlayer.Id, resourceTransactionList));
                        this.ResourcesTransferredEvent?.Invoke(resourceTransactionList);
                    }
                    else
                    {
                        switch (playerAction.ActionType)
                        {
                            case ComputerPlayerActionTypes.PlayMonopolyCard:
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

                            case ComputerPlayerActionTypes.PlayYearOfPlentyCard:
                            {
                                var yearOfPlentyCard = computerPlayer.ChooseYearOfPlentyCard();
                                var resourcesCollected = computerPlayer.ChooseResourcesToCollectFromBank();
                                computerPlayer.AddResources(resourcesCollected);

                                var resourceTransaction = new ResourceTransaction(computerPlayer.Id, this.playerPool.GetBankId(), resourcesCollected);
                                var resourceTransactions = new ResourceTransactionList();
                                resourceTransactions.Add(resourceTransaction);

                                events.Add(new PlayYearOfPlentyCardEvent(computerPlayer.Id, resourceTransactions));
                                break;
                            }

                            case ComputerPlayerActionTypes.TradeWithBank:
                            {
                                var tradeWithBankAction = (TradeWithBankAction)playerAction;

                                var receivingResources = ResourceClutch.CreateFromResourceType(tradeWithBankAction.ReceivingType);
                                receivingResources *= tradeWithBankAction.ReceivingCount;

                                var paymentResources = ResourceClutch.CreateFromResourceType(tradeWithBankAction.GivingType);
                                paymentResources *= (tradeWithBankAction.ReceivingCount * 4);

                                computerPlayer.RemoveResources(paymentResources);
                                computerPlayer.AddResources(receivingResources);

                                events.Add(new TradeWithBankEvent(computerPlayer.Id, this.playerPool.GetBankId(), paymentResources, receivingResources));
                                break;
                            }

                            default: throw new NotImplementedException("Player action '" + playerAction + "' is not recognised.");
                        }
                    }
                }

                if (events.Count > 0)
                {
                    this.GameEvents?.Invoke(events);
                }

                if (computerPlayer.VictoryPoints >= 10)
                {
                    this.GameOverEvent?.Invoke(computerPlayer.Id);
                    return;
                }

                this.ClearDevelopmentCardProcessingForTurn();
                this.ChangeToNextPlayerTurn();
            }

            this.currentTurnToken = new TurnToken();
            this.StartPlayerTurnEvent?.Invoke(this.currentTurnToken);

            this.numberGenerator.RollTwoDice(out var dice1, out var dice2);
            this.DiceRollEvent?.Invoke(this.mainPlayer.Id, dice1, dice2);

            this.CollectResourcesAtStartOfTurn(dice1 + dice2);*/
        } 

        public uint[] GetNeighbouringLocationsFrom(uint location)
        {
            return this.gameBoard.BoardQuery.GetNeighbouringLocationsFrom(location);
        }

        public GameState GetState()
        {
            return new GameState();
        }

        public uint[] GetValidConnectedLocationsFrom(uint location)
        {
            return this.gameBoard.BoardQuery.GetValidConnectedLocationsFrom(location);
        }

        // 01 Join game
        public void JoinGame(GameOptions gameOptions = null)
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
            var playerData = this.CreatePlayerData();
            this.GameJoinedEvent?.Invoke(playerData);
            this.GamePhase = GamePhases.WaitingLaunch;
        }

        // 02 Launch Game
        public void LaunchGame()
        {
            if (this.GamePhase != GamePhases.WaitingLaunch)
            {
                var errorDetails = new ErrorDetails("Cannot call 'LaunchGame' without joining game.");
                this.ErrorRaisedEvent?.Invoke(errorDetails);
                return;
            }

            var gameBoardSetup = new GameBoardSetup(this.gameBoard);
            this.InitialBoardSetupEvent?.Invoke(gameBoardSetup);
            this.GamePhase = GamePhases.StartGameSetup;
        }

        /// <summary>
        /// Load the game controller data from stream.
        /// </summary>
        /// <param name="stream">Stream containing game controller data.</param>
        [Obsolete("Deprecated. Use Load(IGameDataReader<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> reader) instead.")]
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
                            var location = uint.Parse(reader.GetAttribute("location"));

                            //this.gameBoard.InternalPlaceSettlement(playerId, location);
                        }

                        if (reader.Name == "roads" && reader.NodeType == XmlNodeType.Element)
                        {
                            this.gameBoard.ClearRoads();
                        }

                        if (reader.Name == "road" && reader.NodeType == XmlNodeType.Element)
                        {
                            var playerId = Guid.Parse(reader.GetAttribute("playerid"));
                            var start = uint.Parse(reader.GetAttribute("start"));
                            var end = uint.Parse(reader.GetAttribute("end"));

                            //this.gameBoard.InternalPlaceRoadSegment(playerId, start, end);
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

                var playerDataViews = this.CreatePlayerData();

                this.GameLoadedEvent?.Invoke(playerDataViews, this.gameBoard);
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown during board loading.", e);
            }
        }

        public void Load(string filePath)
        {
            var content = File.ReadAllText(filePath);
            var gameModel = JsonConvert.DeserializeObject<GameModel>(content);

            this.gameBoard = new GameBoard(BoardSizes.Standard, gameModel.Board);

            this.computerPlayers = new IComputerPlayer[3]; // TODO - Change to handle different number of computer players
            this.players = new IPlayer[4]; // TODO - Change to handle different number of players

            this.mainPlayer = this.players[0] = new Player(gameModel.Player1);
            this.computerPlayers[0] = (IComputerPlayer)(this.players[1] = new ComputerPlayer(gameModel.Player2, this.numberGenerator));
            this.computerPlayers[1] = (IComputerPlayer)(this.players[2] = new ComputerPlayer(gameModel.Player3, this.numberGenerator));
            this.computerPlayers[2] = (IComputerPlayer)(this.players[3] = new ComputerPlayer(gameModel.Player4, this.numberGenerator));

            this.playersById = new Dictionary<Guid, IPlayer>(4);
            foreach (var player in this.players)
                this.playersById.Add(player.Id, player);

            this.dice1 = gameModel.Dice1;
            this.dice2 = gameModel.Dice2;

            this.GamePhase = GamePhases.ContinueGamePlay;
        }

        [Obsolete("Deprecated. Use Load(string filePath) instead.")]
        public void Load(IGameDataReader<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> reader)
        {
            try
            {
                this.gameBoard.Load(reader);

                var loadedPlayers = new List<IPlayer>();

                var player = this.playerPool.CreatePlayer(reader[GameDataSectionKeys.PlayerOne]);
                loadedPlayers.Add(player);

                IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data = null;
                var key = GameDataSectionKeys.PlayerTwo;
                while (key <= GameDataSectionKeys.PlayerFour && (data = reader[key++]) != null)
                {
                    player = this.playerPool.CreateComputerPlayer(data, this.gameBoard, this.numberGenerator);
                    loadedPlayers.Add(player);
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
                        player = loadedPlayers[index];
                        this.players[index] = player;
                        this.playersById.Add(player.Id, player);
                    }
                }
                else
                {
                    this.CreatePlayers(new GameOptions());
                }

                var playerDataViews = this.CreatePlayerData();

                this.GameLoadedEvent?.Invoke(playerDataViews, this.gameBoard);
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown during board loading.", e);
            }
        }

        public void Quit()
        {
            this.GamePhase = GamePhases.Quit;
        }

        public void Save(string filePath)
        {
            var gameModel = new GameModel();
            gameModel.Board = new GameBoardModel(this.gameBoard);
            gameModel.Player1 = new PlayerModel(this.mainPlayer);
            gameModel.Player2 = new PlayerModel(this.computerPlayers[0]);
            gameModel.Player3 = new PlayerModel(this.computerPlayers[1]);
            gameModel.Player4 = new PlayerModel(this.computerPlayers[2]);
            gameModel.RobberLocation = this.robberHex;
            gameModel.DevelopmentCards = this.developmentCardHolder.GetDevelopmentCards();
            gameModel.Dice1 = this.dice1;
            gameModel.Dice2 = this.dice2;
            var content = JsonConvert.SerializeObject(gameModel, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, content);
        }

        public void SetRobberHex(uint location)
        {
            if (this.GamePhase != GamePhases.SetRobberHex)
            {
                var resourceDropErrorDetails = new ErrorDetails($"Cannot set robber location until expected resources ({this.resourcesToDrop}) have been dropped via call to DropResources method.");
                this.ErrorRaisedEvent?.Invoke(resourceDropErrorDetails);
                return;
            }

            var playerIds = this.gameBoard.GetPlayersForHex(location);
            if (this.PlayerIdsIsEmptyOrOnlyContainsMainPlayer(playerIds))
            {
                this.GamePhase = GamePhases.ContinueGamePlay;
                this.RobbingChoicesEvent?.Invoke(null);
                return;
            }

            this.robbingChoices = new Dictionary<Guid, int>();
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

            // Launch server processing on separate thread
            Task.Factory.StartNew(() =>
            {
                this.GameLoop();
            });

            /*this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
            this.DiceRollEvent?.Invoke(this.currentPlayer.Id, this.dice1, this.dice2);

            var resourceRoll = this.dice1 + this.dice2;
            if (resourceRoll != 7)
            {
                this.CollectResourcesAtStartOfTurn(resourceRoll);
                this.GamePhase = GamePhases.ContinueGamePlay;
            }
            else
            {
                ResourceUpdateEvent resourcesDroppedByComputerPlayers = null;

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
                        var resourcesToDrop = computerPlayer.ChooseResourcesToDrop();
                        computerPlayer.RemoveResources(resourcesToDrop);

                        // TODO: Tidy this up - passing in empty dict is wrong
                        if (resourcesDroppedByComputerPlayers == null)
                        {
                            resourcesDroppedByComputerPlayers = new ResourceUpdateEvent(new Dictionary<Guid, ResourceClutch>());
                        }

                        resourcesDroppedByComputerPlayers.Resources.Add(computerPlayer.Id, resourcesToDrop);
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
            }*/
        }

        // 03 Start game setup
        public bool StartGameSetup()
        {
            if (this.GamePhase != GamePhases.StartGameSetup)
            {
                return false;
            }

            this.players = PlayerTurnOrderCreator.Create(this.players, this.numberGenerator);

            this.playerIndex = 0;
            var gameEvents = this.ContinueSetupForComputerPlayers();
            this.GameEvents?.Invoke(gameEvents);
            this.GamePhase = GamePhases.ContinueGameSetup;

            return true;
        }

        /// <summary>
        /// Trade resources with the bank at a 4-to-1 ratio. Errors will be returned if the transaction cannot be completed.
        /// </summary>
        /// <param name="turnToken">Token of the current turn.</param>
        /// <param name="receivingResourceType">Resource type that the player wants to receive.</param>
        /// <param name="receivingCount">Resource amount that the player wants to receive.</param>
        /// <param name="givingResourceType">Resource type that the player is giving. Must have at least 4 and be divisible by 4.</param>
        public void TradeWithBank(GameToken turnToken, ResourceTypes receivingResourceType, int receivingCount, ResourceTypes givingResourceType)
        {
            if (!this.VerifyTurnToken(turnToken))
            {
                return;
            }

            int resourceCount = 0;
            switch (givingResourceType)
            {
                case ResourceTypes.Brick: resourceCount = this.mainPlayer.BrickCount; break;
                case ResourceTypes.Grain: resourceCount = this.mainPlayer.GrainCount; break;
                case ResourceTypes.Lumber: resourceCount = this.mainPlayer.LumberCount; break;
                case ResourceTypes.Ore: resourceCount = this.mainPlayer.OreCount; break;
                case ResourceTypes.Wool: resourceCount = this.mainPlayer.WoolCount; break;
            }

            if (!this.VerifyTradeWithBank(receivingCount, resourceCount, givingResourceType, receivingResourceType))
            {
                return;
            }

            var receivingResource = ResourceClutch.CreateFromResourceType(receivingResourceType);
            receivingResource *= receivingCount;

            var paymentResource = ResourceClutch.CreateFromResourceType(givingResourceType);
            paymentResource *= (receivingCount * 4);

            this.mainPlayer.RemoveResources(paymentResource);
            this.mainPlayer.AddResources(receivingResource);

            var resourceTransactionList = new ResourceTransactionList();
            resourceTransactionList.Add(new ResourceTransaction(this.playerPool.GetBankId(), this.mainPlayer.Id, paymentResource));
            resourceTransactionList.Add(new ResourceTransaction(this.mainPlayer.Id, this.playerPool.GetBankId(), receivingResource));

            this.ResourcesTransferredEvent?.Invoke(resourceTransactionList);
        }

        public void UseKnightCard(GameToken turnToken, KnightDevelopmentCard developmentCard, uint newRobberHex, Guid? playerId = null)
        {
            if (!this.VerifyParametersForUsingDevelopmentCard(turnToken, developmentCard, "knight"))
            {
                return;
            }

            if (!this.VerifyPlacementOfRobber(newRobberHex))
            {
                return;
            }

            if (playerId.HasValue && !this.VerifyPlayerForResourceTransactionWhenUsingKnightCard(newRobberHex, playerId.Value))
            {
                return;
            }

            this.PlayKnightDevelopmentCard(developmentCard, newRobberHex);

            this.KnightCardPlayedEvent?.Invoke(new KnightCardPlayedEvent(this.mainPlayer.Id, newRobberHex));

            if (playerId.HasValue)
            {
                this.CompleteResourceTransactionBetweenPlayers(this.playersById[playerId.Value]);
            }

            if (this.PlayerHasJustBuiltTheLargestArmy(out Guid previousPlayerId))
            {
                this.LargestArmyEvent?.Invoke(this.playerWithLargestArmy.Id, previousPlayerId);
            }

            this.CheckMainPlayerIsWinner();
        }

        public void UseMonopolyCard(GameToken turnToken, MonopolyDevelopmentCard monopolyCard, ResourceTypes resourceType)
        {
            if (!this.VerifyParametersForUsingDevelopmentCard(turnToken, monopolyCard, "monopoly"))
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

        public void UseYearOfPlentyCard(GameToken turnToken, YearOfPlentyDevelopmentCard yearOfPlentyCard, ResourceTypes firstChoice, ResourceTypes secondChoice)
        {
            if (!this.VerifyParametersForUsingDevelopmentCard(turnToken, yearOfPlentyCard, "year of plenty"))
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

        private void AddResourcesToList(List<ResourceTypes> resources, ResourceTypes resourceType, int total)
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

        private void BuildCity(uint location)
        {
            this.gameBoard.PlaceCity(this.currentPlayer.Id, location);
            this.currentPlayer.PlaceCity();
        }

        private void BuildRoadSegment(uint roadStartLocation, uint roadEndLocation)
        {
            this.gameBoard.PlaceRoadSegment(this.currentPlayer.Id, roadStartLocation, roadEndLocation);
            this.currentPlayer.PlaceRoadSegment();
        }

        private void BuildSettlement(uint location)
        {
            this.gameBoard.PlaceSettlement(this.currentPlayer.Id, location);
            this.currentPlayer.PlaceSettlement();
        }

        private DevelopmentCard BuyDevelopmentCard()
        {
            DevelopmentCard developmentCard;
            this.developmentCardHolder.TryGetNextCard(out developmentCard);
            this.currentPlayer.BuyDevelopmentCard();
            this.cardsPurchasedThisTurn.Add(developmentCard);
            return developmentCard;
        }

        private void ChangeToNextPlayerTurn()
        {
            this.playerIndex++;
            if (this.playerIndex == this.players.Length)
            {
                this.playerIndex = 0;
            }

            this.currentPlayer = this.players[this.playerIndex];
        }

        private bool CheckComputerPlayerIsWinner(IComputerPlayer computerPlayer, List<GameEvent> events)
        {
            if (computerPlayer.VictoryPoints >= 10)
            {
                events.Add(new GameWinEvent(computerPlayer.Id, computerPlayer.VictoryPoints));
                this.GamePhase = GamePhases.GameOver;
                return true;
            }

            return false;
        }

        private void CheckMainPlayerIsWinner()
        {
            if (this.mainPlayer.VictoryPoints >= 10)
            {
                this.GameOverEvent?.Invoke(this.mainPlayer.Id);
                this.GamePhase = GamePhases.GameOver;
            }
        }

        private ResourceCollection GetInitialResourcesForPlayer(uint settlementLocation)
        {
            var resources = this.gameBoard.GetResourcesForLocation(settlementLocation);
            var collectedResources = new ResourceCollection(settlementLocation, resources);
            return collectedResources;
        }

        private void CollectResourcesAtStartOfTurn(uint resourceRoll)
        {
            var resources = this.gameBoard.GetResourcesForRoll(resourceRoll);
            foreach (var player in this.players)
            {
                if (!resources.TryGetValue(player.Id, out var resourcesCollectionForPlayer))
                    continue;

                var resourcesCollectionOrderedByLocation = resourcesCollectionForPlayer
                    .OrderBy(rc => rc.Location).ToArray();

                foreach (var resourceCollection in resourcesCollectionForPlayer)
                    player.AddResources(resourceCollection.Resources);

                //var resourcesCollectedEvent = new ResourcesCollectedEvent(player.Id, resourcesCollectionOrderedByLocation);
               // this.ResourcesCollectedEvent?.Invoke(resourcesCollectedEvent);
            }
        }

        private List<GameEvent> ContinueSetupForComputerPlayers()
        {
            var gameEvents = new List<GameEvent>();

            while (this.playerIndex < this.players.Length)
            {
                var player = this.players[this.playerIndex++];

                if (!player.IsComputer)
                {
                    break;
                }

                var computerPlayer = (IComputerPlayer)player;
                computerPlayer.ChooseInitialInfrastructure(out var chosenSettlementLocation, out var chosenRoadSegmentEndLocation);
                this.gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, chosenSettlementLocation, chosenRoadSegmentEndLocation);

                computerPlayer.PlaceStartingInfrastructure();

                gameEvents.Add(new InfrastructurePlacedEvent(computerPlayer.Id, chosenSettlementLocation, chosenRoadSegmentEndLocation));
            }

            return gameEvents;
        }

        private void CompleteResourceTransactionBetweenPlayers(IPlayer playerToTakeResourceFrom)
        {
            var takenResource = this.GetResourceFromPlayer(playerToTakeResourceFrom);
            this.mainPlayer.AddResources(takenResource);

            var resourceTransactionList = new ResourceTransactionList();
            resourceTransactionList.Add(new ResourceTransaction(this.mainPlayer.Id, playerToTakeResourceFrom.Id, takenResource));

            this.ResourcesTransferredEvent?.Invoke(resourceTransactionList);
        }

        private void CompleteSetupForComputerPlayers(List<GameEvent> gameEvents)
        {
            while (this.playerIndex >= 0)
            {
                var player = this.players[this.playerIndex--];

                if (!player.IsComputer)
                {
                    return;
                }

                var computerPlayer = (IComputerPlayer)player;
                computerPlayer.ChooseInitialInfrastructure(out var chosenSettlementLocation, out var chosenRoadSegmentEndLocation);
                this.gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, chosenSettlementLocation, chosenRoadSegmentEndLocation);

                computerPlayer.PlaceStartingInfrastructure();

                gameEvents.Add(new InfrastructurePlacedEvent(computerPlayer.Id, chosenSettlementLocation, chosenRoadSegmentEndLocation));

                var initialResources = this.GetInitialResourcesForPlayer(chosenSettlementLocation);
                computerPlayer.AddResources(initialResources.Resources);

                //gameEvents.Add(new ResourcesCollectedEvent(computerPlayer.Id, new[] { initialResources }));
            }
        }

        private PlayerDataBase[] CreatePlayerData()
        {
            var playerDataViews = new PlayerDataBase[this.players.Length];

            for (var index = 0; index < playerDataViews.Length; index++)
            {
                playerDataViews[index] = this.players[index].GetDataModel(this.provideFullPlayerData);
            }

            return playerDataViews;
        }

        private PlayerDataBase CreateSinglePlayerData(IPlayer player)
        {
            return player.GetDataModel(this.provideFullPlayerData);
        }

        private void CreatePlayers(GameOptions gameOptions)
        {
            this.mainPlayer = this.playerPool.CreatePlayer();
            this.players = new IPlayer[gameOptions.MaxAIPlayers + 1];
            this.players[0] = this.mainPlayer;
            this.playersById = new Dictionary<Guid, IPlayer>(this.players.Length);
            this.playersById.Add(this.mainPlayer.Id, this.mainPlayer);
            this.computerPlayers = new IComputerPlayer[gameOptions.MaxAIPlayers];

            var index = 1;
            while (gameOptions.MaxAIPlayers-- > 0)
            {
                var computerPlayer = this.playerPool.CreateComputerPlayer(this.gameBoard, this, this.numberGenerator);
                this.players[index] = computerPlayer;
                this.playersById.Add(computerPlayer.Id, computerPlayer);
                this.computerPlayers[index - 1] = (IComputerPlayer)computerPlayer;
                index++;
            }
        }

        private IPlayer DeterminePlayerWithMostKnightCards()
        {
            IPlayer playerWithMostKnightCards = null;
            int workingKnightCardCount = 3;

            foreach (var player in this.players)
            {
                if (player.PlayedKnightCards > workingKnightCardCount)
                {
                    playerWithMostKnightCards = player;
                    workingKnightCardCount = player.PlayedKnightCards;
                }
                else if (player.PlayedKnightCards == workingKnightCardCount)
                {
                    playerWithMostKnightCards = (playerWithMostKnightCards == null ? player : null);
                }
            }

            return playerWithMostKnightCards;
        }

        private void ClearDevelopmentCardProcessingForTurn()
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
            var resourceIndex = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(player.ResourcesCount);
            return player.LoseResourceAtIndex(resourceIndex);
        }

        private void HandlePlaceRoadError(GameBoard.VerificationStatus status)
        {
            var message = String.Empty;
            switch (status)
            {
                case GameBoard.VerificationStatus.RoadIsOffBoard: message = "Cannot place road segment because board location is not valid."; break;
                case GameBoard.VerificationStatus.RoadIsOccupied: message = "Cannot place road segment because road segment already exists."; break;
                case GameBoard.VerificationStatus.NoDirectConnection: message = "Cannot build road segment because no direct connection between start location and end location."; break;
                case GameBoard.VerificationStatus.RoadNotConnectedToExistingRoad: message = "Cannot place road segment because it is not connected to an existing road segment."; break;
                default: message = "Road build segment status not recognised: " + status; break;
            }

            this.ErrorRaisedEvent?.Invoke(new ErrorDetails(message));
        }

        private void LoadHexes()
        {

        }

        private void PlayDevelopmentCard(DevelopmentCard developmentCard)
        {
            this.cardPlayedThisTurn = true;
            this.cardsPlayed.Add(developmentCard);
        }

        private bool PlayerHasJustBuiltTheLargestArmy(out Guid previousPlayerId)
        {
            previousPlayerId = Guid.Empty;
            var playerWithMostKnightCards = this.DeterminePlayerWithMostKnightCards();
            if (playerWithMostKnightCards == this.currentPlayer && this.playerWithLargestArmy != this.currentPlayer)
            {
                previousPlayerId = Guid.Empty;

                if (this.playerWithLargestArmy != null)
                {
                    this.playerWithLargestArmy.HasLargestArmy = false;
                    previousPlayerId = this.playerWithLargestArmy.Id;
                }

                this.playerWithLargestArmy = playerWithMostKnightCards;
                this.playerWithLargestArmy.HasLargestArmy = true;
                return true;
            }

            return false;
        }

        private bool PlayerHasJustBuiltTheLongestRoad(out Guid previousPlayerId)
        {
            previousPlayerId = Guid.Empty;
            if (this.currentPlayer.PlacedRoadSegments < 5)
            {
                return false;
            }

            Guid longestRoadPlayerId = Guid.Empty;
            uint[] road = null;
            if (this.gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road) && road.Length > 5)
            {
                var longestRoadPlayer = this.playersById[longestRoadPlayerId];
                if (longestRoadPlayer == this.currentPlayer && this.playerWithLongestRoad != longestRoadPlayer)
                {
                    previousPlayerId = Guid.Empty;

                    if (this.playerWithLongestRoad != null)
                    {
                        this.playerWithLongestRoad.HasLongestRoad = false;
                        previousPlayerId = this.playerWithLongestRoad.Id;
                    }

                    this.playerWithLongestRoad = longestRoadPlayer;
                    this.playerWithLongestRoad.HasLongestRoad = true;
                    return true;
                }
            }

            return false;
        }

        private bool PlayerIdsIsEmptyOrOnlyContainsMainPlayer(Guid[] playerIds)
        {
            return playerIds == null || playerIds.Length == 0 ||
                   (playerIds.Length == 1 && playerIds[0] == this.mainPlayer.Id);
        }

        private void PlayKnightDevelopmentCard(KnightDevelopmentCard card, uint newRobberHex)
        {
            this.PlayDevelopmentCard(card);
            this.currentPlayer.PlayDevelopmentCard(card);
            this.robberHex = newRobberHex;
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

        private bool VerifyBuildCityRequest(uint location)
        {
            if (this.GamePhase == GamePhases.GameOver)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Game is over."));
                return false;
            }

            if (this.currentPlayer.RemainingCities == 0)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. All cities already built."));
                return false;
            }

            if (this.currentPlayer.GrainCount < Constants.GrainForBuildingCity && this.currentPlayer.OreCount < Constants.OreForBuildingCity)
            {
                var missingGrainCount = (Constants.GrainForBuildingCity - this.currentPlayer.GrainCount);
                var missingOreCount = (Constants.OreForBuildingCity - this.currentPlayer.OreCount);
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Missing " + missingGrainCount + " grain and " + missingOreCount + " ore."));
                return false;
            }

            if (this.currentPlayer.GrainCount < Constants.GrainForBuildingCity)
            {
                var missingGrainCount = (Constants.GrainForBuildingCity - this.currentPlayer.GrainCount);
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Missing " + missingGrainCount + " grain."));
                return false;
            }

            if (this.currentPlayer.OreCount < Constants.OreForBuildingCity)
            {
                var missingOreCount = (Constants.OreForBuildingCity - this.currentPlayer.OreCount);
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Missing " + missingOreCount + " ore."));
                return false;
            }

            var placeCityResults = this.gameBoard.CanPlaceCity(this.currentPlayer.Id, location);

            if (placeCityResults.Status == GameBoard.VerificationStatus.LocationForCityIsInvalid)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Location " + location + " is outside of board range (0 - 53)."));
                return false;
            }

            if (placeCityResults.Status == GameBoard.VerificationStatus.LocationIsNotOwned)
            {
                var player = this.playersById[placeCityResults.PlayerId];
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. Location " + location + " is owned by player '" + player.Name + "'."));
                return false;
            }

            if (placeCityResults.Status == GameBoard.VerificationStatus.LocationIsNotSettled)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. No settlement at location " + location + "."));
                return false;
            }

            if (placeCityResults.Status == GameBoard.VerificationStatus.LocationIsAlreadyCity)
            {
                var player = this.playersById[placeCityResults.PlayerId];
                if (player == this.currentPlayer)
                {
                    this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. There is already a city at location " + location + " that belongs to you."));
                }
                else
                {
                    this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build city. There is already a city at location " + location + " belonging to '" + player.Name + "'."));
                }

                return false;
            }

            return true;
        }

        private bool VerifyBuildRoadSegmentRequest(uint roadStartLocation, uint roadEndLocation)
        {
            if (this.GamePhase == GamePhases.GameOver)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Game is over."));
                return false;
            }

            if (this.CanBuildRoadSegment() != BuildStatuses.Successful)
            {
                this.TryRaiseRoadSegmentBuildingError();
                return false;
            }

            if (!this.VerifyRoadSegmentPlacing(roadStartLocation, roadEndLocation))
            {
                return false;
            }

            return true;
        }

        private bool VerifyBuildSettlementRequest(uint settlementLocation)
        {
            if (this.GamePhase == GamePhases.GameOver)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build settlement. Game is over."));
                return false;
            }

            return this.VerifySettlementBuilding() && this.VerifySettlementPlacing(settlementLocation);
        }

        private bool VerifyBuyDevelopmentCardRequest()
        {
            if (this.GamePhase == GamePhases.GameOver)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Game is over."));
                return false;
            }

            if (!this.developmentCardHolder.HasCards)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. No more cards available"));
                return false;
            }

            if (this.currentPlayer.GrainCount < 1 && this.currentPlayer.OreCount < 1 && this.currentPlayer.WoolCount < 1)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain and 1 ore and 1 wool."));
                return false;
            }

            if (this.currentPlayer.GrainCount < 1 && this.currentPlayer.OreCount < 1)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain and 1 ore."));
                return false;
            }

            if (this.currentPlayer.GrainCount < 1 && this.currentPlayer.WoolCount < 1)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain and 1 wool."));
                return false;
            }

            if (this.currentPlayer.OreCount < 1 && this.currentPlayer.WoolCount < 1)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 ore and 1 wool."));
                return false;
            }

            if (this.currentPlayer.GrainCount < 1)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 grain."));
                return false;
            }

            if (this.currentPlayer.OreCount < 1)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 ore."));
                return false;
            }

            if (this.currentPlayer.WoolCount < 1)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot buy development card. Missing 1 wool."));
                return false;
            }

            return true;
        }

        private bool VerifyPlayerForResourceTransactionWhenUsingKnightCard(uint newRobberHex, Guid playerId)
        {
            var playerIdsOnHex = new List<Guid>(this.gameBoard.GetPlayersForHex(newRobberHex));
            if (!playerIdsOnHex.Contains(playerId))
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Player Id (" + playerId + ") does not match with any players on hex " + newRobberHex + "."));
                return false;
            }

            return true;
        }

        private bool VerifyParametersForUsingDevelopmentCard(GameToken turnToken, DevelopmentCard developmentCard, String shortCardType)
        {
            if (!this.VerifyTurnToken(turnToken))
            {
                return false;
            }

            if (this.GamePhase == GamePhases.GameOver)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot use " + shortCardType + " card. Game is over."));
                return false;
            }

            if (developmentCard == null)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Development card parameter is null."));
                return false;
            }

            if (this.cardsPurchasedThisTurn.Contains(developmentCard))
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

        private bool VerifyPlacementOfRobber(uint newRobberHex)
        {
            if (!this.gameBoard.CanPlaceRobber(newRobberHex))
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot move robber to hex " + newRobberHex + " because it is out of bounds (0.. " + (GameBoard.StandardBoardHexCount - 1) + ")."));
                return false;
            }

            if (newRobberHex == this.robberHex)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot place robber back on present hex (" + this.robberHex + ")."));
                return false;
            }

            return true;
        }

        private bool VerifyRoadSegmentPlacing(uint settlementLocation, uint roadEndLocation)
        {
            var placeRoadStatus = this.gameBoard.CanPlaceRoadSegment(this.currentPlayer.Id, settlementLocation, roadEndLocation);
            //return this.VerifyRoadSegmentPlacing(placeRoadStatus, settlementLocation, roadEndLocation);
            return false;
        }

        private bool VerifyRoadSegmentPlacing(GameBoard.VerificationResults verificationResults, uint settlementLocation, uint roadEndLocation)
        {
            if (verificationResults.Status == GameBoard.VerificationStatus.RoadIsOffBoard)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Locations " + settlementLocation + " and/or " + roadEndLocation + " are outside of board range (0 - 53)."));
                return false;
            }

            if (verificationResults.Status == GameBoard.VerificationStatus.NoDirectConnection)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. No direct connection between locations [" + settlementLocation + ", " + roadEndLocation + "]."));
                return false;
            }

            if (verificationResults.Status == GameBoard.VerificationStatus.RoadIsOccupied)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Road segment between " + settlementLocation + " and " + roadEndLocation + " already exists."));
                return false;
            }

            if (verificationResults.Status == GameBoard.VerificationStatus.RoadNotConnectedToExistingRoad)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build road segment. Road segment [" + settlementLocation + ", " + roadEndLocation + "] not connected to existing road segment."));
                return false;
            }

            return true;
        }

        private bool VerifySettlementBuilding()
        {
            if (this.mainPlayer.RemainingSettlements == 0)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot build settlement. All settlements already built."));
                return false;
            }

            String message = null;
            if (this.mainPlayer.BrickCount == 0)
            {
                message += "1 brick and ";
            }

            if (this.mainPlayer.GrainCount == 0)
            {
                message += "1 grain and ";
            }

            if (this.mainPlayer.LumberCount == 0)
            {
                message += "1 lumber and ";
            }

            if (this.mainPlayer.WoolCount == 0)
            {
                message += "1 wool and ";
            }

            if (message != null)
            {
                message = message.Substring(0, message.Length - " and ".Length);
                message += ".";
                message = "Cannot build settlement. Missing " + message;
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails(message));
                return false;
            }

            return true;
        }

        private bool VerifySettlementPlacing(uint settlementLocation)
        {
            var verificationResults = this.gameBoard.CanPlaceSettlement(this.mainPlayer.Id, settlementLocation);
            return this.VerifySettlementPlacing(verificationResults, settlementLocation);
        }

        private bool VerifySettlementPlacing(GameBoard.VerificationResults verificationResults, uint settlementLocation)
        {
            if (verificationResults.Status == GameBoard.VerificationStatus.LocationForSettlementIsInvalid)
            {
                this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Location " + settlementLocation + " is outside of board range (0 - 53)."));
                return false;
            }

            if (verificationResults.Status == GameBoard.VerificationStatus.TooCloseToSettlement)
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

                return false;
            }

            if (verificationResults.Status == GameBoard.VerificationStatus.LocationIsOccupied)
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

                return false;
            }

            if (verificationResults.Status == GameBoard.VerificationStatus.SettlementNotConnectedToExistingRoad)
            {
                this.ErrorRaisedEvent(new ErrorDetails("Cannot build settlement. Location " + verificationResults.LocationIndex + " not connected to existing road."));
                return false;
            }

            return true;
        }

        private bool VerifyStartingInfrastructurePlacementRequest(uint settlementLocation, uint roadEndLocation)
        {
            var verificationResults = this.gameBoard.CanPlaceStartingInfrastructure(this.mainPlayer.Id, settlementLocation, roadEndLocation);
            return this.VerifySettlementPlacing(verificationResults, settlementLocation) && this.VerifyRoadSegmentPlacing(verificationResults, settlementLocation, roadEndLocation);
        }

        private bool VerifyTradeWithBank(int receivingCount, int resourceCount, ResourceTypes givingResourceType, ResourceTypes receivingResourceType)
        {
            if (this.GamePhase == GamePhases.GameOver)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot trade with bank. Game is over."));
                return false;
            }

            if (receivingCount <= 0)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Cannot complete trade with bank: Receiving count must be positive. Was " + receivingCount + "."));
                return false;
            }

            if (resourceCount < receivingCount * 4)
            {
                var errorMessage = "Cannot complete trade with bank: Need to pay " + (receivingCount * 4) + " " + givingResourceType.ToString().ToLower() + " for " + receivingCount + " " + receivingResourceType.ToString().ToLower() + ". Only paying " + resourceCount + ".";
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails(errorMessage));
                return false;
            }

            return true;
        }

        internal void ApplyPlayerAction(PlayerAction action)
        {
            this.actionRequests.Enqueue(action);
        }

        private bool VerifyTurnToken(GameToken turnToken)
        {
            if (turnToken == null)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token is null."));
                return false;
            }

            if (turnToken != this.currentTurnToken)
            {
                this.ErrorRaisedEvent?.Invoke(new ErrorDetails("Turn token not recognised."));
                return false;
            }

            return true;
        }

        private ConcurrentQueue<PlayerAction> actionRequests = new ConcurrentQueue<PlayerAction>();
        private void GameLoop()
        {
            this.playerIndex = -1;
            this.StartTurn();
            var pauseCount = 40;

            while (true)
            {
                Thread.Sleep(50);
                if (this.GamePhase == GamePhases.Quit)
                    return;

                var gotPlayerAction = this.actionRequests.TryDequeue(out var playerAction);

                if ((pauseCount == 0) || 
                    (gotPlayerAction && playerAction is EndOfTurnAction))
                {
                    this.StartTurn();
                    pauseCount = 40;
                    continue;
                }

                pauseCount--;

                if (!gotPlayerAction)
                    continue;

                // Player action to process
                this.ProcessPlayerAction(playerAction);
            }
        }

        private void StartTurn()
        {
            this.ChangeToNextPlayerTurn();
            this.currentTurnToken = new GameToken();
            this.StartPlayerTurnEvent?.Invoke(this.currentTurnToken);
            this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
            this.DiceRollEvent?.Invoke(this.currentPlayer.Id, this.dice1, this.dice2);
            
            var resourceRoll = this.dice1 + this.dice2;
            if (resourceRoll != 7)
            {
                this.CollectResourcesAtStartOfTurn(resourceRoll);
            }
            else
            {

            }
        }

        private void ProcessPlayerAction(PlayerAction playerAction)
        {
            if (playerAction is MakeDirectTradeOfferAction)
            {
                foreach (var kv in this.playersById.Where(k => k.Key != playerAction.InitiatingPlayerId).ToList())
                {
                }
            }
        }
        #endregion
    }
}
