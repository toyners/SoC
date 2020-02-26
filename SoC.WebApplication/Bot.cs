
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.PlayerActions;
    using Newtonsoft.Json;
    using SoC.WebApplication.Requests;

    public class Bot : IEventReceiver
    {
        private readonly GameBoardQuery gameBoardQuery;
        private readonly ConcurrentQueue<GameEvent> gameEvents = new ConcurrentQueue<GameEvent>();
        private readonly Guid gameId;
        private readonly Task processingTask;
        private readonly IPlayerRequestReceiver playerActionReceiver;
        private IDictionary<string, Guid> playerIdsByName;
        private JsonSerializerSettings jsonSerializerSettings;
        private ResourceClutch resources = ResourceClutch.Zero;

        public Bot(string name, Guid gameId, IPlayerRequestReceiver playerActionReceiver, GameBoardQuery gameBoardQuery)
        {
            this.Name = name;
            this.gameId = gameId;
            this.playerActionReceiver = playerActionReceiver;
            this.gameBoardQuery = gameBoardQuery;

            this.jsonSerializerSettings = new JsonSerializerSettings();
            this.jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            this.jsonSerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            this.jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;

            this.processingTask = Task.Factory.StartNew(o => { this.ProcessAsync(); }, CancellationToken.None);
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }

        public void Post(GameEvent gameEvent)
        {
            this.gameEvents.Enqueue(gameEvent);
        }

        private void Send(PlayerAction request)
        {
            var requestPayload = JsonConvert.SerializeObject(request, this.jsonSerializerSettings);
            var playerActionType = request.GetType().ToString();
            this.playerActionReceiver.PlayerAction(new PlayerActionRequest
            {
                GameId = this.gameId,
                PlayerId = this.Id,
                PlayerActionType = playerActionType,
                Data = requestPayload,
            });
        }

        private void ProcessAsync()
        {
            while (true)
            {
                while (this.gameEvents.TryDequeue(out var gameEvent))
                {
                    PlayerAction request = null;
                    if (gameEvent is ConfirmGameStartEvent)
                    {
                        this.Send(new ConfirmGameStartAction(this.Id));
                        continue;
                    }

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
                        this.Send(new PlaceSetupInfrastructureAction(this.Id, locations[0], locations[0] + 1));
                        continue;
                    }

                    if (gameEvent is StartTurnEvent)
                    {
                        var startTurnEvent = (StartTurnEvent)gameEvent;
                        if (startTurnEvent.CollectedResources != null &&
                            startTurnEvent.CollectedResources.TryGetValue(this.Id, out var collectedResources))
                        {
                            foreach (var rc in collectedResources)
                                this.resources += rc.Resources;
                        }

                        if (startTurnEvent.PlayerId == this.Id)
                            this.Send(new EndOfTurnAction(this.Id));
                        continue;
                    }
                }

                Thread.Sleep(100);
            }
        }
    }
}
