
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Library.PlayerData
{
    public class PlayerDataModel : PlayerDataBase
    {
        public int ResourceCards;
        public int HiddenDevelopmentCards;
        //public int LongestRoadSegmentCount; // TODO: Do we need this?

        public PlayerDataModel(IPlayer player) : base(player)
        {
            this.ResourceCards = player.Resources.Count;
            // TODO: Hidden development cards
        }
    }
}
