
namespace Jabberwocky.SoC.Library
{
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.Interfaces;

    public class NewLocalGameController
    {
        private LocalGameServer localGameServer;
        private Task localGameServerTask;
        private INumberGenerator numberGenerator;
        private IPlayerPool computerPlayerFactory;
        private GameBoard gameBoard;
        private IDevelopmentCardHolder developmentCardHolder;

        public NewLocalGameController(INumberGenerator numberGenerator, IPlayerPool computerPlayerFactory, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder, bool provideFullPlayerData = false)
        {
            this.numberGenerator = numberGenerator;
            this.computerPlayerFactory = computerPlayerFactory;
            this.gameBoard = gameBoard;
            this.developmentCardHolder = developmentCardHolder;
        }

        public void JoinGame(GameOptions gameOptions)
        {
            // Validate request

            // Create server with all player initialization
            this.localGameServer = new LocalGameServer(this.numberGenerator, this.computerPlayerFactory, this.gameBoard, this.developmentCardHolder);

            // Launch server processing on separate thread
            this.localGameServerTask = Task.Factory.StartNew(() => 
            {
                this.localGameServer.Start();
            });
        }

        public void Quit()
        {
            // Validate request
            this.localGameServer.Quit();
        }
    }
}
