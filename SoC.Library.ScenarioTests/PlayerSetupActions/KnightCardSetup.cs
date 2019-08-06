
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    using Jabberwocky.SoC.Library;

    internal class KnightCardSetup : CardSetup
    {
        public KnightCardSetup(int cardCount) : base(cardCount) { }
        public override void Process(ScenarioPlayer player) => 
            player.SetHeldCard(this.cardCount, DevelopmentCardTypes.Knight);
    }
}
