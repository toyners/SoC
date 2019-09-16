using System;

namespace SoC.WebApplication
{
    public class GameLaunchedResponse : CreateGameResponse
    {
        public GameLaunchedResponse(Guid gameId) : base(gameId) {}
    }
}
