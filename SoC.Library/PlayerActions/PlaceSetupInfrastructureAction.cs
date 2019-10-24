
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;
    using Newtonsoft.Json;

    public class PlaceSetupInfrastructureAction : PlayerAction
    {
        public PlaceSetupInfrastructureAction(Guid playerId, uint settlementLocation, uint roadEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }

        [JsonProperty]
        public uint SettlementLocation { get; }

        [JsonProperty]
        public uint RoadEndLocation { get; }
    }
}
