
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class AnswerDirectTradeOfferEventInstruction : EventInstruction
    {
        public AnswerDirectTradeOfferEventInstruction(string playerName, AnswerDirectTradeOfferEvent expectedEvent)
            : base(playerName, expectedEvent)
        {
        }
    }
}
