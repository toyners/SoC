using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class AnswerDirectTradeOfferAction : ComputerPlayerAction
    {
        public readonly string PlayerName;
        public readonly ResourceClutch Resources;
        public AnswerDirectTradeOfferAction(string playerName, ResourceClutch resources) : base(0)
        {
            this.PlayerName = playerName;
            this.Resources = resources;
        }
    }
}
