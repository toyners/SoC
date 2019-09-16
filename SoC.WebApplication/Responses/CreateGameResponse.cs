using System;

namespace SoC.WebApplication
{
    public class CreateGameResponse : ResponseBase
    {
        public CreateGameResponse(Guid gameId) => this.GameId = gameId;

        public Guid GameId { get; set; }
    }
}
