
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    internal class KnightCardSetup : IPlayerSetupAction
    {
        public void Process(ScenarioPlayer player) => player.SetKnightCard();
    }
}
