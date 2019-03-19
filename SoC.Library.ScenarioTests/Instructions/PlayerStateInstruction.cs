
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

        public PlayerStateInstruction(string playerName, ScenarioRunner runner) : base(playerName)
        {
            this.runner = runner;
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

        public GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            var requestStateEvent = new ScenarioRequestStateEvent(playerIdsByName[this.PlayerName]);
            requestStateEvent.Resources = this.expectedResources.Value;
            return requestStateEvent;
        }
    }
}