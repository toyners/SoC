using System;

namespace SoC.WebApplication.Requests
{
    public class GameRequest : RequestBase
    {
        public Guid GameId { get; set; }
    }
}
