
namespace SoC.Library.ScenarioTests.Instructions
{
    using System.Diagnostics;

    [DebuggerDisplay("Action: {Operation}")]
    internal class ActionInstruction : Instruction
    {
        public enum OperationTypes
        {
            AcceptTrade,
            AnswerDirectTradeOffer,
            ConfirmStart,
            EndOfTurn,
            MakeDirectTradeOffer,
            PlaceCity,
            PlaceRoadSegment,
            PlaceSettlement,
            PlaceStartingInfrastructure,
            RequestState,
            QuitGame,
        }

        public readonly OperationTypes Operation;
        public readonly object[] Parameters;

        public ActionInstruction(OperationTypes operaton, object[] parameters)
        {
            this.Operation = operaton;
            this.Parameters = parameters;
        }
    }
}