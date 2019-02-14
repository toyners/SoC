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

        public GameServer(INumberGenerator numberGenerator, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder, IPlayerPool playerPool, GameOptions gameOptions = null)
        {
            
            throw new NotImplementedException();
        }

        public Action<Guid, uint, uint> DiceRollEvent { get; set; }
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
    }

    public class GameController
    {

    }

    public class ComputerPlayer2
    {
        public GameController GameController;
    }
}
