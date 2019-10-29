
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Generic;

    public class GameSessionDetails
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Owner { get; set; }
        public GameStatus Status { get; set; }
        public int NumberOfPlayers { get { return this.Players.Count; } }
        public int TotalBotCount { get; set; }
        public int TotalPlayerCount { get; set; }
        public int NumberOfSlots { get { return this.TotalPlayerCount - this.NumberOfPlayers; } }
        public int TurnTimeoutInSeconds { get; set; }
        public List<PlayerDetails> Players { get; set; } = new List<PlayerDetails>();
        public DateTime LaunchTime { get; set; }
        public bool PlayerStarts { get; set; }
    }
}
