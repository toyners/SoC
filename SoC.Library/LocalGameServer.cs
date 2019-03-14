

namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;

    public class LocalGameServer
    {
        private readonly ConcurrentQueue<PlayerAction> actionRequests = new ConcurrentQueue<PlayerAction>();
        private IPlayer currentPlayer;
        private readonly IDevelopmentCardHolder developmentCardHolder;
        private readonly EventRaiser eventRaiser;
        private bool isQuitting;
        private readonly GameBoard gameBoard;
        private readonly INumberGenerator numberGenerator;
        private Dictionary<Guid, IPlayer> playersById;
        private int playerIndex;
        private IPlayer[] players;
        private uint dice1, dice2;
        private IGameTimer turnTimer;
        private Func<Guid> idGenerator;
        private ITokenManager tokenManager;
        private ILog log;

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

        private event Action<Exception> GameExceptionEvent;

        public void SetTurnTimer(IGameTimer turnTimer)
        {
            if (turnTimer != null)
                this.turnTimer = turnTimer;
        }

        public void SetIdGenerator(Func<Guid> idGenerator)
        {
            if (idGenerator != null)
                this.idGenerator = idGenerator;
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

        public void AddResourcesToPlayer(string playerName, ResourceClutch value)
        {
            // TODO: Return an error if player not found?
            this.players
                .Where(p => p.Name == playerName)
                .FirstOrDefault()
                ?.AddResources(value);
        }

        public void Quit()
        {
            this.isQuitting = true;
            this.eventRaiser.CanRaiseEvents = false;
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
            var token = this.tokenManager.GetNewToken(player);
            // TODO: Pass back current settled locations
            this.eventRaiser.RaiseEvent(new PlaceSetupInfrastructureEvent(), player.Id, token);
            while (true)
            {
                var playerAction = this.WaitForPlayerAction();
                this.turnTimer.Reset();

                if (playerAction is EndOfTurnAction)
                {
                    break;
                }
                else if (playerAction is PlaceInfrastructureAction placeInfrastructureAction)
                {
                    this.PlaceInfrastructure(player, placeInfrastructureAction.SettlementLocation, placeInfrastructureAction.RoadEndLocation);
                    break;
                }
            }
        }

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
                    return playerAction;
            }
        }

        private void PlayerActionEventHandler(GameToken token, PlayerAction playerAction)
        {
            this.tokenManager.ValidatePlayerAction(token);

            this.actionRequests.Enqueue( playerAction);
        }

        private void ProcessPlayerAction(PlayerAction playerAction)
        {
            if (playerAction is AnswerDirectTradeOfferAction answerDirectTradeOfferAction)
            {
                var answerDirectTradeOfferEvent = new AnswerDirectTradeOfferEvent(Guid.Empty,
                    answerDirectTradeOfferAction.PlayerId,
                    answerDirectTradeOfferAction.OfferedResources);

                this.eventRaiser.RaiseEvent(answerDirectTradeOfferEvent, this.PlayerIdsExcept(answerDirectTradeOfferAction.PlayerId));
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
                var otherPlayers = this.PlayerIdsExcept(playerAction.PlayerId);
                this.eventRaiser.RaiseEvent(makeDirectTradeOfferEvent, otherPlayers);
                return;
            }
        }

        private IEnumerable<Guid> PlayerIdsExcept(Guid playerId) => this.playersById.Select(kv => kv.Key).Where(id => id != playerId);

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
                this.eventRaiser.RaiseEvent(diceRollEvent, this.PlayerIdsExcept(this.currentPlayer.Id));

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

        #region Structures
        public interface ITokenManager
        {
            bool ValidatePlayerAction(GameToken token);
            GameToken GetNewToken(IPlayer player);
            IPlayer GetPlayer(GameToken token);
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

            public void RaiseEvent(GameEvent gameEvent, IEnumerable<Guid> playerIds)
            {
                if (!this.CanRaiseEvents)
                    return;

                foreach (var playerId in playerIds)
                    this.gameEventHandlersByPlayerId[playerId].Invoke(gameEvent, null);
            }
        }

        private class TokenManager: ITokenManager
        {
            private Dictionary<GameToken, IPlayer> playerByToken = new Dictionary<GameToken, IPlayer>();
            private Dictionary<IPlayer, GameToken> tokenByPlayer = new Dictionary<IPlayer, GameToken>();

            public GameToken GetNewToken(IPlayer player)
            {
                if (this.tokenByPlayer.ContainsKey(player))
                {
                    var existingToken = this.tokenByPlayer[player];
                    this.tokenByPlayer.Remove(player);
                    if (this.playerByToken.ContainsKey(existingToken))
                    {
                        this.playerByToken.Remove(existingToken);
                    }
                }

                var token = new GameToken();
                this.tokenByPlayer.Add(player, token);
                this.playerByToken.Add(token, player);
                return token;
            }

            public IPlayer GetPlayer(GameToken token)
            {
                return this.playerByToken[token];
            }

            public bool ValidatePlayerAction(GameToken token)
            {
                return this.playerByToken.ContainsKey(token);
            }
        }
        #endregion
    }

    public interface ILog
    {
        void Add(string message);
        void WriteToFile(string filePath);
    }
}
