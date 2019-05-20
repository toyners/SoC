
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerStateInstruction : Instruction
    {
        private readonly PlayerAgent playerAgent;
        private readonly ScenarioRunner runner;
        private ResourceClutch? resources;
        private uint? roadSegmentCount;
        private uint? victoryPoints;

        public PlayerStateInstruction(PlayerAgent playerAgent, ScenarioRunner runner)
        {
            this.runner = runner;
            this.playerAgent = playerAgent;
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
            this.roadSegmentCount = roadSegmentCount;
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
            requestStateEvent.Resources = this.resources.Value;
            requestStateEvent.RoadSegment = this.roadSegmentCount.Value;
            requestStateEvent.VictoryPoints = this.victoryPoints.Value;
            return requestStateEvent;
        }

        
    }
}