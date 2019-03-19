﻿
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using SoC.Library.ScenarioTests.Instructions;

    internal class ScenarioRunner
    {
        #region Fields
        private readonly ScenarioDevelopmentCardHolder developmentCardHolder = new ScenarioDevelopmentCardHolder();
        private readonly List<PlayerAgent> playerAgents = new List<PlayerAgent>();
        private readonly Dictionary<string, ResourceClutch> startingResourcesByName = new Dictionary<string, ResourceClutch>();
        private GameBoard gameBoard;
        private List<Instruction> instructions = new List<Instruction>();
        private bool useServerTimer = true;
        private ScenarioNumberGenerator numberGenerator;
        #endregion

        #region Construction
        private ScenarioRunner(string[] args)
        {
            if (args != null && args.Contains("NoTimer"))
                this.useServerTimer = false;

            this.numberGenerator = new ScenarioNumberGenerator();
        }
        #endregion

        #region Properties
        private string LastInstructionPlayerName
        {
            get { return this.instructions[this.instructions.Count - 1].PlayerName; }
        }
        #endregion

        #region Methods
        public static ScenarioRunner CreateScenarioRunner(string[] args = null)
        {
            return new ScenarioRunner(args);
        }

        public ScenarioRunner AnswerDirectTradeOffer(ResourceClutch wantedResources)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.AnswerDirectTradeOffer, 
                new object[] { wantedResources });
            return this;
        }

        public ScenarioRunner AcceptTrade()
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.AcceptTrade, null);
            return this;
        }

        public ScenarioRunner EndTurn()
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.EndOfTurn, null);
            return this;
        }

        public ScenarioRunner Label(string playerName, string label)
        {
            this.instructions.Add(new LabelInstruction(playerName, label));
            return this;
        }
        
        public ScenarioRunner MakeDirectTradeOffer(ResourceClutch wantedResources)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.MakeDirectTradeOffer, 
                new object[] { wantedResources });
            return this;
        }

        public ScenarioRunner PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceStartingInfrastructure,
                new object[] { settlementLocation, roadEndLocation });
            return this;
        }

        public void Run()
        {
            Thread.CurrentThread.Name = "Scenario Runner";

            var playerIds = new Queue<Guid>(this.playerAgents.Select(agent => agent.Id));

            var playerAgentsByName = this.playerAgents.ToDictionary(playerAgent => playerAgent.Name, playerAgent => playerAgent);
            this.instructions.ForEach(instruction => playerAgentsByName[instruction.PlayerName].AddInstruction(instruction));

            if (this.gameBoard == null)
                this.gameBoard = new GameBoard(BoardSizes.Standard);

            var gameServer = new LocalGameServer(
                this.numberGenerator,
                this.gameBoard,
                this.developmentCardHolder
            );

            if (!this.useServerTimer)
                gameServer.SetTurnTimer(new MockTurnTimer());

            gameServer.SetIdGenerator(() => { return playerIds.Dequeue(); });

            gameServer.LaunchGame();

            this.playerAgents.ForEach(playerAgent =>
            {
                playerAgent.JoinGame(gameServer);
                playerAgent.StartAsync();
            });

            foreach (var kv in this.startingResourcesByName)
                gameServer.AddResourcesToPlayer(kv.Key, kv.Value);

            gameServer.StartGameAsync();

            var playerAgentsFinished = false;
            var playerAgentFaulted = false;
            while (!playerAgentsFinished && !playerAgentFaulted)
            {
                Thread.Sleep(50);
                playerAgentsFinished = this.playerAgents.All(p => p.IsFinished);
                playerAgentFaulted = this.playerAgents.Any(p => p.GameException != null);
            }

            gameServer.Quit();

            if (playerAgentFaulted)
            {
                string message = null;
                foreach (var playerAgent in this.playerAgents.Where(p => p.GameException != null))
                {
                    var exception = playerAgent.GameException;
                    while (exception.InnerException != null)
                        exception = exception.InnerException;

                    message += $"{playerAgent.Name}: {exception.Message}\r\n";
                }

                throw new Exception(message);
            }

            gameServer.SaveLog(@"GameServer.log");
            this.playerAgents.ForEach(playerAgent => playerAgent.SaveLog($"{playerAgent.Name}.log"));
        }

        public ScenarioRunner WhenAnswerDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
        {
            var eventInstruction = new AnswerDirectTradeOfferEventInstruction(playerName, buyingPlayerName, wantedResources);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenDiceRollEvent(string playerName, uint dice1, uint dice2)
        {
            this.numberGenerator.AddTwoDiceRoll(dice1, dice2);
            var eventInstruction = new DiceRollEventInstruction(playerName, dice1, dice2);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenMakeDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
        {
            var eventInstruction = new MakeDirectTradeOfferEventInstruction(playerName, buyingPlayerName, wantedResources);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public PlayerStateInstruction State(string playerName)
        {
            var playerState = new PlayerStateInstruction(playerName, this);
            this.instructions.Add(playerState);
            return playerState;
        }

        public ScenarioRunner WhenPlaceInfrastructureSetupEvent(string playerName)
        {
            var eventInstruction = new ScenarioPlaceSetupInfrastructureEventInstruction(playerName);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WithNoResourceCollection()
        {
            this.gameBoard = new ScenarioGameBoardWithNoResourcesCollected();
            return this;
        }

        public ScenarioRunner WithPlayer(string playerName)
        {
            this.playerAgents.Add(new PlayerAgent(playerName));
            return this;
        }

        public ScenarioRunner WithStartingResourcesForPlayer(string playerName, ResourceClutch playerResources)
        {
            this.startingResourcesByName.Add(playerName, playerResources);
            return this;
        }

        public ScenarioRunner WithTurnOrder(string firstPlayerName, string secondPlayerName, string thirdPlayerName, string fourthPlayerName)
        {
            var rolls = new uint[4];
            for (var index = 0; index < this.playerAgents.Count; index++)
            {
                var playerName = this.playerAgents[index].Name;
                if (firstPlayerName == playerName)
                    rolls[index] = 12;
                else if (secondPlayerName == playerName)
                    rolls[index] = 10;
                else if (thirdPlayerName == playerName)
                    rolls[index] = 8;
                else
                    rolls[index] = 6;
            }

            foreach (var roll in rolls)
                this.numberGenerator.AddTwoDiceRoll(roll / 2, roll / 2);

            return this;
        }

        private void AddActionInstruction(ActionInstruction.OperationTypes operation, object[] arguments)
        {
            var actionInstruction = new ActionInstruction(
                this.LastInstructionPlayerName,
                operation,
                arguments);
            this.instructions.Add(actionInstruction);
        }
        #endregion
    }
}