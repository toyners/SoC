
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Library
{
    public class LocalGameServer
    {
        private ConcurrentQueue<ComputerPlayerAction> actionRequests = new ConcurrentQueue<ComputerPlayerAction>();
        private IPlayer currentPlayer;
        private TurnToken currentTurnToken;
        private IDevelopmentCardHolder developmentCardHolder;
        private bool isQuitting;
        private GameBoard gameBoard;
        private INumberGenerator numberGenerator;
        private Dictionary<Guid, IPlayer> playersById;
        private int playerIndex;
        private IPlayer[] players;
        private uint dice1, dice2;

        public LocalGameServer(INumberGenerator numberGenerator, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder)
        {
            this.numberGenerator = numberGenerator;
            this.gameBoard = gameBoard;
            this.developmentCardHolder = developmentCardHolder;
        }

        public event Action<GameEvent> GameEvent;
        //public event Action<Guid, uint, uint> DiceRollEvent;
        //public event Action<TurnToken> StartPlayerTurnEvent;
        //public event Action<GameBoardSetup> InitialBoardSetupEvent;
        //public Action<ResourcesCollectedEvent> ResourcesCollectedEvent { get; set; }

        private bool gotHumanPlayer;
        public void JoinGame(Player2 player, GameController gameController)
        {
            this.GameEvent += gameController.GameEventHandler;
            gameController.playerActionEvent += this.PlayerActionEventHandler;
            this.players[this.players.Length] = new Player(player.PlayerName);
        }

        private void PlayerActionEventHandler(TurnToken turnToken, ComputerPlayerAction obj)
        {
            throw new NotImplementedException();
        }

        public void LaunchGame(GameOptions gameOptions = null)
        {
            if (gameOptions == null)
                gameOptions = new GameOptions();

            this.players = new IPlayer[gameOptions.MaxPlayers];
        }

        public void StartGame()
        {
            // Complete setup
            if (this.gotHumanPlayer)
            {
                var gameBoardSetup = new GameBoardSetup(this.gameBoard);
                this.GameEvent.Invoke(new InitialBoardSetupEventArgs(gameBoardSetup));
            }

            this.players = PlayerTurnOrderCreator.Create(this.players, this.numberGenerator);

            // Notify human player what the order is?

            // Place first settlement
            for (int i = 0; i < this.players.Length; i++)
                // 1) Notify player to choose first settlement location (Pass in current locations)
                // 2) Pause waiting for player to return settlement choice
                ;

            // Place second settlement
            for (int i = this.players.Length - 1; i >= 0; i--)
                // Notify player to choose second settlement location (Pass in current locations)
                ;

            // Launch server processing on separate thread
            Task.Factory.StartNew(() =>
            {
                this.MainGameLoop();
            });
        }

        private void MainGameLoop()
        {
            this.playerIndex = -1;
            this.StartTurn();
            var pauseCount = 40;

            while (true)
            {
                Thread.Sleep(50);
                if (this.isQuitting)
                    return;

                var gotPlayerAction = this.actionRequests.TryDequeue(out var playerAction);

                if ((pauseCount == 0) ||
                    (gotPlayerAction && playerAction is EndOfTurnAction))
                {
                    this.StartTurn();
                    pauseCount = 40;
                    continue;
                }

                pauseCount--;

                if (!gotPlayerAction)
                    continue;

                // Player action to process
                this.ProcessPlayerAction(playerAction);
            }
        }

        private void ChangeToNextPlayerTurn()
        {
            this.playerIndex++;
            if (this.playerIndex == this.players.Length)
            {
                this.playerIndex = 0;
            }

            this.currentPlayer = this.players[this.playerIndex];
        }

        private void CollectResourcesAtStartOfTurn(uint resourceRoll)
        {
            var resources = this.gameBoard.GetResourcesForRoll(resourceRoll);
            foreach (var player in this.players)
            {
                if (!resources.TryGetValue(player.Id, out var resourcesCollectionForPlayer))
                    continue;

                var resourcesCollectionOrderedByLocation = resourcesCollectionForPlayer
                    .OrderBy(rc => rc.Location).ToArray();

                foreach (var resourceCollection in resourcesCollectionForPlayer)
                    player.AddResources(resourceCollection.Resources);

                var resourcesCollectedEvent = new ResourcesCollectedEvent(player.Id, resourcesCollectionOrderedByLocation);
                this.GameEvent.Invoke(resourcesCollectedEvent);
            }
        }

        private void StartTurn()
        {
            this.ChangeToNextPlayerTurn();
            this.currentTurnToken = new TurnToken();
            this.GameEvent.Invoke(new StartPlayerTurnEventArgs(this.currentTurnToken));
            this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
            this.GameEvent.Invoke(new DiceRollEventArgs(this.dice1, this.dice2));

            var resourceRoll = this.dice1 + this.dice2;
            if (resourceRoll != 7)
            {
                this.CollectResourcesAtStartOfTurn(resourceRoll);
            }
            else
            {

            }
        }

        private void ProcessPlayerAction(ComputerPlayerAction playerAction)
        {
            if (playerAction is MakeDirectTradeOfferAction)
            {
                foreach (var kv in this.playersById.Where(k => k.Key != playerAction.PlayerId).ToList())
                {
                }
            }
        }
    }

    public class GameController
    {
        public event Action<TurnToken, ComputerPlayerAction> playerActionEvent;

        internal void GameEventHandler(GameEvent gameEvent)
        {
            throw new NotImplementedException();
        }

        internal void InitialBoardSetupEventHandler(GameBoardSetup obj)
        {
            throw new NotImplementedException();
        }

        internal void StartPlayerTurnEventHandler(TurnToken turnToken)
        {
            throw new NotImplementedException();
        }
    }

    public class Player2
    {
        protected GameController gameController;

        public string PlayerName { get; private set; }

        public Player2(string playerName)
        {
            this.PlayerName = playerName;
            this.gameController = new GameController();
        }

        public void JoinGame(LocalGameServer gameServer)
        {
            gameServer.JoinGame(this, this.gameController);
        }
    }

    public class ComputerPlayer2 : Player2
    {
        public ComputerPlayer2(string playerName) : base(playerName)
        {
        }
    }

    public class HumanPlayer : Player2
    {
        public HumanPlayer(string playerName) : base(playerName)
        {
        }
    }

    public class InitialBoardSetupEventArgs : GameEventArg<GameBoardSetup>
    {
        public InitialBoardSetupEventArgs(GameBoardSetup item) : base(item) {}
    }

    public class StartPlayerTurnEventArgs : GameEventArg<TurnToken>
    {
        public StartPlayerTurnEventArgs(TurnToken item) : base(item) {}
    }

    public class DiceRollEventArgs : GameEvent
    {
        public uint Dice1, Dice2;
        public DiceRollEventArgs(uint dice1, uint dice2) : base(Guid.Empty)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
        }
    }

    public class GameEventArg<T> : GameEvent
    {
        public readonly T Item;
        public GameEventArg(T item) : base(Guid.Empty) => this.Item = item;
    }
}
