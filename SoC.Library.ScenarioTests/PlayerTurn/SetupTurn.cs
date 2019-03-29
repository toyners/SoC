using System;
using System.Collections.Generic;
using SoC.Library.ScenarioTests.Instructions;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class SetupTurn : GameTurn
    {
        private SetupTurn(string playerName, LocalGameControllerScenarioRunner runner, string label)
            : base(playerName, runner)
        {
            this.Label = label;
        }

        public string Label { get; private set; }

        public SetupTurn StartingInfrastructure(uint settlementLocation, uint roadEnd)
        {
            return this;
        }

        public SetupTurn InitalBoardSetupEvent()
        {
            return this;
        }

        public static SetupTurn InitialBoardSetupEvent(string playerName, LocalGameControllerScenarioRunner runner)
        {
            var setupTurn = new SetupTurn(playerName, runner, "Initial Board Setup");
            setupTurn.InitialBoardSetupEvent();
            return setupTurn;
        }

        public static SetupTurn PlayerSetupEvent(string playerName, LocalGameControllerScenarioRunner runner, IDictionary<string, Guid> playerIdsByName)
        {
            var setupTurn = new SetupTurn(playerName, runner, "Player Setup");
            setupTurn.PlayerSetupEvent(playerName, playerIdsByName);
            return setupTurn;
        }

        public static SetupTurn PlayerInfrastructureSetupEvent(string playerName, LocalGameControllerScenarioRunner runner, uint settlementLocation, uint roadEndLocation, bool verifySetupInfrastructureEvent)
        {
            var setupTurn = new SetupTurn(playerName, runner, "Player Infrastructure Setup");
            if (verifySetupInfrastructureEvent)
                setupTurn.PlaceSetupInfrastructureEvent();
            setupTurn.PlaceStartingInfrastructure(settlementLocation, roadEndLocation);
            setupTurn.EndTurn();

            return setupTurn;
        }

        private void PlayerSetupEvent(string playerName, IDictionary<string, Guid> playerIdsByName)
        {
            this.instructions.Enqueue(new PlayerSetupEventInstruction(this.PlayerName, null));
        }

        private void InitialBoardSetupEvent()
        {
            // TODO: Put in the real expected GameBoardSetup data
            this.instructions.Enqueue(new InitialBoardSetupEventInstruction(this.PlayerName, null));
        }

        private void PlaceSetupInfrastructureEvent()
        {
            var instruction = new PlaceSetupInfrastructureEventInstruction(this.PlayerName);
            this.instructions.Enqueue(instruction);
        }

        private void PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            var instruction = new ActionInstruction(this.PlayerName,
                ActionInstruction.OperationTypes.PlaceStartingInfrastructure,
                new object[] { settlementLocation, roadEndLocation });
            this.instructions.Enqueue(instruction);
        }
    }
}
