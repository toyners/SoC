
namespace Jabberwocky.SoC.Library.GameActions
{
    public class PlaceRobberAction : PlayerAction
    {
        #region Fields
        public readonly uint RobberHex;
        #endregion

        #region Construction
        public PlaceRobberAction(uint robberHex) : base()
        {
            this.RobberHex = robberHex;
        }
        #endregion
    }
}
