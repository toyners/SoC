
namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using Jabberwocky.SoC.Library.GameActions;

    public abstract class ComputerPlayerActionWrapper : ComputerPlayerAction
    {
        public ComputerPlayerActionWrapper(ComputerPlayerAction action) : base(0)
        {
            this.Action = action;
        }

        public ComputerPlayerAction Action { get; private set; }
    }
}
