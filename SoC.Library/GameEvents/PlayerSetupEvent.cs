
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;

    public class PlayerSetupEvent : GameEventWithSingleArgument<IDictionary<string, Guid>>
    {
        public PlayerSetupEvent(string[] playerNames, IDictionary<string, Guid> playerIdsByName) : base(playerIdsByName)
        {
            if (playerIdsByName == null || playerIdsByName.Count == 0)
                throw new ArgumentNullException("playerIdsByName");
        }

        public IDictionary<string, Guid> PlayerIdsByName { get { return this.Item; } }
    }
}
