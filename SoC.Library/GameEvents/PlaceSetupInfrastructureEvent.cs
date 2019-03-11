
namespace Jabberwocky.SoC.Library.GameEvents
{
    public class PlaceSetupInfrastructureEvent : GameEventWithSingleArgument<GameToken>
    {
        public PlaceSetupInfrastructureEvent(GameToken item) : base(item) {}

        public GameToken TurnToken { get { return this.Item; } }
    }
}
