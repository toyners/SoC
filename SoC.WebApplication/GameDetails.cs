
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Generic;

    public class GameDetails
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Owner { get; set; }
        public GameStatus Status { get; set; }
        public int NumberOfPlayers { get { return this.Players.Count; } }
        public int TotalPlayerCount { get; set; }
        public int NumberOfSlots { get { return 2 - this.NumberOfPlayers; } }
        public List<PlayerDetails> Players { get; set; } = new List<PlayerDetails>();
        public DateTime LaunchTime { get; set; }
    }
}
