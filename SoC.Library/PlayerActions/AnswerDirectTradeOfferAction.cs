
namespace Jabberwocky.SoC.Library.GameActions
{
    public class AnswerDirectTradeOfferAction : PlayerAction
    {
        public readonly string PlayerName;
        public readonly ResourceClutch OfferedResources;
        public AnswerDirectTradeOfferAction(string playerName, ResourceClutch offeredResources) : base()
        {
            this.PlayerName = playerName;
            this.OfferedResources = offeredResources;
        }
    }
}
