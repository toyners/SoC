﻿using System;
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
        private readonly Queue<ResourceClutch> resourcesToDrop = new Queue<ResourceClutch>();
        #endregion

        #region Construction
        public ScenarioComputerPlayer(string name, INumberGenerator numberGenerator) : base(name, numberGenerator) { }
        #endregion

        #region Methods
        public void AddAction(ComputerPlayerAction action)
        {
            this.actions.Enqueue(action);
        }

        public void AddActions(IEnumerable<ComputerPlayerAction> actions)
        {
            foreach (var action in actions)
                this.AddAction(action);
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

        public override ComputerPlayerAction GetPlayerAction()
        {
            return this.actions.Count > 0 ? this.actions.Dequeue() : null;
        }
        #endregion
    }
}
