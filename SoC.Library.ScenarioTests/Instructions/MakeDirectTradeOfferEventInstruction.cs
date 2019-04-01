
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class MakeDirectTradeOfferEventInstruction : EventInstruction
    {
        public MakeDirectTradeOfferEventInstruction(string playerName, MakeDirectTradeOfferEvent expectedEvent)
            : base(playerName, expectedEvent)
        {
        }
    }
}
