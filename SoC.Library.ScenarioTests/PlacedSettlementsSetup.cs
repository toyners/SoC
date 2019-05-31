
namespace SoC.Library.ScenarioTests
{
    internal class PlacedSettlementsSetup : IPlayerSetupAction
    {
        private int placedSettlements;
        public PlacedSettlementsSetup(int value) => this.placedSettlements = value;
        public void Process(ScenarioPlayer player) => player.SetPlacedSettlements(this.placedSettlements);
    }
}
