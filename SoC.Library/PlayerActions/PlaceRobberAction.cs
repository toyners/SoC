
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlaceRobberAction : PlayerAction
    {
        #region Fields
        public readonly uint Hex;
        #endregion

        #region Construction
        public PlaceRobberAction(Guid playerId, uint hex) : base(playerId) => this.Hex = hex;
        #endregion
    }
}
