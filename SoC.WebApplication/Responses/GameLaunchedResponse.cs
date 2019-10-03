using System;

namespace SoC.WebApplication
{
    public class GameLaunchedResponse : CreateGameSessionResponse
    {
        public GameLaunchedResponse(Guid gameId, Guid playerId) : base(gameId) => this.PlayerId = playerId;

        public Guid PlayerId { get; set; }
    }
}
