
namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using Jabberwocky.SoC.Library.GameActions;

    public abstract class ComputerPlayerActionWrapper : ComputerPlayerAction
    {
        public readonly string InitiatingPlayerName;
        public ComputerPlayerActionWrapper(string initiatingPlayerName, ComputerPlayerAction action) : base(0)
        {
            this.InitiatingPlayerName = initiatingPlayerName;
            this.Action = action;
        }

        public ComputerPlayerAction Action { get; private set; }
    }
}
