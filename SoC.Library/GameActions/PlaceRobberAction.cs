
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
}
