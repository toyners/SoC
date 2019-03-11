

namespace Jabberwocky.SoC.Library.GameEvents
{
    using Jabberwocky.SoC.Library.GameBoards;

    public class InitialBoardSetupEvent : GameEventWithSingleArgument<GameBoardSetup>
    {
        public InitialBoardSetupEvent(GameBoardSetup item) : base(item) { }

        public GameBoardSetup GameBoardSetup { get { return this.Item; } }
    }
}
