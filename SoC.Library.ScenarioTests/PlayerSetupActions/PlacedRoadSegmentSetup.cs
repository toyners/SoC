
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    internal class PlacedRoadSegmentSetup : IPlayerSetupAction
    {
        private int placedRoadSegments;
        public PlacedRoadSegmentSetup(int value) => this.placedRoadSegments = value;
        public void Process(ScenarioPlayer player) => player.SetPlacedRoadSegments(this.placedRoadSegments);
    }
}