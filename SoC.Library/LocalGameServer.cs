
namespace Jabberwocky.SoC.Library
{
    using System.Collections.Concurrent;
    using System.Threading;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.Interfaces;

    public class LocalGameServer
    {
        private bool requestQuit;
        private ConcurrentQueue<ComputerPlayerAction> actionRequests = new ConcurrentQueue<ComputerPlayerAction>();
        private INumberGenerator numberGenerator;
        private IPlayerPool computerPlayerFactory;
        private GameBoard gameBoard;
        private IDevelopmentCardHolder developmentCardHolder;

        internal LocalGameServer(INumberGenerator numberGenerator, IPlayerPool computerPlayerFactory, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder)
        {
            this.numberGenerator = numberGenerator;
            this.computerPlayerFactory = computerPlayerFactory;
            this.gameBoard = gameBoard;
            this.developmentCardHolder = developmentCardHolder;
        }

        internal void AddPlayerAction(ComputerPlayerAction playerAction)
        {
            this.actionRequests.Enqueue(playerAction);
        }

        internal void Start()
        {
            while (true)
            {
                Thread.Sleep(50);
                if (this.requestQuit)
                    return;

                if (this.actionRequests.TryDequeue(out var playerAction))
                    this.ProcessPlayerAction(playerAction);
            }
        }

        internal void Quit()
        {
            this.requestQuit = true;
        }

        private void ProcessPlayerAction(ComputerPlayerAction playerAction)
        {
            
        }
    }
}
