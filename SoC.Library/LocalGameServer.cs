
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
        private bool requestStateActionsMustHaveToken = true;
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
            this.eventRaiser = new EventRaiser();
            this.actionManager = new ActionManager();
        }
        #endregion

        public bool IsFinished { get; private set; }

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
            this.isQuitting = true;
            this.eventRaiser.CanRaiseEvents = false;
        }

        public void SaveLog(string filePath) => this.log.WriteToFile(filePath);

        public void SetIdGenerator(Func<Guid> idGenerator)
        {
            if (idGenerator != null)
                this.idGenerator = idGenerator;
        }

        public void SetRequestStateExemption(bool value)
        {
            this.requestStateActionsMustHaveToken = value;
        }

        public void SetTurnTimer(IGameTimer turnTimer)
        {
            if (turnTimer != null)
                this.turnTimer = turnTimer;
        }

        public Task StartGameAsync()
        {
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
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    this.log.Add($"ERROR: {e.Message}: {e.StackTrace}");
                    //this.GameExceptionEvent?.Invoke(e); TODO: Do I need this? Probably better to send close game
                    // messages instead
                    throw e;
                }
                finally
                {
                    this.IsFinished = true;
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
            var resourcesCollectedByPlayerId = this.gameBoard.GetResourcesForRoll(resourceRoll);
            var resourcesCollectedEvent = new ResourcesCollectedEvent(resourcesCollectedByPlayerId);
            this.RaiseEvent(resourcesCollectedEvent);
            /*foreach (var player in this.players)
            {
                if (!resourcesCollectedByPlayerId.TryGetValue(player.Id, out var resourcesCollectedForPlayer))
                    continue;

                var resourcesCollectedOrderedByLocation = resourcesCollectedForPlayer
                    .OrderBy(rc => rc.Location).ToArray();

                foreach (var resourceCollection in resourcesCollectedForPlayer)
                    player.AddResources(resourceCollection.Resources);

                var resourcesCollectedEvent = new ResourcesCollectedEvent(player.Id, resourcesCollectedOrderedByLocation);

                this.RaiseEvent(resourcesCollectedEvent, player);
                this.RaiseEvent(resourcesCollectedEvent, this.PlayersExcept(player.Id));
            }*/
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
            this.actionManager.SetExpectedActionsForPlayer(player.Id, typeof(PlaceSetupInfrastructureAction));
            this.RaiseEvent(placeSetupInfrastructureEvent, player, token);
            while (true)
            {
                var playerAction = this.WaitForPlayerAction();
                this.turnTimer.Reset();

                if (playerAction is PlaceSetupInfrastructureAction placeSetupInfrastructureAction)
                {
                    this.actionManager.SetExpectedActionsForPlayer(playerAction.InitiatingPlayerId, null);
                    var settlementLocation = placeSetupInfrastructureAction.SettlementLocation;
                    var roadEndLocation = placeSetupInfrastructureAction.RoadEndLocation;
                    this.PlaceInfrastructure(player, settlementLocation, roadEndLocation);
                    this.RaiseEvent(new InfrastructurePlacedEvent(playerAction.InitiatingPlayerId, settlementLocation, roadEndLocation));
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
                throw new TaskCanceledException();

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

            // Current player gets token to continue their turn
            var token = this.tokenManager.CreateNewToken(
                    this.playersById[this.currentPlayer.Id]);

            this.RaiseEvent(acceptTradeEvent, this.currentPlayer, token);
            this.RaiseEvent(acceptTradeEvent, this.PlayersExcept(this.currentPlayer.Id));
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
            this.RaiseEvent(
                answerDirectTradeOfferEvent,
                this.playersById[answerDirectTradeOfferAction.InitialPlayerId],
                token);

            // Other two players gets informational event
            var informationalAnswerDirectTradeOfferEvent = new AnswerDirectTradeOfferEvent(
                answerDirectTradeOfferAction.InitiatingPlayerId, answerDirectTradeOfferAction.WantedResources);

            var otherPlayers = this.PlayersExcept(
                    answerDirectTradeOfferAction.InitiatingPlayerId,
                    answerDirectTradeOfferAction.InitialPlayerId);

            this.RaiseEvent(informationalAnswerDirectTradeOfferEvent, otherPlayers);
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
                this.actionManager.SetExpectedActionsForPlayer(player.Id, typeof(AnswerDirectTradeOfferAction));
                this.RaiseEvent(makeDirectTradeOfferEvent, player, this.tokenManager.CreateNewToken(player));
            });
        }

        private void ProcessPlayerAction(PlayerAction playerAction)
        {
            if (playerAction is AcceptDirectTradeAction acceptDirectTradeAction)
            {
                this.ProcessAcceptDirectTradeAction(acceptDirectTradeAction);
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

                this.RaiseEvent(requestStateEvent, player);
                return;
            }

            throw new Exception($"Player action {playerAction.GetType()} not recognised.");
        }

        private void RaiseEvent(GameEvent gameEvent)
        {
            this.log.Add($"Sending {gameEvent.SimpleTypeName} to all players");
            this.eventRaiser.RaiseEvent(gameEvent);
        }

        private void RaiseEvent(GameEvent gameEvent, IEnumerable<IPlayer> otherPlayers)
        {
            var message = $"Sending {this.ToPrettyString(gameEvent)} " +
                $"to {string.Join(", ", otherPlayers.Select(player => player.Name))}";
            this.log.Add(message);
            this.eventRaiser.RaiseEvent(gameEvent, otherPlayers);
        }

        private void RaiseEvent(GameEvent gameEvent, IPlayer player, GameToken token = null)
        {
            if (token != null)
                this.log.Add($"Sending {this.ToPrettyString(gameEvent)} to {player.Name} with token {token}");
            else
                this.log.Add($"Sending {gameEvent.SimpleTypeName} to {player.Name} without token");
            this.eventRaiser.RaiseEvent(gameEvent, player.Id, token);
        }

        private void RaiseEventWithReusedToken(GameEvent gameEvent, IPlayer player)
        {
            var token = this.tokenManager.GetTokenForPlayer(player);
            this.RaiseEvent(gameEvent, player, token);
        }

        private void StartTurn()
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

        private void SendDiceRollEvent()
        {
            var diceRollEvent = new DiceRollEvent(this.currentPlayer.Id, this.dice1, this.dice2);
            
            this.RaiseEventWithReusedToken(diceRollEvent, this.currentPlayer);

            this.RaiseEvent(new DiceRollEvent(this.currentPlayer.Id, this.dice1, this.dice2),
                this.PlayersExcept(this.currentPlayer.Id));
        }

        private void SendStartPlayerTurnEvent()
        {
            //foreach (var player in this.PlayersExcept(this.currentPlayer.Id))
              //  this.actionManager.SetExpectedActionsForPlayer(player.Id, null);
            this.actionManager.SetExpectedActionsForPlayer(this.currentPlayer.Id, typeof(EndOfTurnAction));
            //var token = this.tokenManager.CreateNewToken(this.currentPlayer);
            this.RaiseEvent(new StartPlayerTurnEvent(), this.currentPlayer /*, token*/);
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
                    this.players = this.players.Where(player => player.Id == quitGameAction.InitiatingPlayerId).ToArray();
                    this.playersById.Remove(quitGameAction.InitiatingPlayerId);
                }
                else
                {
                    // Illegal command
                }
            }

            if (this.players.Length == 0)
                throw new TaskCanceledException();
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

                    if (playerAction is RequestStateAction && !this.requestStateActionsMustHaveToken)
                        return playerAction;

                    if (!this.actionManager.ValidateAction(playerAction))
                        throw new Exception($"FAILED: Action Validation - {this.playersById[playerAction.InitiatingPlayerId].Name}, {playerAction.GetType().Name}");

                    return playerAction;
                }
            }
        }

        private string ToPrettyString(GameEvent gameEvent)
        {
            var message = $"{gameEvent.SimpleTypeName}";
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
            GameToken GetTokenForPlayer(IPlayer player);
            bool ValidateToken(GameToken token);
        }
   
        public interface IActionManager
        {
            void SetExpectedActionsForPlayer(Guid playerId, params Type[] actionTypes);
            bool ValidateAction(PlayerAction playerAction);
        }

        private class ActionManager : IActionManager
        {
            private readonly Dictionary<Guid, Type> actionTypeByPlayerId = new Dictionary<Guid, Type>();
            private readonly Dictionary<Guid, HashSet<Type>> actionTypesByPlayerId = new Dictionary<Guid, HashSet<Type>>();

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

        private class EventRaiser
        {
            private Dictionary<Guid, Action<GameEvent, GameToken>> gameEventHandlersByPlayerId = new Dictionary<Guid, Action<GameEvent, GameToken>>();
            private event Action<GameEvent, GameToken> gameEventHandler;

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

            public IPlayer GetPlayerForToken(GameToken token) => this.playersByToken[token];

            public GameToken GetTokenForPlayer(IPlayer player) => this.tokensByPlayer[player];

            public bool ValidateToken(GameToken token) => 
                token != null && this.playersByToken.ContainsKey(token);
        }
        #endregion
    }
}
