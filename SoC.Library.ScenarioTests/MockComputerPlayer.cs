using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.PlayerData;

namespace SoC.Library.ScenarioTests
{
    public class MockComputerPlayer : ComputerPlayer
    {
        private PlaceInfrastructureInstruction firstInstruction;
        private PlaceInfrastructureInstruction secondInstruction;
        private readonly Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
        public readonly Queue<DevelopmentCard> BoughtDevelopmentCards = new Queue<DevelopmentCard>();

        #region Construction
        public MockComputerPlayer(string name, INumberGenerator numberGenerator) : base(name, numberGenerator) { }
        #endregion

        #region Methods
        public void AddActions(IEnumerable<ComputerPlayerAction> actions)
        {
            foreach (var action in actions)
                this.actions.Enqueue(action);
        }

        public override void AddDevelopmentCard(DevelopmentCard developmentCard)
        {
            base.AddDevelopmentCard(developmentCard);
            this.BoughtDevelopmentCards.Enqueue(developmentCard);
        }

        public void AddSetupInstructions(PlaceInfrastructureInstruction firstInstruction, PlaceInfrastructureInstruction secondInstruction)
        {
            this.firstInstruction = firstInstruction;
            this.secondInstruction = secondInstruction;
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

        public override uint ChooseRobberLocation()
        {
            return 0;
        }

        public override ComputerPlayerAction GetPlayerAction()
        {
            return this.actions.Count > 0 ? this.actions.Dequeue() : null;
        }
        #endregion
    }
}
