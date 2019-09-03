using System;
using System.Collections.Generic;

namespace SoC.SignalR.Testbed
{
    public class Response
    {
    }

    public class GameInfoListResponse : Response
    {
        public GameInfoListResponse(List<GameInfo> gameInfo) => this.GameInfo = gameInfo;

        public List<GameInfo> GameInfo { get; set; }
    }

    public class GameInfo
    {
        public enum GameStatus
        {
            Open,
            Closed,
            Full,
            Playing
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string InitiatingPlayer { get; set; }
        public GameStatus Status { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfSlots { get; set; }
    }

    public class CreateGameResponse : Response
    {
        public CreateGameResponse(Guid gameId) => this.GameId = gameId;

        public Guid GameId { get; set; }
    }
}
