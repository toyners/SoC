
namespace SoC.Library.ScenarioTests
{
    using System.Diagnostics;

    [DebuggerDisplay("Action: {Operation}")]
    internal class ActionInstruction : Instruction
    {
        public enum OperationTypes
        {
            EndOfTurn,
            AnswerDirectTradeOffer,
            MakeDirectTradeOffer,
            PlaceStartingInfrastructure,
            RequestState
        }

        public readonly OperationTypes Operation;
        public readonly object[] Parameters;

        public ActionInstruction(string playerName, OperationTypes operaton, object[] parameters)
            : base(playerName)
        {
            this.Operation = operaton;
            this.Parameters = parameters;
        }
    }
}