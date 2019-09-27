
namespace SoC.WebApplication.Requests
{
    public class ConfirmGameJoinRequest : RequestBase
    {
        public string GameId { get; set; }
        public string PlayerId { get; set; }
    }
}
