using System;

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

    public class CreateGameResponse : Response
    {
        public CreateGameResponse(Guid gameId) => this.GameId = gameId;

        public Guid GameId { get; set; }
    }
}
