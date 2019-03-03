

namespace Jabberwocky.SoC.Library.GameEvents
{
    using Jabberwocky.SoC.Library.GameBoards;

    public class InitialBoardSetupEventArgs : GameEventArg<GameBoardSetup>
    {
        public InitialBoardSetupEventArgs(GameBoardSetup item) : base(item) { }
    }
}
