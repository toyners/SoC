
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library;

    internal class AcceptDirectTradeEventInstruction : EventInstruction
    {
        public AcceptDirectTradeEventInstruction(string playerName, AcceptTradeEvent expectedEvent)
            : base(playerName, expectedEvent)
        {
        }
    }
}