
namespace Jabberwocky.SoC.Library.GameActions
{
    public class PlaceRobberAction : ComputerPlayerAction
    {
        #region Fields
        public readonly uint RobberHex;
        #endregion

        #region Construction
        public PlaceRobberAction(uint robberHex) : base(0)
        {
            this.RobberHex = robberHex;
        }
        #endregion
    }
}
