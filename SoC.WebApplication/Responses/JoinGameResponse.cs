namespace SoC.SignalR.Testbed
{
    public class JoinGameResponse : ResponseBase
    {
        public JoinGameResponse(GameStatus status) => this.Status = status;

        public GameStatus Status { get; set; }
    }
}
