using System;

namespace SoC.WebApplication
{
    public class CreateGameSessionResponse : ResponseBase
    {
        public CreateGameSessionResponse(Guid gameId) => this.GameId = gameId;

        public Guid GameId { get; set; }
    }
}
