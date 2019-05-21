namespace SoC.Library.ScenarioTests
{
    internal class PlacedRoadSegmentSetup : IPlayerSetupAction
    {
        private uint placedRoadSegments;
        public PlacedRoadSegmentSetup(uint value) => this.placedRoadSegments = value;
        public void Process(ScenarioPlayer player) => player.SetPlacedRoadSegments(this.placedRoadSegments);
    }
}