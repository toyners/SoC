
namespace Jabberwocky.SoC.Library.GameEvents
{
    public class StartPlayerTurnEvent : GameEventWithSingleArgument<TurnToken>
    {
        public StartPlayerTurnEvent(TurnToken item) : base(item) {}

        public TurnToken TurnToken { get { return this.Item; } }
    }
}
