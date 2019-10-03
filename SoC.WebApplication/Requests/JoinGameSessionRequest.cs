using System;

namespace SoC.WebApplication.Requests
{
    public class JoinGameSessionRequest : RequestBase
    {
        public Guid GameId { get; set; }
    }
}
