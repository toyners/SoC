
using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class PlaceRobberAction : ComputerPlayerAction
    {
        public readonly uint RobberHex;
        public PlaceRobberAction(uint robberHex) : base(0)
        {
            this.RobberHex = robberHex;
        }
    }

    public class SelectResourceFromPlayerAction : ComputerPlayerAction
    {
        public readonly System.Guid PlayerId;
        public SelectResourceFromPlayerAction(System.Guid playerId) : base(0)
        {
            this.PlayerId = playerId;
        }
    }
}
