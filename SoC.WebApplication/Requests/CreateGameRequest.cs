namespace SoC.WebApplication.Requests
{
    public class CreateGameSessionRequest : RequestBase
    {
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public int MaxBots { get; set; }
    }
}
