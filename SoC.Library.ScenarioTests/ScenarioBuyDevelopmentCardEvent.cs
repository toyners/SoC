using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests
{
    public class ScenarioBuyDevelopmentCardEvent : BuyDevelopmentCardEvent
    {
        private readonly DevelopmentCardTypes developmentCardType;
        private readonly ScenarioComputerPlayer player;

        public ScenarioBuyDevelopmentCardEvent(ScenarioComputerPlayer player, DevelopmentCardTypes developmentCardType) : base(player.Id)
        {
            this.developmentCardType = developmentCardType;
            this.player = player;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BuyDevelopmentCardEvent) || this.PlayerId != ((GameEvent)obj).PlayerId)
                return false;

            var developmentCard = this.player.BoughtDevelopmentCards.Dequeue();

            switch (this.developmentCardType)
            {
                case DevelopmentCardTypes.Knight: return developmentCard is KnightDevelopmentCard;
            }

            return false;
        }
    }
}
