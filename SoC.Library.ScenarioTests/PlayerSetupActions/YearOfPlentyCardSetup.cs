
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    using Jabberwocky.SoC.Library;

    internal class YearOfPlentyCardSetup : CardSetup
    {
        public YearOfPlentyCardSetup(int cardCount) : base(cardCount) { }

        public override void Process(ScenarioPlayer player) =>
            player.SetHeldCard(this.cardCount, DevelopmentCardTypes.YearOfPlenty);
    }
}
