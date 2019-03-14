
namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using System;
    using Jabberwocky.SoC.Library.GameActions;

    public abstract class ComputerPlayerActionWrapper : PlayerAction
    {
        public readonly string InitiatingPlayerName;
        public ComputerPlayerActionWrapper(string initiatingPlayerName, PlayerAction action) : base(Guid.Empty)
        {
            this.InitiatingPlayerName = initiatingPlayerName;
            this.Action = action;
        }

        public PlayerAction Action { get; private set; }
    }
}
