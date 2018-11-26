
using System.Collections.Generic;

namespace Jabberwocky.SoC.Library
{
    public class PlayerDataModel : PlayerDataBase
    {
        public int ResourceCards;
        public int HiddenDevelopmentCards;
        //public int LongestRoadSegmentCount; // TODO: Do we need this?
    }

    public class PlayerFullDataModel : PlayerDataBase
    {
        public List<DevelopmentCard> HiddenDevelopmentCards;
        public ResourceClutch Resources;
    }
}
