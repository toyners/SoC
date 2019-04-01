
namespace SoC.Library.ScenarioTests.PlayerTurn
{
    using Jabberwocky.SoC.Library.GameEvents;
    using SoC.Library.ScenarioTests.Instructions;

    internal class MakeDirectTradeOfferEventInstruction : EventInstruction
    {
        public MakeDirectTradeOfferEventInstruction(string playerName, MakeDirectTradeOfferEvent expectedEvent)
            : base(playerName, expectedEvent)
        {
        }
    }

    internal class AnswerDirectTradeOfferEventInstruction : EventInstruction
    {
        public AnswerDirectTradeOfferEventInstruction(string playerName, AnswerDirectTradeOfferEvent expectedEvent)
            : base(playerName, expectedEvent)
        {
        }
    }
}