
namespace Jabberwocky.SoC.Library.GameActions
{
    public class MakeDirectTradeOfferAction : ComputerPlayerAction
    {
        public readonly ResourceClutch WantedResources;
        public MakeDirectTradeOfferAction(ResourceClutch wantedResources) : base(0)
        {
            this.WantedResources = wantedResources;
        }
    }
}
