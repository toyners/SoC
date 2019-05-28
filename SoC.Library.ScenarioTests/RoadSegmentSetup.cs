namespace SoC.Library.ScenarioTests
{
    internal class PlacedRoadSegmentSetup : IPlayerSetupAction
    {
        private int placedRoadSegments;
        public PlacedRoadSegmentSetup(int value) => this.placedRoadSegments = value;
        public void Process(ScenarioPlayer player) => player.SetPlacedRoadSegments(this.placedRoadSegments);
    }
}