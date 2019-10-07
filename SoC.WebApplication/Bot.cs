﻿
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using SoC.WebApplication.Requests;

    public class Bot : IEventReceiver
    {
        private readonly ConcurrentQueue<GameEvent> gameEvents = new ConcurrentQueue<GameEvent>();
        private readonly Guid gameId;
        private readonly Task processingTask;
        private readonly IPlayerRequestReceiver playerActionReceiver;
        private IDictionary<string, Guid> playerIdsByName;

        public Bot(string name, Guid gameId, IPlayerRequestReceiver playerActionReceiver)
        {
            this.Name = name;
            this.gameId = gameId;
            this.playerActionReceiver = playerActionReceiver;
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

                    var playerActionRequest = new PlayerActionRequest
                    {
                        GameId = this.gameId,
                        PlayerId = this.Id,
                        Data = "",
                    };
                    this.playerActionReceiver.PlayerAction(playerActionRequest);
                }

                Thread.Sleep(100);
            }
        }
    }
}
