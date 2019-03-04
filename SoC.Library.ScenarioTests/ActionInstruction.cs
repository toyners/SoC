namespace SoC.Library.ScenarioTests
{
    internal class ActionInstruction : Instruction
    {
        public enum OperationTypes
        {
            EndOfTurn,
            AnswerDirectTradeOffer,
            MakeDirectTradeOffer,
            PlaceStartingInfrastructure
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