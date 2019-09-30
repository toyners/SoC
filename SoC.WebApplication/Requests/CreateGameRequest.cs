namespace SoC.WebApplication.Requests
{
    public class CreateGameRequest : RequestBase
    {
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public int MaxBots { get; set; }
    }
}
