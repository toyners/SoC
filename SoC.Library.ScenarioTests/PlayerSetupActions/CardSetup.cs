
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    abstract class CardSetup : IPlayerSetupAction
    {
        protected readonly int cardCount;

        public CardSetup(int cardCount) => this.cardCount = cardCount;
        public abstract void Process(ScenarioPlayer player);
    }
}
