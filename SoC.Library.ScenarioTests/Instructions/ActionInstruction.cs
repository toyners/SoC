
namespace SoC.Library.ScenarioTests.Instructions
{
    using System.Diagnostics;

    [DebuggerDisplay("Action: \"{Operation}\"")]
    internal class ActionInstruction : Instruction
    {
        public enum OperationTypes
        {
            AcceptTrade,
            AnswerDirectTradeOffer,
            ConfirmDirectTrade,
            EndOfTurn,
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