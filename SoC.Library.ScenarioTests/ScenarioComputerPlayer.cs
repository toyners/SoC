using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.PlayerData;

namespace SoC.Library.ScenarioTests
{
    public class ScenarioComputerPlayer : ComputerPlayer
    {
        #region Fields
        private readonly Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
        public readonly Queue<DevelopmentCard> BoughtDevelopmentCards = new Queue<DevelopmentCard>();
        #endregion

        #region Construction
        public ScenarioComputerPlayer(string name, INumberGenerator numberGenerator) : base(name, numberGenerator) { }
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

        public void AddSetupInstructions(uint settlementLocation, uint roadSegmentEndLocation)
        {
            this.actions.Enqueue(new PlaceInfrastructureAction(settlementLocation, roadSegmentEndLocation));
        }

        public override void BuildInitialPlayerActions(PlayerDataModel[] otherPlayerData, bool rolledSeven)
        {
            //Do nothing
        }

        public override void ChooseInitialInfrastructure(out uint settlementLocation, out uint roadEndLocation)
        {
            var placeInfrastructureAction = (PlaceInfrastructureAction)this.actions.Dequeue();
            settlementLocation = placeInfrastructureAction.SettlementLocation;
            roadEndLocation = placeInfrastructureAction.RoadEndLocation;
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
