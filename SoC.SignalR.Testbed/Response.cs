using System;
using System.Collections.Generic;

namespace SoC.SignalR.Testbed
{
    public class Response
    {
    }

    public class GameInfoListResponse : Response
    {
        public GameInfoListResponse(List<GameInfoResponse> gameInfo) => this.GameInfo = gameInfo;

        public List<GameInfoResponse> GameInfo { get; set; }
    }

    public class GameInfoResponse : Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public GameStatus Status { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfSlots { get; set; }
    }

    public enum GameStatus
    {
        Open,
        Closed,
        Full,
        Playing
    }

    public class CreateGameResponse : Response
    {
        public CreateGameResponse(Guid gameId) => this.GameId = gameId;

        public Guid GameId { get; set; }
    }

    public class JoinGameResponse : Response
    {
        public JoinGameResponse(bool success) => this.Success = success;

        public bool Success { get; set; }
    }
}
