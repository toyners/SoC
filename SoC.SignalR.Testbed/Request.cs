using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SoC.SignalR.Testbed
{
    public class Request
    {
        public IClientProxy ClientProxy { get; set; }
    }

    public class CreateGameRequest : Request
    {
        public string Name { get; set; }
    }

    public class JoinGameRequest : Request
    {
        public Guid GameId { get; set; }
    }

    public class GetWaitingGamesRequest : Request
    {
    }
}
