
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerStateInstruction : Instruction
    {
        private readonly ScenarioRunner runner;
        private ResourceClutch? expectedResources;
        private PlayerAgent playerAgent;

        public PlayerStateInstruction(PlayerAgent playerAgent, ScenarioRunner runner)
        {
            this.runner = runner;
            this.playerAgent = playerAgent;
        }

        public PlayerStateInstruction HeldCards(DevelopmentCardTypes developmentCardType)
        {
            return this;
        }

        public PlayerStateInstruction VictoryPoints(uint victoryPoints)
        {
            return this;
        }

        public PlayerStateInstruction WithResources(ResourceClutch expectedResources)
        {
            this.expectedResources = expectedResources;
            return this;
        }

        public ScenarioRunner EndPlayerStateMeasuring()
        {
            this.playerAgent.AddInstruction(this);
            return this.runner;
        }

        public ActionInstruction GetAction()
        {
            return new ActionInstruction(ActionInstruction.OperationTypes.RequestState, null);
        }

        public GameEvent GetEvent()
        {
            var requestStateEvent = new ScenarioRequestStateEvent(this.playerAgent.Id);
            requestStateEvent.Resources = this.expectedResources.Value;
            return requestStateEvent;
        }
    }
}