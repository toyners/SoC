
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;

    internal class InfrastructureSetupBuilder
    {
        private readonly List<string> playerNames = new List<string>();
        private readonly Dictionary<string, List<Tuple<uint, uint>>> locationsByPlayerName = new Dictionary<string, List<Tuple<uint, uint>>>();
        public InfrastructureSetupBuilder Add(string playerName, uint settlementLocation, uint roadEndLocation)
        {
            if (!this.playerNames.Contains(playerName))
                this.playerNames.Add(playerName);

            if (!this.locationsByPlayerName.TryGetValue(playerName, out var list))
            {
                list = new List<Tuple<uint, uint>>();
                this.locationsByPlayerName.Add(playerName, list);
            }

            list.Add(new Tuple<uint, uint>(settlementLocation, roadEndLocation));
            return this;
        }

        public InfrastructureSetup Build()
        {
            var infrastructureSetup = new InfrastructureSetup();
            infrastructureSetup.PlayerOrder = this.playerNames.ToArray();

            infrastructureSetup.SetupLocations = new InfrastructureSetupLocations[infrastructureSetup.PlayerOrder.Length * 2];
            var sindex = 0;
            var eindex = infrastructureSetup.SetupLocations.Length - 1;

            foreach(var playerName in infrastructureSetup.PlayerOrder)
            {
                var locations = this.locationsByPlayerName[playerName];
                infrastructureSetup.SetupLocations[sindex++] = new InfrastructureSetupLocations
                {
                    PlayerName = playerName,
                    SettlementLocation = locations[0].Item1,
                    RoadEndLocation = locations[0].Item2
                };
                infrastructureSetup.SetupLocations[eindex--] = new InfrastructureSetupLocations
                {
                    PlayerName = playerName,
                    SettlementLocation = locations[1].Item1,
                    RoadEndLocation = locations[1].Item2
                };
            }

            return infrastructureSetup;
        }

        public class InfrastructureSetup
        {
            public string PlayerOneName { get { return this.PlayerOrder[0]; } }
            public string PlayerTwoName { get { return this.PlayerOrder[1]; } }
            public string PlayerThreeName { get { return this.PlayerOrder[2]; } }
            public string PlayerFourName { get { return this.PlayerOrder[3]; } }
            public string[] PlayerOrder { get; set; }
            public InfrastructureSetupLocations[] SetupLocations { get; set; }
        }

        public struct InfrastructureSetupLocations
        {
            public string PlayerName;
            public uint SettlementLocation, RoadEndLocation;
        }
    }
}
