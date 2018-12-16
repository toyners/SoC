using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class LocalGameControllerScenarioRunner
    {
        internal enum EventTypes
        {
            DiceRollEvent
        }

        private readonly MockPlayerPool mockPlayerPool = new MockPlayerPool();
        private readonly Queue<Instruction> playerInstructions = new Queue<Instruction>();
        private readonly List<PlayerTurnSetupAction> FirstRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly List<PlayerTurnSetupAction> SecondRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly Dictionary<EventTypes, Delegate> eventHandlers;
        private readonly List<PlayerTurn> playerTurns = new List<PlayerTurn>();
        private readonly MockNumberGenerator mockNumberGenerator = new MockNumberGenerator();
        private readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Dictionary<string, MockComputerPlayer> computerPlayersByName = new Dictionary<string, MockComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);

        private static LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;

        public static LocalGameControllerScenarioRunner LocalGameController(Dictionary<EventTypes, Delegate> eventHandlers = null)
        {
            return localGameControllerScenarioBuilder = new LocalGameControllerScenarioRunner(eventHandlers);
        }

        private LocalGameControllerScenarioRunner(Dictionary<EventTypes, Delegate> eventHandlers)
        {
            this.eventHandlers = eventHandlers;
        }

        public LocalGameController BuildAndRun()
        {
            var localGameController = new LocalGameController(null, this.mockPlayerPool);

            if (this.eventHandlers != null)
            {
                if (this.eventHandlers.TryGetValue(EventTypes.DiceRollEvent, out var eventHandler))
                    localGameController.DiceRollEvent = (Action<uint, uint>)eventHandler;
            }

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();

            var placeInfrastructureInstruction = (PlaceInfrastructureInstruction)this.playerInstructions.Dequeue();
            localGameController.ContinueGameSetup(placeInfrastructureInstruction.SettlementLocation, placeInfrastructureInstruction.RoadEndLocation);

            placeInfrastructureInstruction = (PlaceInfrastructureInstruction)this.playerInstructions.Dequeue();
            localGameController.CompleteGameSetup(placeInfrastructureInstruction.SettlementLocation, placeInfrastructureInstruction.RoadEndLocation);

            localGameController.FinalisePlayerTurnOrder();
            localGameController.StartGamePlay();

            return localGameController;
        }

        public LocalGameControllerScenarioRunner WithMainPlayer(string name)
        {
            this.CreatePlayer(name, false);
            return this;
        }

        public LocalGameControllerScenarioRunner WithComputerPlayer(string name)
        {
            this.CreatePlayer(name, true);
            return this;
        }

        private IPlayer CreatePlayer(string name, bool isComputerPlayer)
        {
            IPlayer player = isComputerPlayer 
                ? new MockComputerPlayer(name, this.mockNumberGenerator) as IPlayer
                : new MockPlayer(name) as IPlayer;

            this.players.Add(player);
            this.playersByName.Add(name, player);
            this.mockPlayerPool.AddPlayer(player);
            if (isComputerPlayer)
                this.computerPlayersByName.Add(name, (MockComputerPlayer)player);

            return player;
        }

        public LocalGameControllerScenarioRunner WithPlayerSetup(string playerName, uint firstSettlementLocation, uint firstRoadEndLocation, uint secondSettlementLocation, uint secondRoadEndLocation)
        {
            if (playerName == this.players[0].Name)
            {
                this.playerInstructions.Enqueue(new PlaceInfrastructureInstruction(this.players[0].Id, firstSettlementLocation, firstRoadEndLocation));
                this.playerInstructions.Enqueue(new PlaceInfrastructureInstruction(this.players[0].Id, secondSettlementLocation, secondRoadEndLocation));
            }
            else
            {
                var computerPlayer = this.computerPlayersByName[playerName];
                computerPlayer.AddInstructions(
                    new PlaceInfrastructureInstruction(computerPlayer.Id, firstSettlementLocation, firstRoadEndLocation),
                    new PlaceInfrastructureInstruction(computerPlayer.Id, secondSettlementLocation, secondRoadEndLocation));
            }

            return this;
        }

        public LocalGameControllerScenarioRunner WithTurnOrder(string firstPlayerName, string secondPlayerName, string thirdPlayerName, string fourthPlayerName)
        {
            var rolls = new uint[4];
            for (var index = 0; index < this.players.Count; index++)
            {
                var player = this.players[index];
                if (firstPlayerName == player.Name)
                    rolls[index] = 12;
                else if (secondPlayerName == player.Name)
                    rolls[index] = 10;
                else if (thirdPlayerName == player.Name)
                    rolls[index] = 8;
                else
                    rolls[index] = 6;
            }

            foreach (var roll in rolls)
                this.mockNumberGenerator.AddTwoDiceRoll(roll / 2, roll / 2);

            return this;
        }

        public PlayerTurn DuringPlayerTurn(string playerName, uint dice1, uint dice2)
        {
            PlayerTurn playerTurn = null;

            if (playerName == this.players[0].Name)
            {
                var player = this.players[0];
                playerTurn = new PlayerTurn(this, player);
            }
            else
            {
                var computerPlayer = this.computerPlayersByName[playerName];
                playerTurn = new PlayerTurn(this, computerPlayer);
            }

            this.playerTurns.Add(playerTurn);

            return playerTurn;
        }
    }
}
