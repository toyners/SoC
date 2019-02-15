﻿
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
    public class GameServer
    {
        private ConcurrentQueue<ComputerPlayerAction> actionRequests = new ConcurrentQueue<ComputerPlayerAction>();
        private IPlayer currentPlayer;
        private TurnToken currentTurnToken;
        private bool isQuitting;
        private GameBoard gameBoard;
        private INumberGenerator numberGenerator;
        private Dictionary<Guid, IPlayer> playersById;
        private int playerIndex;
        private IPlayer[] players;
        private uint dice1, dice2;

        public GameServer(INumberGenerator numberGenerator, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder)
        {
            throw new NotImplementedException();
        }

        public event Action<Guid, uint, uint> DiceRollEvent;
        public Action<ResourcesCollectedEvent> ResourcesCollectedEvent { get; set; }
        public Action<TurnToken> StartPlayerTurnEvent { get; set; }

        public void StartGame()
        {
            // Launch server processing on separate thread
            Task.Factory.StartNew(() =>
            {
                this.GameLoop();
            });
        }

        private void GameLoop()
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
                this.ResourcesCollectedEvent?.Invoke(resourcesCollectedEvent);
            }
        }

        public void AddController(GameController gameController)
        {

            throw new NotImplementedException();
        }

        private void StartTurn()
        {
            this.ChangeToNextPlayerTurn();
            this.currentTurnToken = new TurnToken();
            this.StartPlayerTurnEvent?.Invoke(this.currentTurnToken);
            this.numberGenerator.RollTwoDice(out this.dice1, out this.dice2);
            this.DiceRollEvent?.Invoke(this.currentPlayer.Id, this.dice1, this.dice2);

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


        internal void JoinGame(Player2 player, GameController gameController)
        {
            if (player is HumanPlayer)
                this.DiceRollEvent += gameController.DiceRollEventHandler;
        }
    }

    public class GameController
    {
        internal void DiceRollEventHandler(Guid arg1, uint arg2, uint arg3)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Player2
    {

    }

    public class ComputerPlayer2 : Player2
    {
        private GameController GameController;

        private string playerName;
        public ComputerPlayer2(string playerName)
        {
            this.playerName = playerName;
            this.GameController = new GameController();
        }

        public void JoinGame(GameServer gameServer)
        {
            gameServer.JoinGame(this, this.GameController);
        }
    }

    public class HumanPlayer : Player2
    {
        private GameController GameController;
        private string playerName;
        public HumanPlayer(string playerName)
        {
            this.playerName = playerName;
            this.GameController = new GameController();
        }

        public void JoinGame(GameServer gameServer)
        {
            gameServer.JoinGame(this, this.GameController);
        }
    }
}
