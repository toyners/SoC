using System;

namespace SoC.SignalR.Testbed
{
    public class JoinGameRequest : RequestBase
    {
        public Guid GameId { get; set; }
    }
}
