
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.PlayerActions;

    public class LocalGameServer
    {
        #region Fields
        private readonly ActionManager actionManager;
        private readonly ConcurrentQueue<Tuple<GameToken, PlayerAction>> actionRequests = new ConcurrentQueue<Tuple<GameToken, PlayerAction>>();
        private readonly IDevelopmentCardHolder developmentCardHolder;
        private readonly EventRaiser eventRaiser;
        private readonly GameBoard gameBoard;
        private readonly ILog log = new Log();
        private readonly INumberGenerator numberGenerator;
        private readonly ITokenManager tokenManager;
        private IPlayer currentPlayer;
        private bool isQuitting;
        private IDictionary<Guid, IPlayer> playersById;
        private int playerIndex;
        private IPlayer[] players;
        private uint dice1, dice2;
        private IGameTimer turnTimer;
        private Func<Guid> idGenerator;
        private Tuple<Guid, ResourceClutch> initialDirectTradeOffer;
        private Dictionary<Guid, ResourceClutch> answeringDirectTradeOffers = new Dictionary<Guid, ResourceClutch>();
        #endregion

        #region Construction
        public LocalGameServer(INumberGenerator numberGenerator, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder)
        {
            this.numberGenerator = numberGenerator;
            this.gameBoard = gameBoard;
            this.developmentCardHolder = developmentCardHolder;
            this.turnTimer = new GameServerTimer();
            this.idGenerator = () => { return Guid.NewGuid(); };
            this.tokenManager = new TokenManager();
            this.eventRaiser = new EventRaiser(this.log);
            this.actionManager = new ActionManager();
        }
        #endregion

        private event Action<Exception> GameExceptionEvent;

        #region Methods
        public void AddResourcesToPlayer(string playerName, ResourceClutch value)
        {
            // TODO: Return an error if player not found?
            this.players
                .Where(p => p.Name == playerName)
                .FirstOrDefault()
                ?.AddResources(value);
        }

        public void JoinGame(string playerName, GameController gameController)
        {
            var player = new Player(playerName, this.idGenerator.Invoke());
            this.players[this.playerIndex++] = player;

            this.eventRaiser.AddEventHandler(player.Id, gameController.GameEventHandler);
            this.GameExceptionEvent += gameController.GameExceptionHandler;
            gameController.PlayerActionEvent += this.PlayerActionEventHandler;

            this.eventRaiser.RaiseEvent(new GameJoinedEvent(player.Id), player.Id);
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
            this.isQuitting = true;
            this.eventRaiser.CanRaiseEvents = false;
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

        public void StartGameAsync()
        {
            // Launch server processing on separate thread
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Name = "Local Game Server";
                try
                {
                    this.players = PlayerTurnOrderCreator.Create(this.players, this.numberGenerator);
                    // TODO: Notify players what the order is

                    this.playersById = this.players.ToDictionary(p => p.Id, p => p);

                    // TODO: Send event with player details to everyone
                    var playerIdsByName = this.players.ToDictionary(p => p.Name, p => p.Id);
                    this.eventRaiser.RaiseEvent(new PlayerSetupEvent(playerIdsByName));

                    var gameBoardSetup = new GameBoardSetup(this.gameBoard);
                    this.eventRaiser.RaiseEvent(new InitialBoardSetupEvent(gameBoardSetup));

                    try
                    {
                        this.GameSetup();
                        this.MainGameLoop();
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    this.GameExceptionEvent?.Invoke(e);
                }
            });
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

                var resourcesCollectedEvent = new ResourcesCollectedEvent(player.Id, resourcesCollectionOrderedByLocation);
                this.eventRaiser.RaiseEvent(resourcesCollectedEvent, null);
            }
        }

        private void GameSetup()
        {
            // Place first settlement
            for (int i = 0; i < this.players.Length; i++)
            {
                this.GameSetupLoop(this.players[i]);
            }

            // Place second settlement
            for (int i = this.players.Length - 1; i >= 0; i--)
            {
                this.GameSetupLoop(this.players[i]);
            }
        }

        private void GameSetupLoop(IPlayer player)
        {
            var token = this.tokenManager.CreateNewToken(player);
            // TODO: Pass back current settled locations
            var placeSetupInfrastructureEvent = new PlaceSetupInfrastructureEvent();
            this.actionManager.SetExpectedActionTypeForPlayer(player.Id, typeof(PlaceSetupInfrastructureAction));
            this.eventRaiser.RaiseEvent(placeSetupInfrastructureEvent, player.Id, token);
            while (true)
            {
                var playerAction = this.WaitForPlayerAction();
                this.turnTimer.Reset();

                if (playerAction is PlaceSetupInfrastructureAction placeSetupInfrastructureAction)
                {
                    this.actionManager.SetExpectedActionTypeForPlayer(playerAction.InitiatingPlayerId, null);
                    this.PlaceInfrastructure(player, placeSetupInfrastructureAction.SettlementLocation, placeSetupInfrastructureAction.RoadEndLocation);
                    break;
                }
                else
                {
                    //TODO: Handle case where action is not correct
                }
            }
        }

        private void MainGameLoop()
        {
            if (this.isQuitting)
                return;

            this.playerIndex = -1;
            this.StartTurn();
            this.turnTimer.Reset();
            while (true)
            {
                var playerAction = this.WaitForPlayerAction();
                this.turnTimer.Reset();
                this.ProcessPlayerAction(playerAction);
            }
        }

        private IEnumerable<IPlayer> PlayersExcept(params Guid[] playerIds) => this.playersById.Select(kv => kv.Value).Where(player => !playerIds.Contains(player.Id));

        private void PlaceInfrastructure(IPlayer player, uint settlementLocation, uint roadEndLocation)
        {
            // TODO: Validation

            try
            {
                this.gameBoard.PlaceStartingInfrastructure(player.Id, settlementLocation, roadEndLocation);
                player.PlaceStartingInfrastructure();
            }
            catch (Exception e)
            {
                // TODO
            }
        }

        private void PlayerActionEventHandler(GameToken token, PlayerAction playerAction)
        {
            // Leave all validation and processing to the game server thread
            this.actionRequests.Enqueue(new Tuple<GameToken, PlayerAction>(token, playerAction));
        }

        private void ProcessAnswerDirectTradeOfferAction(AnswerDirectTradeOfferAction answerDirectTradeOfferAction)
        {
            var answerDirectTradeOfferEvent = new AnswerDirectTradeOfferEvent(
                    answerDirectTradeOfferAction.InitiatingPlayerId, answerDirectTradeOfferAction.WantedResources);

            this.answeringDirectTradeOffers.Add(
                answerDirectTradeOfferAction.InitiatingPlayerId,
                answerDirectTradeOfferAction.WantedResources);

            var token = this.tokenManager.CreateNewToken(
                    this.playersById[answerDirectTradeOfferAction.InitialPlayerId]);

            // Initial player gets chance to confirm. 
            var message = $"Sending {this.ToPrettyString(answerDirectTradeOfferEvent)} " +
                $"to {this.playersById[answerDirectTradeOfferAction.InitialPlayerId].Name}, {token}";
            this.log.Add(message);

            this.eventRaiser.RaiseEvent(
                answerDirectTradeOfferEvent,
                answerDirectTradeOfferAction.InitialPlayerId,
                token);

            // Other two players gets informational event
            var informationalAnswerDirectTradeOfferEvent = new AnswerDirectTradeOfferEvent(
                answerDirectTradeOfferAction.InitiatingPlayerId, answerDirectTradeOfferAction.WantedResources);
            informationalAnswerDirectTradeOfferEvent.IsInformation = true;

            var otherPlayers = this.PlayersExcept(
                    answerDirectTradeOfferAction.InitiatingPlayerId,
                    answerDirectTradeOfferAction.InitialPlayerId);

            message = $"Sending {this.ToPrettyString(answerDirectTradeOfferEvent)} " +
                $"to {string.Join(", ", otherPlayers.Select(player => player.Name))}";
            this.log.Add(message);

            this.eventRaiser.RaiseEvent(informationalAnswerDirectTradeOfferEvent, otherPlayers);
        }

        private void ProcessMakeDirectTradeOfferAction(MakeDirectTradeOfferAction makeDirectTradeOfferAction)
        {
            var makeDirectTradeOfferEvent = new MakeDirectTradeOfferEvent(
                        makeDirectTradeOfferAction.InitiatingPlayerId, makeDirectTradeOfferAction.WantedResources);

            this.initialDirectTradeOffer = new Tuple<Guid, ResourceClutch>(
                makeDirectTradeOfferAction.InitiatingPlayerId,
                makeDirectTradeOfferAction.WantedResources);

            var otherPlayers = this.PlayersExcept(makeDirectTradeOfferAction.InitiatingPlayerId).ToList();
            otherPlayers.ForEach(player => {
                this.actionManager.SetExpectedActionTypeForPlayer(player.Id, typeof(AnswerDirectTradeOfferAction));
                this.eventRaiser.RaiseEvent(
                    makeDirectTradeOfferEvent,
                    player.Id,
                    this.tokenManager.CreateNewToken(player));
            });
        }

        private void ProcessPlayerAction(PlayerAction playerAction)
        {
            if (playerAction is AcceptDirectTradeAction acceptDirectTradeAction)
            {

                return;
            }

            if (playerAction is AnswerDirectTradeOfferAction answerDirectTradeOfferAction)
            {
                this.ProcessAnswerDirectTradeOfferAction(answerDirectTradeOfferAction);
                return;
            }

            if (playerAction is EndOfTurnAction)
            {
                this.StartTurn();
                return;
            }

            if (playerAction is MakeDirectTradeOfferAction makeDirectTradeOfferAction)
            {
                this.ProcessMakeDirectTradeOfferAction(makeDirectTradeOfferAction);
                return;
            }

            if (playerAction is RequestStateAction requestStateAction)
            {
                var player = this.playersById[requestStateAction.InitiatingPlayerId];
                var requestStateEvent = new RequestStateEvent(requestStateAction.InitiatingPlayerId);
                requestStateEvent.Resources = player.Resources;

                var message = $"Sending {this.ToPrettyString(requestStateEvent)} to {player.Name}";
                this.log.Add(message);

                this.eventRaiser.RaiseEvent(requestStateEvent, requestStateAction.InitiatingPlayerId);
                return;
            }

            throw new Exception($"Player action {playerAction.GetType()} not recognised.");
        }

        private void StartTurn()
        {
            try
            {
                this.ChangeToNextPlayer();

                this.SendStartPlayerTurnEvent();
                this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
                this.SendDiceRollEvent();

                var resourceRoll = this.dice1 + this.dice2;
                if (resourceRoll != 7)
                {
                    this.CollectResourcesAtStartOfTurn(resourceRoll);
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                if (!this.isQuitting)
                    this.GameExceptionEvent?.Invoke(e);
            }
        }

        private void SendDiceRollEvent()
        {
            var token = this.tokenManager.CreateNewToken(this.currentPlayer);
            var diceRollEvent = new DiceRollEvent(this.currentPlayer.Id, this.dice1, this.dice2);
            var message = $"Sending {this.ToPrettyString(diceRollEvent)} " +
                $"to {this.currentPlayer.Name}, {token}";
            this.log.Add(message);
            this.eventRaiser.RaiseEvent(diceRollEvent, this.currentPlayer.Id, token);
        }

        private void SendStartPlayerTurnEvent()
        {
            var startPlayerTurnEvent = new StartPlayerTurnEvent();
            var message = $"Sending {this.ToPrettyString(startPlayerTurnEvent)} to {this.currentPlayer.Name}";
            this.log.Add(message);
            this.eventRaiser.RaiseEvent(startPlayerTurnEvent, this.currentPlayer.Id);
        }

        private PlayerAction WaitForPlayerAction()
        {
            while (true)
            {
                Thread.Sleep(50);
                if (this.isQuitting)
                    throw new TaskCanceledException();

                if (this.turnTimer.IsLate)
                {
                    // Out of time so game should be killed
                    throw new TimeoutException($"Time out exception waiting for player '{this.currentPlayer.Name}'");
                }

                if (this.actionRequests.TryDequeue(out var actionRequest))
                {
                    var token = actionRequest.Item1;
                    var playerAction = actionRequest.Item2;
                    this.log.Add($"Received {playerAction.GetType().Name} from {this.playersById[playerAction.InitiatingPlayerId].Name}");

                    if (!this.tokenManager.ValidateToken(token))
                    {
                        this.log.Add($"FAILED: Token Validation - {this.playersById[playerAction.InitiatingPlayerId]}, {playerAction.GetType().Name}");
                        continue;
                    }

                    if (!(playerAction is RequestStateAction) && !this.actionManager.ValidateAction(playerAction))
                    {
                        this.log.Add($"FAILED: Action Validation - {this.playersById[playerAction.InitiatingPlayerId]}, {playerAction.GetType().Name}");
                        continue;
                    }

                    return playerAction;
                }
            }
        }

        private string ToPrettyString(GameEvent gameEvent)
        {
            var message = $"{gameEvent.GetType().Name}";
            if (gameEvent is DiceRollEvent diceRollEvent)
                message += $", Dice rolls {diceRollEvent.Dice1} {diceRollEvent.Dice2}";
            return message;
        }

        private string ToPrettyString(IEnumerable<string> playerNames)
        {
            return $"{string.Join(", ", playerNames)}";
        }
        #endregion

        #region Structures
        public interface ITokenManager
        {
            GameToken CreateNewToken(IPlayer player);
            IPlayer GetPlayerForToken(GameToken token);
            bool ValidateToken(GameToken token);
        }

        public interface IActionManager
        {
            void SetExpectedActionTypeForPlayer(Guid initiatingPlayerId, Type actionType);
            bool ValidateAction(PlayerAction playerAction);
        }

        private class ActionManager : IActionManager
        {
            private readonly Dictionary<Guid, Type> actionTypesByPlayerId = new Dictionary<Guid, Type>();
            public void SetExpectedActionTypeForPlayer(Guid initiatingPlayerId, Type actionType)
            {
                this.actionTypesByPlayerId[initiatingPlayerId] = actionType;
            }

            public bool ValidateAction(PlayerAction playerAction)
            {
                var initiatingPlayerId = playerAction.InitiatingPlayerId;
                if (this.actionTypesByPlayerId.ContainsKey(initiatingPlayerId))
                {
                    return 
                        this.actionTypesByPlayerId[initiatingPlayerId] == null ||
                        this.actionTypesByPlayerId[initiatingPlayerId] == playerAction.GetType();
                }

                return true;
            }
        }

        private class EventRaiser
        {
            private Dictionary<Guid, Action<GameEvent, GameToken>> gameEventHandlersByPlayerId = new Dictionary<Guid, Action<GameEvent, GameToken>>();
            private event Action<GameEvent, GameToken> gameEventHandler;
            private ILog log;

            public EventRaiser(ILog log)
            {
                this.log = log;
            }

            public bool CanRaiseEvents { get; set; } = true;

            public void AddEventHandler(Guid playerId, Action<GameEvent, GameToken> gameEventHandler)
            {
                this.gameEventHandler += gameEventHandler;
                this.gameEventHandlersByPlayerId.Add(playerId, gameEventHandler);
            }

            public void RaiseEvent(GameEvent gameEvent)
            {
                if (!this.CanRaiseEvents)
                    return;

                this.gameEventHandler.Invoke(gameEvent, null);
            }

            public void RaiseEvent(GameEvent gameEvent, Guid playerId, GameToken gameToken = null)
            {
                if (!this.CanRaiseEvents)
                    return;

                this.gameEventHandlersByPlayerId[playerId].Invoke(gameEvent, gameToken);
            }

            public void RaiseEvent(GameEvent gameEvent, IEnumerable<IPlayer> players)
            {
                if (!this.CanRaiseEvents)
                    return;

                foreach (var player in players)
                    this.gameEventHandlersByPlayerId[player.Id].Invoke(gameEvent, null);
            }
        }

        private class TokenManager: ITokenManager
        {
            private readonly Dictionary<GameToken, IPlayer> playersByToken = new Dictionary<GameToken, IPlayer>();
            private readonly Dictionary<IPlayer, GameToken> tokensByPlayer = new Dictionary<IPlayer, GameToken>();

            public GameToken CreateNewToken(IPlayer player)
            {
                GameToken token;
                if (this.tokensByPlayer.ContainsKey(player))
                {
                    token = this.tokensByPlayer[player];
                    this.tokensByPlayer.Remove(player);
                    if (this.playersByToken.ContainsKey(token))
                        this.playersByToken.Remove(token);
                }

                token = new GameToken();
                this.tokensByPlayer.Add(player, token);
                this.playersByToken.Add(token, player);
                return token;
            }

            public IPlayer GetPlayerForToken(GameToken token)
            {
                return this.playersByToken.FirstOrDefault(t => t.Key == token).Value;
            }

            public bool ValidateToken(GameToken token)
            {
                return this.playersByToken.ContainsKey(token);
            }
        }
        #endregion
    }
}
