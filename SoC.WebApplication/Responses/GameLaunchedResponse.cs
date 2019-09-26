using System;

namespace SoC.WebApplication
{
    public class GameLaunchedResponse : CreateGameResponse
    {
        public GameLaunchedResponse(Guid gameId, Guid playerId) : base(gameId) => this.PlayerId = playerId;

        public Guid PlayerId { get; set; }
    }
}
