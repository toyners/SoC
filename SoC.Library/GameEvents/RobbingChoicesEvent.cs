using System;
using System.Collections.Generic;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class RobbingChoicesEvent : GameEvent
    {
        public Dictionary<Guid, int> RobbingChoices;
        public RobbingChoicesEvent(Guid playerId, Dictionary<Guid, int> robbingChoices) : base(playerId) => this.RobbingChoices = robbingChoices;
    }
}
