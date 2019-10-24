
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;

    public class PlayerSetupEvent : GameEvent
    {
        public PlayerSetupEvent(string[] playerNames, IDictionary<string, Guid> playerIdsByName) : base(Guid.Empty)
        {
            if (playerIdsByName == null || playerIdsByName.Count == 0)
                throw new ArgumentNullException("playerIdsByName");

            this.PlayerNames = playerNames;
            this.PlayerIdsByName = playerIdsByName;
        }

        public IDictionary<string, Guid> PlayerIdsByName { get; set; }
        public string[] PlayerNames { get; set; }
    }
}
