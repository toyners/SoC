
namespace SoC.Library.ScenarioTests.Instructions
{
    using System.Diagnostics;

    [DebuggerDisplay("Action: {Operation}")]
    public class ActionInstruction : Instruction
    {
        public enum OperationTypes
        {
            AcceptTrade,
            AnswerDirectTradeOffer,
            BuyDevelopmentCard,
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