using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.PlayerData;

namespace SoC.Library.ScenarioTests
{
    public class MockComputerPlayer : ComputerPlayer
    {
        private PlaceInfrastructureInstruction firstInstruction;
        private PlaceInfrastructureInstruction secondInstruction;
        private readonly List<ComputerPlayerAction> actions = new List<ComputerPlayerAction>();

        public MockComputerPlayer(string name, INumberGenerator numberGenerator) : base(name, numberGenerator) { }

        public void AddSetupInstructions(PlaceInfrastructureInstruction firstInstruction, PlaceInfrastructureInstruction secondInstruction)
        {
            this.firstInstruction = firstInstruction;
            this.secondInstruction = secondInstruction;
        }

        public void AddInstructions(IEnumerable<ComputerPlayerAction> actions)
        {
            this.actions.AddRange(actions);
        }

        public override void BuildInitialPlayerActions(PlayerDataModel[] otherPlayerData)
        {
            
        }

        public override void ChooseInitialInfrastructure(out uint settlementLocation, out uint roadEndLocation)
        {
            if (this.firstInstruction != null)
            {
                settlementLocation = this.firstInstruction.SettlementLocation;
                roadEndLocation = this.firstInstruction.RoadEndLocation;
                this.firstInstruction = null;
            }
            else
            {
                settlementLocation = this.secondInstruction.SettlementLocation;
                roadEndLocation = this.secondInstruction.RoadEndLocation;
            }
        }

        public override ComputerPlayerAction GetPlayerAction()
        {
            return base.GetPlayerAction();
        }
    }
}
