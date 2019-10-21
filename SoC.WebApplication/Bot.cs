
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using SoC.WebApplication.Requests;

    public class Bot : IEventReceiver
    {
        private readonly GameBoardQuery gameBoardQuery;
        private readonly ConcurrentQueue<GameEvent> gameEvents = new ConcurrentQueue<GameEvent>();
        private readonly Guid gameId;
        private readonly Task processingTask;
        private readonly IPlayerRequestReceiver playerActionReceiver;
        private IDictionary<string, Guid> playerIdsByName;

        public Bot(string name, Guid gameId, IPlayerRequestReceiver playerActionReceiver, GameBoardQuery gameBoardQuery)
        {
            this.Name = name;
            this.gameId = gameId;
            this.playerActionReceiver = playerActionReceiver;
            this.gameBoardQuery = gameBoardQuery;

            this.processingTask = Task.Factory.StartNew(o => { this.ProcessAsync(); }, CancellationToken.None);
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }

        public void Post(GameEvent gameEvent)
        {
            this.gameEvents.Enqueue(gameEvent);
        }

        private void ProcessAsync()
        {
            while (true)
            {
                while (this.gameEvents.TryDequeue(out var gameEvent))
                {
                    string requestPayload = null;

                    if (gameEvent is GameJoinedEvent)
                    {
                        continue; // Nothing to do
                    }

                    if (gameEvent is PlayerSetupEvent)
                    {
                        this.playerIdsByName = ((PlayerSetupEvent)gameEvent).PlayerIdsByName;
                        continue;
                    }

                    if (gameEvent is InitialBoardSetupEvent)
                    {
                        continue;
                    }

                    if (gameEvent is PlayerOrderEvent)
                    {
                        continue;
                    }

                    if (gameEvent is PlaceSetupInfrastructureEvent)
                    {
                        var locations = this.gameBoardQuery.GetLocationsWithBestYield(1);
                        requestPayload = string.Format("{{ __typename: \\\"PlaceSetupInfrastructureAction\\\", settlementLocation: {0}, roadEndLocation: {1}}}",
                            locations[0],
                            locations[0] + 1);
                    }
                    
                    if (requestPayload != null)
                    {
                        this.playerActionReceiver.PlayerAction(new PlayerActionRequest
                        {
                            GameId = this.gameId,
                            PlayerId = this.Id,
                            Data = requestPayload,
                        });
                    }
                }

                Thread.Sleep(100);
            }
        }
    }
}
