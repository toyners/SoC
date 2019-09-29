using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SoC.WebApplication.Pages
{
    public class GameModel : PageModel
    {
        public void OnGet(string gameId, string playerId)
        {
            this.GameId = gameId;
            this.PlayerId = playerId;
        }

        public string GameId { get; set; }
        public string PlayerId { get; set; }
    }
}