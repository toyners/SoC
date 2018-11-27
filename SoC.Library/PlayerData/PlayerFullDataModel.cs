
namespace Jabberwocky.SoC.Library.PlayerData
{
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.Interfaces;

    public class PlayerFullDataModel : PlayerDataBase
    {
        public List<DevelopmentCard> HiddenDevelopmentCards;
        public ResourceClutch Resources;

        public PlayerFullDataModel(IPlayer player) : base(player)
        {
            this.Resources = player.Resources;
            // TODO: Hidden development cards
        }
    }
}
