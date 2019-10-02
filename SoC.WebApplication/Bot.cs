
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
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

        public Bot(Guid gameId, IPlayerRequestReceiver playerActionReceiver)
        {
            this.gameId = gameId;
            this.playerActionReceiver = playerActionReceiver;
            this.processingTask = Task.Factory.StartNew(o => { this.ProcessAsync(); }, CancellationToken.None);
        }

        public Guid Id { get; private set; } = Guid.NewGuid();

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
