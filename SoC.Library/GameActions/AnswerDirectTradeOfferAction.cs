
namespace Jabberwocky.SoC.Library.GameActions
{
    public class AnswerDirectTradeOfferAction : ComputerPlayerAction
    {
        public readonly string PlayerName;
        public readonly ResourceClutch OfferedResources;
        public AnswerDirectTradeOfferAction(string playerName, ResourceClutch offeredResources) : base(0)
        {
            this.PlayerName = playerName;
            this.OfferedResources = offeredResources;
        }
    }
}
