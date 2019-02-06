using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.PlayerData;
using SoC.Library.ScenarioTests.ScenarioActions;

namespace SoC.Library.ScenarioTests
{
    public class ScenarioComputerPlayer : ComputerPlayer
    {
        #region Fields
        private readonly Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
        public readonly Queue<DevelopmentCard> BoughtDevelopmentCards = new Queue<DevelopmentCard>();
        private readonly Queue<ResourceClutch> resourcesToDrop = new Queue<ResourceClutch>();
        #endregion

        #region Construction
        public ScenarioComputerPlayer(string name, INumberGenerator numberGenerator) : base(name, numberGenerator) { }
        #endregion

        #region Methods
        public void AddAction(ComputerPlayerAction action)
        {
            this.ApplyPlayerAction(action);
        }

        public void AddActions(IEnumerable<ComputerPlayerAction> actions)
        {
            foreach (var action in actions)
                this.AddAction(action);
        }

        private Queue<object> instructions;
        public void AddInstructions(IEnumerable<object> instructions)
        {
            this.instructions = new Queue<object>(instructions);
        }

        public override void AddDevelopmentCard(DevelopmentCard developmentCard)
        {
            base.AddDevelopmentCard(developmentCard);
            this.BoughtDevelopmentCards.Enqueue(developmentCard);
        }

        public void AddResourcesToDrop(ResourceClutch resourcesToDrop)
        {
            this.resourcesToDrop.Enqueue(resourcesToDrop);
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

        List<GameEvent> actualEvents = new List<GameEvent>();
        internal void AddEvent(GameEvent gameEvent)
        {
            this.actualEvents.Add(gameEvent);
        }

        public override uint ChooseRobberLocation()
        {
            return 0;
        }

        public override ResourceClutch ChooseResourcesToDrop()
        {
            if (this.resourcesToDrop.Count == 0)
                throw new Exception("No resources have been set up to be dropped");
            var resourcesToDrop = this.resourcesToDrop.Dequeue();
            return resourcesToDrop;
        }

        public void AddDropResourcesAction(DropResourcesAction dropResourcesAction)
        {
            this.dropResourcesActions.Enqueue(dropResourcesAction);
        }

        private Queue<DropResourcesAction> dropResourcesActions = new Queue<DropResourcesAction>();
        public override DropResourcesAction GetDropResourcesAction()
        {
            return this.dropResourcesActions.Count > 0 ? this.dropResourcesActions.Dequeue() : null;
        }

        public override ComputerPlayerAction GetPlayerAction()
        {
            if (this.actions.Count == 0)
                return null;

            ComputerPlayerAction action = null;
            do
            {
                action = this.actions.Dequeue();
                if (action is ScenarioVerifySnapshotAction scenarioVerifySnapshotAction)
                {
                    scenarioVerifySnapshotAction.Verify();
                    action = null;
                }
            } while (this.actions.Count > 0 && action == null);

            return action;
            /*ComputerPlayerAction playerAction = null;
            while (this.instructions.Count > 0)
            {
                var obj = this.instructions.Peek();
                if (obj is GameEvent)
                    break;

                if (obj is ComputerPlayerAction action)
                {
                    if (playerAction != null)
                        break;

                    playerAction = action;
                    this.instructions.Dequeue();
                }
                else if (obj is PlayerSnapshot snapshot)
                {
                    this.instructions.Dequeue();
                }
            }

            return playerAction;*/
        }
        #endregion
    }
}
