using System;

namespace SoC.SignalR.Testbed
{
    public class GameLaunchedResponse : CreateGameResponse
    {
        public GameLaunchedResponse(Guid gameId) : base(gameId) {}
    }
}
