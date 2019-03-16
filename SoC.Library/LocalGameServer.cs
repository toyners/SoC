

namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;

    public class LocalGameServer
    {
        #region Fields
        private readonly ConcurrentQueue<PlayerAction> actionRequests = new ConcurrentQueue<PlayerAction>();
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
            var token = this.tokenManager.GetNewToken(player, typeof(PlaceInfrastructureAction));
            // TODO: Pass back current settled locations
            this.eventRaiser.RaiseEvent(new PlaceSetupInfrastructureEvent(), player.Id, token);
            while (true)
            {
                var playerAction = this.WaitForPlayerAction();
                this.turnTimer.Reset();

                if (playerAction is PlaceInfrastructureAction placeInfrastructureAction)
                {
                    this.PlaceInfrastructure(player, placeInfrastructureAction.SettlementLocation, placeInfrastructureAction.RoadEndLocation);
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

        private IEnumerable<IPlayer> PlayersExcept(params Guid[] playerIds) => this.playersById.Select(kv => kv.Value).Where(player => playerIds.Contains(player.Id));

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
            if (!this.tokenManager.ValidateToken(token, playerAction.GetType()))
                throw new Exception($"Token not valid for {this.playersById[playerAction.PlayerId]}, {playerAction.GetType().Name}");

            this.actionRequests.Enqueue( playerAction);
        }

        private void ProcessPlayerAction(PlayerAction playerAction)
        {
            if (playerAction is AnswerDirectTradeOfferAction answerDirectTradeOfferAction)
            {
                var answerDirectTradeOfferEvent = new AnswerDirectTradeOfferEvent(
                    answerDirectTradeOfferAction.PlayerId, answerDirectTradeOfferAction.WantedResources);

                var token = this.tokenManager.GetNewToken(
                        this.playersById[answerDirectTradeOfferAction.InitialPlayerId]);

                // Initial player gets chance to confirm. 
                this.eventRaiser.RaiseEvent(
                    answerDirectTradeOfferEvent,
                    answerDirectTradeOfferAction.InitialPlayerId,
                    token);

                var message = this.ToPrettyString(
                    answerDirectTradeOfferEvent, 
                    token, 
                    new[] { this.playersById[answerDirectTradeOfferAction.InitialPlayerId].Name });
                this.log.Add(message);

                // Other two players gets informational event
                var otherPlayers = this.PlayersExcept(
                        answerDirectTradeOfferAction.PlayerId,
                        answerDirectTradeOfferAction.InitialPlayerId);

                this.eventRaiser.RaiseEvent(answerDirectTradeOfferEvent, otherPlayers);
                 
                message = this.ToPrettyString(
                    answerDirectTradeOfferEvent, 
                    null,
                    otherPlayers.Select(player => player.Name));
                this.log.Add(message);
            }

            if (playerAction is EndOfTurnAction)
            {
                this.StartTurn();
                return;
            }

            if (playerAction is MakeDirectTradeOfferAction makeDirectTradeOfferAction)
            {
                var makeDirectTradeOfferEvent = new MakeDirectTradeOfferEvent(
                        makeDirectTradeOfferAction.PlayerId, makeDirectTradeOfferAction.WantedResources);
                this.PlayersExcept(playerAction.PlayerId)
                    .ToList()
                    .ForEach(player => {
                        this.eventRaiser.RaiseEvent(
                            makeDirectTradeOfferEvent,
                            player.Id,
                            this.tokenManager.GetNewToken(player, typeof(AnswerDirectTradeOfferAction)));
                    });

                return;
            }
        }

        private void StartTurn()
        {
            try
            {
                this.ChangeToNextPlayer();
                this.eventRaiser.RaiseEvent(new StartPlayerTurnEvent(), this.currentPlayer.Id);

                var token = this.tokenManager.GetNewToken(this.currentPlayer);
                this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
                var diceRollEvent = new DiceRollEvent(this.currentPlayer.Id, this.dice1, this.dice2);
                this.eventRaiser.RaiseEvent(diceRollEvent, this.currentPlayer.Id, token);
                this.eventRaiser.RaiseEvent(diceRollEvent, this.PlayersExcept(this.currentPlayer.Id));

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

                if (this.actionRequests.TryDequeue(out var playerAction))
                {
                    this.log.Add($"Received {playerAction.GetType().Name} from {this.playersById[playerAction.PlayerId].Name}");
                    return playerAction;
                }
            }
        }

        private string ToPrettyString(GameEvent gameEvent, GameToken gameToken, IEnumerable<string> playerNames)
        {
            var tokenSubstring = gameToken != null ? $" - {gameToken}" : "";
            var playerSubstring = playerNames.Count() > 0 ? $" - {string.Concat(playerNames)}" : "";
            var message = $"{gameEvent.GetType().Name}{tokenSubstring}{playerSubstring}";
            if (gameEvent is DiceRollEvent diceRollEvent)
                message += $", Dice rolls {diceRollEvent.Dice1} {diceRollEvent.Dice2}";

            return message;
        }
        #endregion

        #region Structures
        public interface ITokenManager
        {
            bool ValidateToken(GameToken token, Type actionType);
            GameToken GetNewToken(IPlayer player, Type actionType = null);
            IPlayer GetPlayerForToken(GameToken token);
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
            private Dictionary<TokenInformation, IPlayer> playerByTokenInformation = new Dictionary<TokenInformation, IPlayer>();
            private Dictionary<IPlayer, TokenInformation> tokenInformationByPlayer = new Dictionary<IPlayer, TokenInformation>();

            public GameToken GetNewToken(IPlayer player, Type actionType = null)
            {
                TokenInformation tokenInformation;
                if (this.tokenInformationByPlayer.ContainsKey(player))
                {
                    tokenInformation = this.tokenInformationByPlayer[player];
                    this.tokenInformationByPlayer.Remove(player);
                    if (this.playerByTokenInformation.ContainsKey(tokenInformation))
                        this.playerByTokenInformation.Remove(tokenInformation);
                }

                var token = new GameToken();
                tokenInformation =
                    actionType == typeof(TokenConstraintedPlayerAction) ?
                        new TokenInformation { Token = token, ActionType = actionType } :
                        new TokenInformation { Token = token };
                this.tokenInformationByPlayer.Add(player, tokenInformation);
                this.playerByTokenInformation.Add(tokenInformation, player);
                return token;
            }

            public IPlayer GetPlayerForToken(GameToken token)
            {
                return this.playerByTokenInformation.FirstOrDefault(t => t.Key.Token == token).Value;
            }

            public bool ValidateToken(GameToken token, Type actionType)
            {
                var tokenInformation =
                actionType == typeof(TokenConstraintedPlayerAction) ? 
                    new TokenInformation { Token = token, ActionType = actionType } :
                    new TokenInformation { Token = token };

                return this.playerByTokenInformation.ContainsKey(tokenInformation);
            }

            private class TokenInformation
            {
                public GameToken Token;
                public Type ActionType;

                public override bool Equals(object obj)
                {
                    if (object.ReferenceEquals(this, obj))
                        return true;

                    if (obj == null)
                        return false;

                    var other = obj as TokenInformation;
                    if (other == null)
                        return false;

                    if (this.Token != other.Token)
                        return false;

                    if (this.ActionType != null && other.ActionType != null)
                        return this.ActionType.Equals(other.ActionType);
                    else if (this.ActionType == null && other.ActionType == null)
                        return true;

                    return false;
                }

                public override int GetHashCode()
                {
                    var hashCode = 869305385;
                    hashCode = hashCode * -1521134295 + EqualityComparer<GameToken>.Default.GetHashCode(this.Token);
                    hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(this.ActionType);
                    return hashCode;
                }
            }
        }
        #endregion
    }

    public interface ILog
    {
        void Add(string message);
        //void Add(GameEvent gameEvent);
        void WriteToFile(string filePath);
    }

    public class Log : ILog
    {
        public List<string> Messages { get; private set; } = new List<string>();

        public void Add(string message) => this.Messages.Add(message);

        /*public void Add(GameEvent gameEvent)
        {
            var message = $"Event: {gameEvent.GetType().Name}, Initiating Player: {this.playerNamesById[gameEvent.PlayerId]} ";
            if (gameEvent is DiceRollEvent diceRollEvent)
            {
                message += $"Dice: {diceRollEvent.Dice1}, {diceRollEvent.Dice2}";
            }
            else if (gameEvent is MakeDirectTradeOfferEvent makeDirectTradeOfferEvent)
            {
                message += $"Buying Player: {this.playerNamesById[makeDirectTradeOfferEvent.BuyingPlayerId]}";
            }

            this.Messages.Add(message);
        }*/

        public void WriteToFile(string filePath) => File.WriteAllLines(filePath, this.Messages);
    }
}
