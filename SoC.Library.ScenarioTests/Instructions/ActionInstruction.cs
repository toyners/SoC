
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
            ChooseResourcesToLose,
            ConfirmStart,
            EndOfTurn,
            MakeDirectTradeOffer,
            PlaceCity,
            PlaceRoadSegment,
            PlaceRobber,
            PlaceSettlement,
            PlaceStartingInfrastructure,
            PlayKnightCard,
            RequestState,
            SelectResourceFromPlayer,
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