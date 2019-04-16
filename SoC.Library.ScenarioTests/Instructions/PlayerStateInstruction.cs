
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerStateInstruction : Instruction
    {
        private readonly ScenarioRunner runner;
        private ResourceClutch? expectedResources;
        private Guid playerId;

        public PlayerStateInstruction(PlayerAgent player, ScenarioRunner runner) : base(player.Name)
        {
            this.runner = runner;
            this.playerId = player.Id;
        }

        public PlayerStateInstruction HeldCards(DevelopmentCardTypes developmentCardType)
        {
            return this;
        }

        public PlayerStateInstruction VictoryPoints(uint victoryPoints)
        {
            return this;
        }

        public PlayerStateInstruction Resources(ResourceClutch expectedResources)
        {
            this.expectedResources = expectedResources;
            return this;
        }

        public ScenarioRunner End() { return this.runner; }

        public ActionInstruction GetAction()
        {
            return new ActionInstruction(this.PlayerName, ActionInstruction.OperationTypes.RequestState, null);
        }

        public GameEvent GetEvent()
        {
            var requestStateEvent = new ScenarioRequestStateEvent(this.playerId);
            requestStateEvent.Resources = this.expectedResources.Value;
            return requestStateEvent;
        }
    }
}