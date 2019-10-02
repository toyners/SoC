
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.PlayerActions;
    using Microsoft.AspNetCore.SignalR;
    using SoC.WebApplication.Hubs;
    using SoC.WebApplication.Requests;

    public class GamesAdministrator : IGamesAdministrator, IPlayerRequestReceiver
    {
        private readonly CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IHubContext<GameHub> gameHubContext;
        private readonly ConcurrentDictionary<Guid, GameManagerToken> inPlayGames = new ConcurrentDictionary<Guid, GameManagerToken>();
        private readonly ConcurrentQueue<GameRequest> gameRequests = new ConcurrentQueue<GameRequest>();
        private readonly ConcurrentDictionary<Guid, GameDetails> gamesToLaunchById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly Task launchGameTask;
        private readonly Task mainGameTask;
        private readonly INumberGenerator numberGenerator = new NumberGenerator();

        public GamesAdministrator(IHubContext<GameHub> gameHubContext)
        {
            this.gameHubContext = gameHubContext;
            this.cancellationToken = this.cancellationTokenSource.Token;
            //this.launchGameTask = Task.Factory.StartNew(o => { this.LaunchGame(); }, null, this.cancellationToken);
            this.mainGameTask = Task.Factory.StartNew(o => { this.ProcessInPlayGames(); }, null, this.cancellationToken);
        }

        public void AddGame(GameDetails gameDetails)
        {
            this.gamesToLaunchById.TryAdd(gameDetails.Id, gameDetails);
        }

        public void ConfirmGameJoin(ConfirmGameJoinRequest confirmGameJoinRequest)
        {
            var gameId = Guid.Parse(confirmGameJoinRequest.GameId);
            if (this.gamesToLaunchById.TryGetValue(gameId, out var gameDetails))
            {
                var playerId = Guid.Parse(confirmGameJoinRequest.PlayerId);
                var player = gameDetails.Players.First(pd => pd.Id.Equals(playerId));
                player.ConnectionId = confirmGameJoinRequest.ConnectionId;

                var playerWithoutConnectionId = gameDetails.Players.FirstOrDefault(pd => pd.ConnectionId == null);
                if (playerWithoutConnectionId == null)
                {
                    var gameManagerToken = this.LaunchGame(gameDetails);
                    this.inPlayGames.GetOrAdd(gameDetails.Id, gameManagerToken);
                }
            }
        }

        public void PlayerAction(PlayerActionRequest playerActionRequest)
        {
            if (this.inPlayGames.TryGetValue(playerActionRequest.GameId, out var gameManagerToken))
            {
                PlayerAction playerAction = null;
                gameManagerToken.GameManager.Post(playerAction);
            }
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        private void ProcessInPlayGames()
        {
            try
            {
                while (true)
                {
                    while (this.gameRequests.TryDequeue(out var request))
                    {
                        if (!this.inPlayGames.TryGetValue(request.GameId, out var game))
                        {
                            // Game missing so handle this
                        }
                    }

                    Thread.Sleep(50);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private void LaunchGame()
        {
            try
            {
                while (true)
                {
                    /*while (this.gamesToLaunch.TryDequeue(out var gameDetails))
                    {
                        var gameManagerToken = this.LaunchGame(gameDetails);
                        this.inPlayGames.GetOrAdd(gameDetails.Id, gameManagerToken);
                    }*/

                    Thread.Sleep(50);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private GameManagerToken LaunchGame(GameDetails gameDetails)
        {
            var playerIds = new Queue<Guid>();

            var connectionIdsByPlayerId = new Dictionary<Guid, string>();
            gameDetails.Players.ForEach(player =>
            {
                connectionIdsByPlayerId.Add(player.Id, player.ConnectionId);
                playerIds.Enqueue(player.Id);
            });

            Dictionary<Guid, IEventReceiver> eventReceiversByPlayerId = null;
            if (gameDetails.TotalBotCount > 0)
            {
                eventReceiversByPlayerId = new Dictionary<Guid, IEventReceiver>();
                while (gameDetails.TotalBotCount-- > 0)
                {
                    var bot = new Bot(gameDetails.Id, this);
                    eventReceiversByPlayerId.Add(bot.Id, bot);
                    playerIds.Enqueue(bot.Id);
                }
            }

            
            var eventSender = new EventSender(this.gameHubContext, connectionIdsByPlayerId, eventReceiversByPlayerId);

            var gameManager = new GameManager(
                this.numberGenerator,
                new GameBoard(BoardSizes.Standard),
                new DevelopmentCardHolder(),
                new PlayerFactory(),
                eventSender,
                new GameOptions
                {
                    Players = gameDetails.NumberOfPlayers,
                    TurnTimeInSeconds = 120
                }
            );

            gameManager.SetIdGenerator(() => { return playerIds.Dequeue(); });

            gameDetails.Players.ForEach(player =>
            {
                gameManager.JoinGame(player.UserName);
            });

            var token = new GameManagerToken
            {
                GameManager = gameManager,
                GameManagerTask = gameManager.StartGameAsync()
            };

            return token;
        }

        private struct GameManagerToken
        {
            public GameManager GameManager;
            public Task GameManagerTask;
        }

        private class EventSender : IEventSender
        {
            private readonly IHubContext<GameHub> gameHubContext;
            private readonly Dictionary<Guid, string> connectionIdsByPlayerId;
            private readonly Dictionary<Guid, IEventReceiver> eventReceiversByPlayerId;
            public EventSender(IHubContext<GameHub> gameHubContext,
                Dictionary<Guid, string> connectionIdsByPlayerId,
                Dictionary<Guid, IEventReceiver> eventReceiversByPlayerId)
            {
                this.gameHubContext = gameHubContext;
                this.connectionIdsByPlayerId = connectionIdsByPlayerId;
                this.eventReceiversByPlayerId = eventReceiversByPlayerId;
            }

            public void Send(GameEvent gameEvent, Guid playerId)
            {
                if (this.eventReceiversByPlayerId.ContainsKey(playerId))
                {
                    this.eventReceiversByPlayerId[playerId].Post(gameEvent);
                }
                else if (this.connectionIdsByPlayerId.ContainsKey(playerId))
                {
                    var connectionId = this.connectionIdsByPlayerId[playerId];
                    this.gameHubContext.Clients.Client(connectionId).SendAsync("GameEvent", gameEvent);
                }
                else
                {
                    throw new NotImplementedException("Should not get here");
                }
            }
        }
    }
}
