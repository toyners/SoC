
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    internal class RoadBuildingCardSetup : IPlayerSetupAction
    {
        private readonly int cardCount;
        public RoadBuildingCardSetup(int cardCount)
            => this.cardCount = cardCount;

        public void Process(ScenarioPlayer player) => player.SetRoadBuildingCard(this.cardCount);
    }
}
