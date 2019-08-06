
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.Enums;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.PlayerActions;

    public class GameManager
    {
        #region Fields
        private readonly ActionManager actionManager;
        private readonly ConcurrentQueue<PlayerAction> actionRequests = new ConcurrentQueue<PlayerAction>();
        private readonly ISet<DevelopmentCard> cardsBoughtThisTurn = new HashSet<DevelopmentCard>();
        private readonly Dictionary<Guid, ChooseLostResourcesEvent> chooseLostResourcesEventByPlayerId = new Dictionary<Guid, ChooseLostResourcesEvent>();
        private readonly IDevelopmentCardHolder developmentCardHolder;
        private readonly EventRaiser eventRaiser;
        private readonly GameBoard gameBoard;
        private readonly ILog log = new Log();
        private readonly IActionLog actionLog = null;
        private readonly INumberGenerator numberGenerator;
        private readonly IPlayerFactory playerFactory;

        private IPlayer currentPlayer;
        private Func<Guid> idGenerator;
        private bool isGameSetup = true;
        private bool developmentCardPlayerThisTurn;
        private Guid[] playerIdsInRobberHex;
        private int playerIndex;
        private IDictionary<Guid, IPlayer> playersById;
        private IPlayer playerWithLargestArmy;
        private IPlayer playerWithLongestRoad;
        private uint robberHex = 0;

        private IPlayer[] players;
        private IGameTimer turnTimer;

        // TODO: Review this - cleaner way to do this?
        private Tuple<Guid, ResourceClutch> initialDirectTradeOffer;
        private Dictionary<Guid, ResourceClutch> answeringDirectTradeOffers = new Dictionary<Guid, ResourceClutch>();

        // Only needed for scenario running?
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken cancellationToken;
        #endregion

        #region Construction
        public GameManager(INumberGenerator numberGenerator, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder, IPlayerFactory playerFactory)
        {
            this.numberGenerator = numberGenerator;
            this.gameBoard = gameBoard;
            this.developmentCardHolder = developmentCardHolder;
            this.turnTimer = new GameServerTimer();
            this.idGenerator = () => { return Guid.NewGuid(); };
            this.eventRaiser = new EventRaiser();
            this.actionManager = new ActionManager();
            this.playerFactory = playerFactory;
        }
        #endregion

        #region Properties
        public bool IsFinished { get; private set; }
        #endregion

        #region Methods
        // This is a scenario only method
        public void AddResourcesToPlayer(string playerName, ResourceClutch value)
        {
            this.players
                .Where(p => p.Name == playerName)
                .FirstOrDefault()
                ?.AddResources(value);
        }

        public void JoinGame(string playerName, GameController gameController)
        {
            var player = this.playerFactory.CreatePlayer(playerName, this.idGenerator.Invoke());
            this.players[this.playerIndex++] = player;

            this.eventRaiser.AddEventHandler(player.Id, gameController.GameEventHandler);
            gameController.PlayerActionEvent += this.PlayerActionEventHandler;

            this.RaiseEvent(new GameJoinedEvent(player.Id), player);
        }

        public void LaunchGame(GameOptions gameOptions = null)
        {
            if (gameOptions == null)
                gameOptions = new GameOptions();

            this.playerIndex = 0;
            this.players = new IPlayer[gameOptions.MaxPlayers + gameOptions.MaxAIPlayers];
        }

        public void Quit()
        {
            this.eventRaiser.CanSendEvents = false;
            this.cancellationTokenSource.Cancel();
        }

        public void SaveLog(string filePath) => this.log.WriteToFile(filePath);

        public void SetIdGenerator(Func<Guid> idGenerator)
        {
            if (idGenerator != null)
                this.idGenerator = idGenerator;
        }

        public void SetTurnTimer(IGameTimer turnTimer)
        {
            if (turnTimer != null)
                this.turnTimer = turnTimer;
        }

        public Task StartGameAsync()
        {
            this.cancellationToken = this.cancellationTokenSource.Token;

            // Launch server processing on separate thread
            return Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Name = "Local Game Server";
                try
                {
                    this.playersById = this.players.ToDictionary(p => p.Id, p => p);

                    var playerIdsByName = this.players.ToDictionary(p => p.Name, p => p.Id);
                    this.RaiseEvent(new PlayerSetupEvent(playerIdsByName));

                    var gameBoardSetup = new GameBoardSetup(this.gameBoard);
                    this.RaiseEvent(new InitialBoardSetupEvent(gameBoardSetup));

                    this.players = PlayerTurnOrderCreator.Create(this.players, this.numberGenerator);
                    var playerIds = this.players.Select(player => player.Id).ToArray();
                    this.RaiseEvent(new PlayerOrderEvent(playerIds));

                    try
                    {
                        this.GameSetup();
                        this.WaitForGameStartConfirmationFromPlayers();
                        this.MainGameLoop();
                        this.CaretakerLoop();
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    this.log.Add($"ERROR: {e.Message}: {e.StackTrace}");
                    //TODO: Send game error message to clients
                    throw e;
                }
                finally
                {
                    this.IsFinished = true;
                }
            });
        }

        private void CaretakerLoop()
        {
            while (true)
            {
                var requestStateAction = this.WaitForRequestStateAction();
                this.ProcessRequestStateAction(requestStateAction);
            }
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

        private void StartTurnWithResourceCollection(uint dice1, uint dice2)
        {
            var resourcesCollectedByPlayerId = this.gameBoard.GetResourcesForRoll(dice1 + dice2);
            foreach (var keyValuePair in resourcesCollectedByPlayerId)
            {
                if (this.playersById.TryGetValue(keyValuePair.Key, out var player))
                {
                    foreach (var resourceCollection in keyValuePair.Value)
                        player.AddResources(resourceCollection.Resources);
                }
            }

            var startPlayerTurnEvent = new StartTurnEvent(this.currentPlayer.Id, dice1, dice2, resourcesCollectedByPlayerId);
            this.RaiseEvent(startPlayerTurnEvent);
        }

        private void GameSetup()
        {
            // Place first settlement
            for (int i = 0; i < this.players.Length; i++)
            {
                this.GameSetupForPlayer(this.players[i]);
            }

            // Place second settlement
            for (int i = this.players.Length - 1; i >= 0; i--)
            {
                this.GameSetupForPlayer(this.players[i]);
            }

            this.isGameSetup = false;
        }

        private void GameSetupForPlayer(IPlayer player)
        {
            var placeSetupInfrastructureEvent = new PlaceSetupInfrastructureEvent();
            this.actionManager.SetExpectedActionsForPlayer(player.Id, 
                typeof(PlaceSetupInfrastructureAction),
                typeof(QuitGameAction));
            this.RaiseEvent(placeSetupInfrastructureEvent, player);
            var playerAction = this.WaitForPlayerAction();
            this.turnTimer.Reset();
            this.ProcessPlayerAction(playerAction);
        }

        private string GetErrorMessage(Type actualAction, HashSet<Type> expectedActions)
        {
            if (expectedActions == null || expectedActions.Count == 0)
                return $"Received action type {actualAction.Name}. No action expected";
            
            var expectedActionsNames = expectedActions.Select(
                    expectedAction => expectedAction.Name).ToList();

            expectedActionsNames.Sort();

            return $"Received action type {actualAction.Name}. Expected one of {string.Join(", ", expectedActionsNames)}";
        }

        private IPlayer GetWinningPlayer()
        {
            return this.players.FirstOrDefault(player => player.VictoryPoints >= 10);
        }

        private void MainGameLoop()
        {
            this.playerIndex = -1;
            this.StartTurn();
            this.turnTimer.Reset();
            var isFinished = false;
            while (!isFinished)
            {
                var playerAction = this.WaitForPlayerAction();
                this.turnTimer.Reset();
                isFinished = this.ProcessPlayerAction(playerAction);
            }
        }

        private void PlaceInfrastructure(IPlayer player, uint settlementLocation, uint roadEndLocation)
        {
            try
            {
                this.gameBoard.PlaceStartingInfrastructure(player.Id, settlementLocation, roadEndLocation);
                player.PlaceStartingInfrastructure();
            }
            catch (Exception e)
            {
                // TODO: Send back message to user
            }
        }

        private void PlayerActionEventHandler(PlayerAction playerAction)
        {
            // Leave all validation and processing to the game server thread
            this.actionRequests.Enqueue(playerAction);
        }

        private IEnumerable<IPlayer> PlayersExcept(params Guid[] playerIds) => this.playersById.Select(kv => kv.Value).Where(player => !playerIds.Contains(player.Id));

        private void ProcessAcceptDirectTradeAction(AcceptDirectTradeAction acceptDirectTradeAction)
        {
            var sellingResources = this.answeringDirectTradeOffers[acceptDirectTradeAction.SellerId];
            var buyingResources = this.initialDirectTradeOffer.Item2;
            var buyingPlayer = this.playersById[this.initialDirectTradeOffer.Item1];
            var sellingPlayer = this.playersById[acceptDirectTradeAction.SellerId];

            buyingPlayer.AddResources(buyingResources);
            sellingPlayer.RemoveResources(buyingResources);
            buyingPlayer.RemoveResources(sellingResources);
            sellingPlayer.AddResources(sellingResources);

            var acceptTradeEvent = new AcceptTradeEvent(
                this.initialDirectTradeOffer.Item1,
                buyingResources,
                acceptDirectTradeAction.SellerId,
                sellingResources);

            this.RaiseEvent(acceptTradeEvent, this.currentPlayer);
            this.RaiseEvent(acceptTradeEvent, this.PlayersExcept(this.currentPlayer.Id));
        }

        private void ProcessAnswerDirectTradeOfferAction(AnswerDirectTradeOfferAction answerDirectTradeOfferAction)
        {
            var answerDirectTradeOfferEvent = new AnswerDirectTradeOfferEvent(
                answerDirectTradeOfferAction.InitiatingPlayerId, answerDirectTradeOfferAction.WantedResources);

            this.answeringDirectTradeOffers.Add(
                answerDirectTradeOfferAction.InitiatingPlayerId,
                answerDirectTradeOfferAction.WantedResources);

            // Initial player gets chance to confirm. 
            this.RaiseEvent(
                answerDirectTradeOfferEvent,
                this.playersById[answerDirectTradeOfferAction.InitialPlayerId]);

            // Other two players gets informational event
            var informationalAnswerDirectTradeOfferEvent = new AnswerDirectTradeOfferEvent(
                answerDirectTradeOfferAction.InitiatingPlayerId, answerDirectTradeOfferAction.WantedResources);

            var otherPlayers = this.PlayersExcept(
                    answerDirectTradeOfferAction.InitiatingPlayerId,
                    answerDirectTradeOfferAction.InitialPlayerId);

            this.RaiseEvent(informationalAnswerDirectTradeOfferEvent, otherPlayers);
        }

        private void ProcessBuyDevelopmentCardAction()
        {
            if (!this.currentPlayer.CanBuyDevelopmentCard)
            {
                this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "921", "Not enough resources for buying development card"), this.currentPlayer);
            }
            else
            {
                this.currentPlayer.BuyDevelopmentCard();
                this.developmentCardHolder.TryGetNextCard(out var card);
                this.currentPlayer.HeldCards.Add(card);
                this.cardsBoughtThisTurn.Add(card);
                this.RaiseEvent(new DevelopmentCardBoughtEvent(this.currentPlayer.Id, card.Type));
                this.RaiseEvent(new DevelopmentCardBoughtEvent(this.currentPlayer.Id), this.PlayersExcept(this.currentPlayer.Id));
            }
        }

        private void ProcessMakeDirectTradeOfferAction(MakeDirectTradeOfferAction makeDirectTradeOfferAction)
        {
            this.initialDirectTradeOffer = new Tuple<Guid, ResourceClutch>(
                makeDirectTradeOfferAction.InitiatingPlayerId,
                makeDirectTradeOfferAction.WantedResources);

            this.actionManager.AddExpectedActionsForPlayer(this.currentPlayer.Id,
                typeof(AcceptDirectTradeAction));

            var makeDirectTradeOfferEvent = new MakeDirectTradeOfferEvent(
                makeDirectTradeOfferAction.InitiatingPlayerId, makeDirectTradeOfferAction.WantedResources);

            var otherPlayers = this.PlayersExcept(makeDirectTradeOfferAction.InitiatingPlayerId).ToList();
            otherPlayers.ForEach(player => {
                this.actionManager.SetExpectedActionsForPlayer(player.Id, typeof(AnswerDirectTradeOfferAction));
                this.RaiseEvent(makeDirectTradeOfferEvent, player);
            });
        }

        private void ProcessNewRobberPlacement()
        {
            this.playerIdsInRobberHex = this.gameBoard.GetPlayersForHex(this.robberHex);
            if (this.playerIdsInRobberHex != null)
            {
                if (this.playerIdsInRobberHex.Length == 1)
                {
                    if (this.playerIdsInRobberHex[0] != this.currentPlayer.Id)
                    {
                        var player = this.playersById[this.playerIdsInRobberHex[0]];

                        var resourceIndex = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(player.Resources.Count);
                        var robbedResource = player.LoseResourceAtIndex(resourceIndex);
                        this.currentPlayer.AddResources(robbedResource);
                        this.RaiseEvent(new ResourcesGainedEvent(robbedResource), this.currentPlayer);
                        this.RaiseEvent(new ResourcesLostEvent(robbedResource, this.currentPlayer.Id, ResourcesLostEvent.ReasonTypes.Robbed), player);
                        this.RaiseEvent(new ResourcesLostEvent(robbedResource, this.currentPlayer.Id, ResourcesLostEvent.ReasonTypes.Witness),
                            this.PlayersExcept(this.currentPlayer.Id, player.Id));
                    }
                }
                else
                {
                    var resourceCountsByPlayerId = this.playerIdsInRobberHex.Where(playerId => playerId != this.currentPlayer.Id).ToDictionary(playerId => playerId, playerId => this.playersById[playerId].Resources.Count);
                    this.RaiseEvent(new RobbingChoicesEvent(this.currentPlayer.Id, resourceCountsByPlayerId));

                    this.actionManager.SetExpectedActionsForPlayer(this.currentPlayer.Id, typeof(SelectResourceFromPlayerAction));
                    this.ProcessPlayerAction(this.WaitForPlayerAction());

                    this.SetStandardExpectedActionsForCurrentPlayer();
                }
            }
        }

        private bool ProcessPlaceCityAction(PlaceCityAction placeCityAction)
        {
            try
            {
                PlayerPlacementVerificationStates verificationState = this.currentPlayer.CanPlaceCity;
                if (verificationState == PlayerPlacementVerificationStates.NotEnoughResources)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "913", "Not enough resources for placing city"),
                        this.currentPlayer);
                    return false;
                }

                this.gameBoard.PlaceCity(this.currentPlayer.Id,
                    placeCityAction.CityLocation);
                this.currentPlayer.PlaceCity();

                this.RaiseEvent(new CityPlacedEvent(this.currentPlayer.Id,
                    placeCityAction.CityLocation));

                var winningPlayer = this.GetWinningPlayer();
                if (winningPlayer != null)
                {
                    this.RaiseEvent(new GameWinEvent(winningPlayer.Id, winningPlayer.VictoryPoints));
                    return true;
                }
            }
            catch (GameBoard.PlacementException pe)
            {
                switch (pe.VerificationStatus)
                {
                    case GameBoard.VerificationStatus.LocationIsNotOwned:
                    {
                        var occupyingPlayer = this.playersById[pe.OtherPlayerId];
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "908", $"Location ({placeCityAction.CityLocation}) already occupied by {occupyingPlayer.Name}"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.LocationIsAlreadyCity:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "908", $"Location ({placeCityAction.CityLocation}) already occupied by you"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.LocationIsNotSettled:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "914", $"Location ({placeCityAction.CityLocation}) not an settlement"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.LocationForCityIsInvalid:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "915", $"Location ({placeCityAction.CityLocation}) is invalid"),
                            this.currentPlayer);
                        break;
                    }
                    default:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "998", $"Unknown error"),
                            this.currentPlayer);
                        break;
                    }
                }
            }

            return false;
        }

        private bool ProcessPlaceRoadSegmentAction(PlaceRoadSegmentAction placeRoadSegmentAction)
        {
            try
            {
                PlayerPlacementVerificationStates verificationState = this.currentPlayer.CanPlaceRoadSegment;
                if (verificationState == PlayerPlacementVerificationStates.NoRoadSegments)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "905", "No road segments to place"),
                        this.currentPlayer);
                    return false;
                }
                else if (verificationState == PlayerPlacementVerificationStates.NotEnoughResources)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "906", "Not enough resources for placing road segment"),
                        this.currentPlayer);
                    return false;
                }

                this.gameBoard.PlaceRoadSegment(this.currentPlayer.Id,
                    placeRoadSegmentAction.StartLocation,
                    placeRoadSegmentAction.EndLocation);
                this.currentPlayer.PlaceRoadSegment();

                this.RaiseEvent(new RoadSegmentPlacedEvent(this.currentPlayer.Id, 
                    placeRoadSegmentAction.StartLocation,
                    placeRoadSegmentAction.EndLocation));

                if (this.currentPlayer.PlacedRoadSegments >= 5)
                {
                    if (this.gameBoard.TryGetLongestRoadDetails(out var playerId, out var locations) && locations.Length > 5)
                    {
                        if (playerId == this.currentPlayer.Id && (this.playerWithLongestRoad == null || this.playerWithLongestRoad != this.currentPlayer))
                        {
                            Guid? previousPlayerId = null;
                            if (this.playerWithLongestRoad != null)
                            {
                                this.playerWithLongestRoad.HasLongestRoad = false;
                                previousPlayerId = this.playerWithLongestRoad.Id;
                            }

                            this.playerWithLongestRoad = this.currentPlayer;
                            this.playerWithLongestRoad.HasLongestRoad = true;
                            this.RaiseEvent(new LongestRoadBuiltEvent(this.playerWithLongestRoad.Id, locations, previousPlayerId));
                        }

                        if (this.currentPlayer.VictoryPoints >= 10)
                        {
                            this.RaiseEvent(new GameWinEvent(this.currentPlayer.Id, this.currentPlayer.VictoryPoints));
                            return true;
                        }
                    }
                }
            }
            catch (GameBoard.PlacementException pe)
            {
                switch (pe.VerificationStatus)
                {
                    case GameBoard.VerificationStatus.RoadIsOffBoard:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "903", $"Locations ({placeRoadSegmentAction.StartLocation}, {placeRoadSegmentAction.EndLocation}) invalid for placing road segment"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.NoDirectConnection:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "904", $"Locations ({placeRoadSegmentAction.StartLocation}, {placeRoadSegmentAction.EndLocation}) not connected when placing road segment"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.RoadIsOccupied:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "907", $"Cannot place road segment on existing road segment ({placeRoadSegmentAction.StartLocation}, {placeRoadSegmentAction.EndLocation})"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.RoadNotConnectedToExistingRoad:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "910", $"Cannot place road segment because locations ({placeRoadSegmentAction.StartLocation}, {placeRoadSegmentAction.EndLocation}) are not connected to existing road"),
                            this.currentPlayer);
                        break;
                    }
                    default:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "998", $"Unknown error"),
                            this.currentPlayer);
                        break;
                    }
                }
            }

            return false;
        }

        private void ProcessPlaceRobberAction(PlaceRobberAction placeRobberAction)
        {
            if (this.robberHex == placeRobberAction.Hex)
            {
                this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "918", "New robber hex cannot be the same as previous robber hex"),
                    this.currentPlayer);
                return;
            }

            this.actionManager.SetExpectedActionsForPlayer(this.currentPlayer.Id, null);
            this.robberHex = placeRobberAction.Hex;
            this.RaiseEvent(new RobberPlacedEvent(this.currentPlayer.Id, this.robberHex));

            this.ProcessNewRobberPlacement();
        }

        private bool ProcessPlaceSettlementAction(PlaceSettlementAction placeSettlementAction)
        {
            try
            {
                PlayerPlacementVerificationStates verificationState = this.currentPlayer.CanPlaceSettlement;
                if (verificationState == PlayerPlacementVerificationStates.NoSettlements) { 
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "911", "No settlements to place"),
                        this.currentPlayer);
                    return false;
                }
                else if (verificationState == PlayerPlacementVerificationStates.NotEnoughResources)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "912", "Not enough resources for placing settlement"),
                        this.currentPlayer);
                    return false;
                }

                this.gameBoard.PlaceSettlement(this.currentPlayer.Id,
                    placeSettlementAction.SettlementLocation);
                this.currentPlayer.PlaceSettlement();

                this.RaiseEvent(new SettlementPlacedEvent(this.currentPlayer.Id,
                    placeSettlementAction.SettlementLocation));

                var winningPlayer = this.GetWinningPlayer();
                if (winningPlayer != null)
                {
                    this.RaiseEvent(new GameWinEvent(winningPlayer.Id, winningPlayer.VictoryPoints));
                    return true;
                }
            }
            catch (GameBoard.PlacementException pe)
            {
                switch (pe.VerificationStatus)
                {
                    case GameBoard.VerificationStatus.LocationIsOccupied:
                    {
                        var occupyingPlayer = this.playersById[pe.OtherPlayerId];
                        var occupyingPlayerName = occupyingPlayer == this.currentPlayer ? "you" : occupyingPlayer.Name;
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "908", $"Location ({placeSettlementAction.SettlementLocation}) already occupied by {occupyingPlayerName}"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.SettlementNotConnectedToExistingRoad:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "909", $"Location ({placeSettlementAction.SettlementLocation}) is not connected to your road system"),
                            this.currentPlayer);
                        break;
                    }
                    case GameBoard.VerificationStatus.LocationForSettlementIsInvalid:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "915", $"Location ({placeSettlementAction.SettlementLocation}) is invalid"),
                            this.currentPlayer);
                        break;
                    }
                    default:
                    {
                        this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "998", $"Unknown error"),
                            this.currentPlayer);
                        break;
                    }
                }
            }

            return false;
        }

        private void ProcessPlaceSetupInfrastructureAction(PlaceSetupInfrastructureAction placeSetupInfrastructureAction)
        {
            var player = this.playersById[placeSetupInfrastructureAction.InitiatingPlayerId];
            this.actionManager.SetExpectedActionsForPlayer(player.Id, null);
            var settlementLocation = placeSetupInfrastructureAction.SettlementLocation;
            var roadEndLocation = placeSetupInfrastructureAction.RoadEndLocation;
            this.PlaceInfrastructure(player, settlementLocation, roadEndLocation);
            this.RaiseEvent(new InfrastructurePlacedEvent(player.Id, settlementLocation, roadEndLocation));
        }

        private bool ProcessPlayerAction(PlayerAction playerAction)
        {
            if (playerAction is AcceptDirectTradeAction acceptDirectTradeAction)
            {
                this.ProcessAcceptDirectTradeAction(acceptDirectTradeAction);
                return false;
            }

            if (playerAction is AnswerDirectTradeOfferAction answerDirectTradeOfferAction)
            {
                this.ProcessAnswerDirectTradeOfferAction(answerDirectTradeOfferAction);
                return false;
            }

            if (playerAction is BuyDevelopmentCardAction buyDevelopmentCardAction)
            {
                this.ProcessBuyDevelopmentCardAction();
                return false;
            }

            if (playerAction is EndOfTurnAction)
            {
                this.StartTurn();
                return false;
            }

            if (playerAction is MakeDirectTradeOfferAction makeDirectTradeOfferAction)
            {
                this.ProcessMakeDirectTradeOfferAction(makeDirectTradeOfferAction);
                return false;
            }

            if (playerAction is PlaceCityAction placeCityAction)
            {
                return this.ProcessPlaceCityAction(placeCityAction);
            }

            if (playerAction is PlaceSetupInfrastructureAction placeSetupInfrastructureAction)
            {
                this.ProcessPlaceSetupInfrastructureAction(placeSetupInfrastructureAction);
                return false;
            }

            if (playerAction is PlaceRoadSegmentAction placeRoadSegmentAction)
            {
                return this.ProcessPlaceRoadSegmentAction(placeRoadSegmentAction);
            }

            if (playerAction is PlayKnightCardAction playKnightCardAction)
            {
                return this.ProcessPlayKnightCardAction(playKnightCardAction);
            }

            if (playerAction is PlayMonopolyCardAction playMonopolyCardAction)
            {
                if (this.developmentCardPlayerThisTurn)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "922", "Cannot play more than one development card per turn"),
                        this.currentPlayer);
                    return false;
                }

                DevelopmentCard card = null;
                if ((card = this.currentPlayer.HeldCards.FirstOrDefault(c => c.Type == DevelopmentCardTypes.Monopoly &&
                    !this.cardsBoughtThisTurn.Contains(c))) == null)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "920", "No Monopoly card owned that can be played this turn"),
                        this.currentPlayer);
                    return false;
                }

                return false;
            }

            if (playerAction is PlayRoadBuildingCardAction playRoadBuildingCardAction)
            {
                return this.ProcessPlayRoadBuildingCardAction(playRoadBuildingCardAction); 
            }

            if (playerAction is PlayYearOfPlentyCardAction playYearOfPlentyCardAction)
            {
                if (this.developmentCardPlayerThisTurn)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "922", "Cannot play more than one development card per turn"),
                        this.currentPlayer);
                    return false;
                }

                DevelopmentCard card = null;
                if ((card = this.currentPlayer.HeldCards.FirstOrDefault(c => c.Type == DevelopmentCardTypes.YearOfPlenty &&
                    !this.cardsBoughtThisTurn.Contains(c))) == null)
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "920", "No Year of Plenty card owned that can be played this turn"),
                        this.currentPlayer);
                    return false;
                }

                return false;
            }

            if (playerAction is SelectResourceFromPlayerAction selectResourceFromPlayerAction)
            {
                if (!this.playerIdsInRobberHex.Contains(selectResourceFromPlayerAction.SelectedPlayerId))
                {
                    this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "919", "Invalid player selection"),
                        this.currentPlayer);
                }
                else
                {
                    var player = this.playersById[selectResourceFromPlayerAction.SelectedPlayerId];
                    var resourceIndex = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(player.Resources.Count);
                    var robbedResource = player.LoseResourceAtIndex(resourceIndex);
                    this.currentPlayer.AddResources(robbedResource);
                    this.RaiseEvent(new ResourcesGainedEvent(robbedResource), this.currentPlayer);
                    this.RaiseEvent(new ResourcesLostEvent(robbedResource, this.currentPlayer.Id, ResourcesLostEvent.ReasonTypes.Robbed), player);
                    this.RaiseEvent(new ResourcesLostEvent(robbedResource, this.currentPlayer.Id, ResourcesLostEvent.ReasonTypes.Witness),
                        this.PlayersExcept(this.currentPlayer.Id, player.Id));
                }

                return false;
            }

            if (playerAction is PlaceRobberAction placeRobberAction)
            {
                this.ProcessPlaceRobberAction(placeRobberAction);
                return false;
            }

            if (playerAction is PlaceSettlementAction placeSettlementAction)
            {
                return this.ProcessPlaceSettlementAction(placeSettlementAction);
            }

            if (playerAction is QuitGameAction quitGameAction)
            {
                return this.ProcessQuitGameAction(quitGameAction);
            }

            if (playerAction is RequestStateAction requestStateAction)
            {
                this.ProcessRequestStateAction(requestStateAction);
                return false;
            }

            throw new Exception($"Player action {playerAction.GetType()} not recognised.");
        }

        private bool ProcessPlayKnightCardAction(PlayKnightCardAction playKnightCardAction)
        {
            if (this.developmentCardPlayerThisTurn)
            {
                this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "922", "Cannot play more than one development card per turn"),
                    this.currentPlayer);
                return false;
            }
        
            DevelopmentCard card = null;
            if ((card = this.currentPlayer.HeldCards.FirstOrDefault(c => c.Type == DevelopmentCardTypes.Knight &&
                !this.cardsBoughtThisTurn.Contains(c))) == null)
            {
                this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "920", "No Knight card owned that can be played this turn"),
                    this.currentPlayer);
                return false;
            }

            // TODO: Move this after below check
            this.developmentCardPlayerThisTurn = true;

            if (this.robberHex == playKnightCardAction.NewRobberHex)
            {
                this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "918", "New robber hex cannot be the same as previous robber hex"),
                    this.currentPlayer);
                return false;
            }

            this.currentPlayer.PlaceKnightDevelopmentCard((KnightDevelopmentCard)card);

            this.robberHex = playKnightCardAction.NewRobberHex;
            this.RaiseEvent(new KnightCardPlayedEvent(this.currentPlayer.Id, this.robberHex));

            if (this.currentPlayer.PlayedKnightCards >= 3)
            {
                if (this.playerWithLargestArmy == null)
                {
                    this.RaiseEvent(new LargestArmyChangedEvent(this.currentPlayer.Id, null));
                    this.playerWithLargestArmy = this.currentPlayer;
                    this.currentPlayer.HasLargestArmy = true;
                }
                else if (this.currentPlayer != this.playerWithLargestArmy &&
                    this.currentPlayer.PlayedKnightCards > this.playerWithLargestArmy.PlayedKnightCards)
                {
                    var previousPlayer = this.playerWithLargestArmy;
                    previousPlayer.HasLargestArmy = false;
                    this.playerWithLargestArmy = this.currentPlayer;
                    this.currentPlayer.HasLargestArmy = true;

                    this.RaiseEvent(new LargestArmyChangedEvent(this.currentPlayer.Id, previousPlayer.Id));
                }

                if (this.currentPlayer.VictoryPoints >= 10)
                {
                    this.RaiseEvent(new GameWinEvent(this.currentPlayer.Id, this.currentPlayer.VictoryPoints));
                    return true;
                }
            }

            this.ProcessNewRobberPlacement();
            return false;
        }

        private bool ProcessPlayRoadBuildingCardAction(PlayRoadBuildingCardAction playRoadBuildingCardAction)
        {
            if (this.developmentCardPlayerThisTurn)
            {
                this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "922", "Cannot play more than one development card per turn"),
                    this.currentPlayer);
                return false;
            }

            DevelopmentCard card = null;
            if ((card = this.currentPlayer.HeldCards.FirstOrDefault(c => c.Type == DevelopmentCardTypes.RoadBuilding &&
                !this.cardsBoughtThisTurn.Contains(c))) == null)
            {
                this.RaiseEvent(new GameErrorEvent(this.currentPlayer.Id, "920", "No Road building card owned that can be played this turn"),
                    this.currentPlayer);
                return false;
            }

            this.currentPlayer.PlaceRoadBuildingDevelopmentCard((RoadBuildingDevelopmentCard)card);
            this.developmentCardPlayerThisTurn = true;

            return false;
        }

        private void ProcessPlayerQuit(Guid playerId)
        {
            this.players = this.players.Where(player => player.Id != playerId).ToArray();
            this.playersById.Remove(playerId);
        }

        private bool ProcessQuitGameAction(QuitGameAction quitGameAction)
        {
            this.ProcessPlayerQuit(quitGameAction.InitiatingPlayerId);
            this.playerIndex--;
            this.RaiseEvent(new PlayerQuitEvent(quitGameAction.InitiatingPlayerId));
            if (this.players.Length == 1)
            {
                this.RaiseEvent(new GameWinEvent(this.players[0].Id, this.players[0].VictoryPoints));
                return true;
            }
            else if (!this.isGameSetup)
            {
                this.StartTurn();
            }

            return false;
        }

        private void ProcessRequestStateAction(RequestStateAction requestStateAction)
        {
            var player = this.playersById[requestStateAction.InitiatingPlayerId];
            var requestStateEvent = new RequestStateEvent(player.Id);
            requestStateEvent.Cities = player.RemainingCities;

            requestStateEvent.HeldCards = 0;
            requestStateEvent.DevelopmentCardsByCount = player.HeldCards?.Aggregate(new Dictionary<DevelopmentCardTypes, int>(),
                (dict, card) => {
                    if (!dict.ContainsKey(card.Type))
                        dict.Add(card.Type, 1);
                    else
                        dict[card.Type]++;
                    requestStateEvent.HeldCards++;
                    return dict;
                });
            requestStateEvent.PlayedKnightCards = player.PlayedKnightCards;
            requestStateEvent.Resources = player.Resources;
            requestStateEvent.RoadSegments = player.RemainingRoadSegments;
            requestStateEvent.Settlements = player.RemainingSettlements;
            requestStateEvent.VictoryPoints = player.VictoryPoints;
            this.RaiseEvent(requestStateEvent, player);
        }

        private void RaiseEvent(GameEvent gameEvent)
        {
            this.RaiseEvent(gameEvent, this.players);
        }

        private void RaiseEvent(GameEvent gameEvent, IEnumerable<IPlayer> players)
        {
            var message = $"Sending {this.ToPrettyString(gameEvent)} " +
                $"to {string.Join(", ", players.Select(player => player.Name))}";
            this.log.Add(message);
            this.eventRaiser.SendEvent(gameEvent, players);
        }

        private void RaiseEvent(GameEvent gameEvent, IPlayer player)
        {
            this.log.Add($"Sending {this.ToPrettyString(gameEvent)} to {player.Name}");
            this.eventRaiser.SendEvent(gameEvent, player.Id);
        }

        private void SetStandardExpectedActionsForCurrentPlayer()
        {
            this.actionManager.SetExpectedActionsForPlayer(this.currentPlayer.Id,
                typeof(BuyDevelopmentCardAction),
                typeof(EndOfTurnAction), typeof(MakeDirectTradeOfferAction),
                typeof(PlaceCityAction), typeof(PlaceRoadSegmentAction),
                typeof(PlaceSettlementAction), typeof(PlayKnightCardAction),
                typeof(PlayMonopolyCardAction), typeof(PlayRoadBuildingCardAction),
                typeof(PlayYearOfPlentyCardAction));

            foreach (var player in this.PlayersExcept(this.currentPlayer.Id))
                this.actionManager.SetExpectedActionsForPlayer(player.Id, null);
        }

        private void StartTurn()
        {
            this.ChangeToNextPlayer();
            this.cardsBoughtThisTurn.Clear();
            this.developmentCardPlayerThisTurn = false;

            foreach (var player in this.players)
                this.actionManager.SetExpectedActionsForPlayer(player.Id, null);

            this.numberGenerator.RollTwoDice(out var dice1, out var dice2);
            if (dice1 + dice2 != 7)
            {
                this.StartTurnWithResourceCollection(dice1, dice2);
                this.SetStandardExpectedActionsForCurrentPlayer();
            }
            else
            {
                this.StartTurnWithRobberPlacement(dice1, dice2);
            }
        }

        private void StartTurnWithRobberPlacement(uint dice1, uint dice2)
        {
            var startPlayerTurnEvent = new StartTurnEvent(this.currentPlayer.Id, dice1, dice2, null);
            this.RaiseEvent(startPlayerTurnEvent);
            this.WaitForLostResourcesFromPlayers();
            this.WaitForRobberPlacement();
        }

        private string ToPrettyString(GameEvent gameEvent)
        {
            var message = $"{gameEvent.SimpleTypeName}";
            if (gameEvent is StartTurnEvent startTurnEvent)
                message += $", Dice rolls {startTurnEvent.Dice1} {startTurnEvent.Dice2}";
            return message;
        }

        private string ToPrettyString(IEnumerable<string> playerNames)
        {
            return $"{string.Join(", ", playerNames)}";
        }

        private void WaitForGameStartConfirmationFromPlayers()
        {
            foreach (var player in this.players) {
                this.actionManager.SetExpectedActionsForPlayer(player.Id, typeof(QuitGameAction), typeof(ConfirmGameStartAction));
                this.RaiseEvent(new ConfirmGameStartEvent(), player);
            }

            var playersToConfirm = new HashSet<IPlayer>(this.players);
            while (playersToConfirm.Count > 0)
            {
                var playerAction = this.WaitForPlayerAction();
                if (playerAction is ConfirmGameStartAction confirmGameStartAction)
                {
                    playersToConfirm.Remove(this.playersById[confirmGameStartAction.InitiatingPlayerId]);
                }
                else if (playerAction is QuitGameAction quitGameAction)
                {
                    playersToConfirm.Remove(this.playersById[quitGameAction.InitiatingPlayerId]);
                    this.ProcessPlayerQuit(quitGameAction.InitiatingPlayerId);
                }
            }

            if (this.players.Length == 0)
                throw new OperationCanceledException();

            var resourcesCollectedByPlayerId = new Dictionary<Guid, ResourceCollection[]>();
            foreach (var player in this.players)
            {
                var settlementlocation = this.gameBoard.GetSettlementsForPlayer(player.Id)[1];
                var resourcesForLocation = this.gameBoard.GetResourcesForLocation(settlementlocation);
                player.AddResources(resourcesForLocation);
                var resourceCollection = new ResourceCollection(settlementlocation, resourcesForLocation);
                resourcesCollectedByPlayerId.Add(player.Id, new[] { resourceCollection });
            }
            var resourcesCollectedEvent = new ResourcesCollectedEvent(resourcesCollectedByPlayerId);
            this.RaiseEvent(resourcesCollectedEvent);
        }

        private void WaitForLostResourcesFromPlayers()
        {
            foreach (var player in this.players)
            {
                this.actionManager.SetExpectedActionsForPlayer(player.Id, null);
                if (player.Resources.Count > 7)
                {
                    this.actionManager.SetExpectedActionsForPlayer(player.Id, typeof(LoseResourcesAction));
                    var resourceCount = player.Resources.Count / 2;
                    var chooseLostResourceEvent = new ChooseLostResourcesEvent(resourceCount);
                    this.chooseLostResourcesEventByPlayerId[player.Id] = chooseLostResourceEvent;
                    this.RaiseEvent(new ChooseLostResourcesEvent(resourceCount), player);
                }
            }

            while (this.chooseLostResourcesEventByPlayerId.Count > 0)
            {
                var loseResourcesAction = (LoseResourcesAction)this.WaitForPlayerAction();
                var player = this.playersById[loseResourcesAction.InitiatingPlayerId];
                var chooseLostResourcesEvent = this.chooseLostResourcesEventByPlayerId[player.Id];

                if (loseResourcesAction.Resources.Count != chooseLostResourcesEvent.ResourceCount)
                {
                    var expectedResourceCount = chooseLostResourcesEvent.ResourceCount + " resource";
                    if (chooseLostResourcesEvent.ResourceCount > 1)
                        expectedResourceCount += "s";
                    this.RaiseEvent(new GameErrorEvent(player.Id, "916", $"Expected {expectedResourceCount} but received {loseResourcesAction.Resources.Count}"), player);
                    continue;
                }

                if (player.Resources - loseResourcesAction.Resources < ResourceClutch.Zero)
                {
                    this.RaiseEvent(new GameErrorEvent(player.Id, "917", "Resources sent results in negative counts"), player);
                    continue;
                }

                player.RemoveResources(loseResourcesAction.Resources);
                this.actionManager.SetExpectedActionsForPlayer(player.Id, null);
                this.chooseLostResourcesEventByPlayerId.Remove(player.Id);
                this.RaiseEvent(new ResourcesLostEvent(loseResourcesAction.Resources, Guid.Empty, ResourcesLostEvent.ReasonTypes.TooManyResources));
            }
        }

        private PlayerAction WaitForPlayerAction()
        {
            while (true)
            {
                Thread.Sleep(50);
                this.cancellationToken.ThrowIfCancellationRequested();

                if (this.turnTimer.IsLate)
                {
                    // Out of time so game should be killed
                    throw new TimeoutException($"Time out exception waiting for player '{this.currentPlayer.Name}'");
                }

                if (this.actionRequests.TryDequeue(out var playerAction))
                {
                    var playerActionTypeName = playerAction.GetType().Name;
                    var playerName = this.playersById[playerAction.InitiatingPlayerId].Name;
                    this.log.Add($"Received {playerActionTypeName} from {playerName}");

                    if (playerAction is RequestStateAction ||
                        playerAction is QuitGameAction)
                        return playerAction;

                    if (!this.actionManager.ValidateAction(playerAction))
                    {
                        var expectedActions = this.actionManager.GetExpectedActionsForPlayer(playerAction.InitiatingPlayerId);
                        var errorMessage = this.GetErrorMessage(playerAction.GetType(), expectedActions);
                        this.RaiseEvent(new GameErrorEvent(playerAction.InitiatingPlayerId, "999", errorMessage),
                            this.playersById[playerAction.InitiatingPlayerId]);
                        this.log.Add($"FAILED: Action Validation - {playerName}, {playerActionTypeName}");
                        continue;
                    }

                    this.log.Add($"Validated {playerActionTypeName} from {playerName}");
                    return playerAction;
                }
            }
        }

        private RequestStateAction WaitForRequestStateAction()
        {
            while (true)
            {
                Thread.Sleep(50);
                this.cancellationToken.ThrowIfCancellationRequested();

                if (this.actionRequests.TryDequeue(out var playerAction))
                {
                    var playerActionTypeName = playerAction.GetType().Name;
                    var playerName = this.playersById[playerAction.InitiatingPlayerId].Name;

                    if (!(playerAction is RequestStateAction))
                    {
                        var errorMessage = $"Received action type {playerActionTypeName}. Expected RequestStateAction";
                        this.RaiseEvent(new GameErrorEvent(playerAction.InitiatingPlayerId, "999", errorMessage),
                            this.playersById[playerAction.InitiatingPlayerId]);
                        this.log.Add($"FAILED: Action Validation - {playerName}, {playerActionTypeName}");
                        continue;
                    }
                    
                    this.log.Add($"Received {playerActionTypeName} from {playerName}");

                    
                    return (RequestStateAction)playerAction;
                }
            }
        }

        private void WaitForRobberPlacement()
        {
            this.RaiseEvent(new PlaceRobberEvent(), this.currentPlayer);
            this.actionManager.SetExpectedActionsForPlayer(this.currentPlayer.Id, typeof(PlaceRobberAction));
            this.ProcessPlayerAction(this.WaitForPlayerAction());
        }

        private PlayerAction WaitForPlayerAction(HashSet<Type> allowedActionTypes, List<Guid?> players, IGameTimer[] turnTimers, PlayerAction[] timeOutActions)
        {
            while (true)
            {
                Thread.Sleep(50);
                this.cancellationToken.ThrowIfCancellationRequested();

                for (var index = 0; index < turnTimers.Length; index++)
                {
                    if (turnTimers[index].IsLate)
                    {
                        // TODO: return time out player action
                        // players[index] = null;
                        // return timeOutActions[index]
                    }
                }

                if (this.actionRequests.TryDequeue(out var playerAction))
                {
                    var playerActionTypeName = playerAction.GetType().Name;
                    var playerName = this.playersById[playerAction.InitiatingPlayerId].Name;
                    var message = $"Received {playerActionTypeName} from {playerName}";

                    if (this.IsAutomaticPlayerAction(playerAction))
                    {
                        //players[index] = null;
                        this.log.Add(message + " AUTOMATIC");
                        return playerAction;
                    }

                    if (!players.Contains(playerAction.InitiatingPlayerId))
                    {
                        this.log.Add(message + " PLAYER MISSING");
                        continue;
                    }

                    if (!allowedActionTypes.Contains(playerAction.GetType()))
                    {
                        this.log.Add(message + " NO MATCH");
                        continue;
                    }

                    var index = players.FindIndex(playerId => playerId == playerAction.InitiatingPlayerId);
                    players[index] = null;

                    this.log.Add($"Validated {playerActionTypeName} from {playerName}");
                    return playerAction;
                }
            }
        }

        private bool IsAutomaticPlayerAction(PlayerAction playerAction)
        {
            return playerAction is RequestStateAction || playerAction is QuitGameAction;
        }
        #endregion

        #region Structures
        public interface IActionManager
        {
            void AddExpectedActionsForPlayer(Guid playerId, params Type[] actionsTypes);
            HashSet<Type> GetExpectedActionsForPlayer(Guid playerId);
            void SetExpectedActionsForPlayer(Guid playerId, params Type[] actionTypes);
            bool ValidateAction(PlayerAction playerAction);
        }

        private class ActionManager : IActionManager
        {
            private readonly Dictionary<Guid, HashSet<Type>> actionTypesByPlayerId = new Dictionary<Guid, HashSet<Type>>();

            public void AddExpectedActionsForPlayer(Guid playerId, params Type[] actionTypes)
            {
                if (actionTypes == null || actionTypes.Length == 0)
                    throw new Exception("Must add at least one action type to player");

                foreach (var actionType in actionTypes)
                    this.actionTypesByPlayerId[playerId].Add(actionType);
            }

            public HashSet<Type> GetExpectedActionsForPlayer(Guid playerId)
            {
                return this.actionTypesByPlayerId[playerId];
            }

            public void SetExpectedActionsForPlayer(Guid playerId, params Type[] actionTypes)
            {
                if (actionTypes == null || actionTypes.Length == 0)
                    this.actionTypesByPlayerId[playerId] = null;
                else
                    this.actionTypesByPlayerId[playerId] = new HashSet<Type>(actionTypes);
            }

            public bool ValidateAction(PlayerAction playerAction)
            {
                var initiatingPlayerId = playerAction.InitiatingPlayerId;
                if (this.actionTypesByPlayerId.ContainsKey(initiatingPlayerId))
                {
                    return 
                        this.actionTypesByPlayerId[initiatingPlayerId] != null &&
                        this.actionTypesByPlayerId[initiatingPlayerId].Contains(playerAction.GetType());
                }

                return false;
            }
        }

        private class EventRaiser : IEventSender
        {
            private Dictionary<Guid, Action<GameEvent>> gameEventHandlersByPlayerId = new Dictionary<Guid, Action<GameEvent>>();
        
            public bool CanSendEvents { get; set; } = true;

            public void AddEventHandler(Guid playerId, Action<GameEvent> gameEventHandler)
            {
                this.gameEventHandlersByPlayerId.Add(playerId, gameEventHandler);
            }

            public void SendEvent(GameEvent gameEvent, Guid playerId)
            {
                if (!this.CanSendEvents)
                    return;
                
                this.gameEventHandlersByPlayerId[playerId].Invoke(gameEvent);
            }

            public void SendEvent(GameEvent gameEvent, IEnumerable<IPlayer> players)
            {
                if (!this.CanSendEvents)
                    return;

                foreach (var player in players)
                    this.gameEventHandlersByPlayerId[player.Id].Invoke(gameEvent);
            }
        }
        #endregion
    }
}
