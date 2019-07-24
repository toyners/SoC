
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    internal class KnightCardSetup : IPlayerSetupAction
    {
        private readonly int cardCount;

        public KnightCardSetup(int cardCount) => this.cardCount = cardCount;
        public void Process(ScenarioPlayer player) => player.SetKnightCard(this.cardCount);
    }
}
