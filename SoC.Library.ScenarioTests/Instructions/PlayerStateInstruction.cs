
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;
    using SoC.Library.ScenarioTests.ScenarioEvents;

    internal class PlayerStateInstruction : Instruction
    {
        private readonly PlayerAgent playerAgent;
        private readonly ScenarioRunner runner;
        private uint? cities;
        private ResourceClutch? resources;
        private uint? roadSegments;
        private uint? settlements;
        private uint? victoryPoints;

        public PlayerStateInstruction(PlayerAgent playerAgent, ScenarioRunner runner)
        {
            this.runner = runner;
            this.playerAgent = playerAgent;
        }

        public PlayerStateInstruction Cities(uint cities)
        {
            this.cities = cities;
            return this;
        }

        public PlayerStateInstruction HeldCards(DevelopmentCardTypes developmentCardType)
        {
            return this;
        }

        public PlayerStateInstruction Resources(ResourceClutch expectedResources)
        {
            this.resources = expectedResources;
            return this;
        }

        public PlayerStateInstruction RoadSegments(uint roadSegmentCount)
        {
            this.roadSegments = roadSegmentCount;
            return this;
        }

        public PlayerStateInstruction Settlements(uint settlements)
        {
            this.settlements = settlements;
            return this;
        }

        public PlayerStateInstruction VictoryPoints(uint victoryPoints)
        {
            this.victoryPoints = victoryPoints;
            return this;
        }

        public ScenarioRunner End()
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
            requestStateEvent.Cities = this.cities;
            requestStateEvent.Resources = this.resources;
            requestStateEvent.RoadSegments = this.roadSegments;
            requestStateEvent.Settlements = this.settlements;
            requestStateEvent.VictoryPoints = this.victoryPoints;
            return requestStateEvent;
        }
    }
}