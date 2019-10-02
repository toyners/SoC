
namespace SoC.WebApplication.Requests
{
    using System;

    public class PlayerActionRequest
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public string Data { get; set; }
    }
}
