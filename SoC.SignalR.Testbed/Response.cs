using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoC.SignalR.Testbed
{
    public class Response
    {
    }

    public class GameInfoListResponse : Response
    {
        public GameInfoListResponse(GameInfo[] gameInfo) => this.GameInfo = gameInfo;

        public GameInfo[] GameInfo { get; set; }
    }

    public class GameInfo
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfSlots { get; set; }
    }
}
