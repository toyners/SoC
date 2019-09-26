
using System;

namespace SoC.WebApplication
{
    public class PlayerDetails
    {
        public string ConnectionId { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public string UserName { get; set; }
    }
}
