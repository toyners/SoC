using System;

namespace SoC.WebApplication
{
    public class JoinGameResponse : ResponseBase
    {
        public JoinGameResponse(GameStatus status) => this.Status = status;

        public GameStatus Status { get; set; }
    }

    public class LaunchGameResponse : JoinGameResponse
    {
        public LaunchGameResponse(GameStatus status, Guid gameId, Guid playerId, string connectionId) : base(status)
        {
            this.GameId = gameId;
            this.PlayerId = playerId;
            this.ConnectionId = connectionId;
        }

        public string ConnectionId { get; set; }
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
    }
}
