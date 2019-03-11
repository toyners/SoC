
namespace Jabberwocky.SoC.Library.GameEvents
{
    public class StartPlayerTurnEvent : GameEventWithSingleArgument<GameToken>
    {
        public StartPlayerTurnEvent(GameToken item) : base(item) {}

        public GameToken TurnToken { get { return this.Item; } }
    }
}
