
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;
    using Enums;

    public class ComputerPlayerAction
    {
        public readonly ComputerPlayerActionTypes ActionType;
        public readonly Guid PlayerId;

        public ComputerPlayerAction(ComputerPlayerActionTypes action)
        {
            this.ActionType = action;
        }

        public ComputerPlayerAction(Guid playerId)
        {
            this.PlayerId = playerId;
        }
    }
}
