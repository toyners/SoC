
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlaceRobberAction : PlayerAction
    {
        #region Fields
        public readonly uint RobberHex;
        #endregion

        #region Construction
        public PlaceRobberAction(uint robberHex) : base(Guid.Empty)
        {
            this.RobberHex = robberHex;
        }
        #endregion
    }
}
