
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;
    using Newtonsoft.Json;

    public class PlayerAction
    {
        public PlayerAction(Guid initiatingPlayerId) => this.InitiatingPlayerId = initiatingPlayerId;

        [JsonProperty]
        public Guid InitiatingPlayerId { get; }
    }
}
