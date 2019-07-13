
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    using Jabberwocky.SoC.Library;

    internal class ResourceSetup : IPlayerSetupAction
    {
        private ResourceClutch resources;
        public ResourceSetup(ResourceClutch resources) => this.resources = resources;
        public void Process(ScenarioPlayer player) => player.AddResources(this.resources);
    }
}
