
namespace Jabberwocky.SoC.Library.GameEvents
{
    public class PlaceSetupInfrastructureEvent : GameEventArg<TurnToken>
    {
        public PlaceSetupInfrastructureEvent(TurnToken item) : base(item) {}
    }
}
