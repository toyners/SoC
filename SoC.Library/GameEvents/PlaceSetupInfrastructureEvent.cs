
namespace Jabberwocky.SoC.Library.GameEvents
{
    public class PlaceSetupInfrastructureEvent : GameEventWithSingleArgument<TurnToken>
    {
        public PlaceSetupInfrastructureEvent(TurnToken item) : base(item) {}

        public TurnToken TurnToken { get { return this.Item; } }
    }
}
