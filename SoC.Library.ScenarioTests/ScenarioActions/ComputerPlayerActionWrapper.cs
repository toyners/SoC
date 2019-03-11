
namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using Jabberwocky.SoC.Library.GameActions;

    public abstract class ComputerPlayerActionWrapper : PlayerAction
    {
        public readonly string InitiatingPlayerName;
        public ComputerPlayerActionWrapper(string initiatingPlayerName, PlayerAction action) : base(0)
        {
            this.InitiatingPlayerName = initiatingPlayerName;
            this.Action = action;
        }

        public PlayerAction Action { get; private set; }
    }
}
