
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    using Jabberwocky.SoC.Library;

    internal class RoadBuildingCardSetup : CardSetup
    {
        public RoadBuildingCardSetup(int cardCount) : base(cardCount) { }
        public override void Process(ScenarioPlayer player) => 
            player.SetHeldCard(this.cardCount, DevelopmentCardTypes.RoadBuilding);
    }
}
