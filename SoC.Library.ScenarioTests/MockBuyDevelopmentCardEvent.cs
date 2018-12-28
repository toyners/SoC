using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests
{
    public class MockBuyDevelopmentCardEvent : BuyDevelopmentCardEvent
    {
        public readonly DevelopmentCardTypes DevelopmentCardType;
        public MockBuyDevelopmentCardEvent(Guid playerId, DevelopmentCardTypes developmentCardType) : base(playerId)
        {
            this.DevelopmentCardType = developmentCardType;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            return obj is BuyDevelopmentCardEvent;
        }
    }
}
