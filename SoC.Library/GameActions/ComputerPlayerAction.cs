
namespace Jabberwocky.SoC.Library.GameActions
{
    using Enums;

    public class ComputerPlayerAction
    {
        public readonly ComputerPlayerActionTypes ActionType;

        public ComputerPlayerAction(ComputerPlayerActionTypes action)
        {
            this.ActionType = action;
        }
    }
}
