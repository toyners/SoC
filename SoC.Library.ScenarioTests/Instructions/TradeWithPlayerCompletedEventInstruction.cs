
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class TradeWithPlayerCompletedEventInstruction : EventInstruction
    {
        public TradeWithPlayerCompletedEventInstruction(string playerName, TradeWithPlayerCompletedEvent expectedEvent)
            : base(playerName, expectedEvent)
        {
        }
    }
}