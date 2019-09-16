using System.Collections.Generic;

namespace SoC.WebApplication
{
    public class GameInfoListResponse : ResponseBase
    {
        public GameInfoListResponse(List<GameInfoResponse> gameInfo) => this.GameInfo = gameInfo;

        public List<GameInfoResponse> GameInfo { get; set; }
    }
}
