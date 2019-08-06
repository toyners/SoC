
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    using Jabberwocky.SoC.Library;

    class MonopolyCardSetup : CardSetup
    {
        public MonopolyCardSetup(int cardCount) : base(cardCount) { }
        public override void Process(ScenarioPlayer player)
            => player.SetHeldCard(this.cardCount, DevelopmentCardTypes.Monopoly);
    }
}
