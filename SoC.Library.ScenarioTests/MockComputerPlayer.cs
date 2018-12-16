using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    public class MockComputerPlayer : ComputerPlayer
    {
        private readonly List<Instruction> instructions = new List<Instruction>();

        public MockComputerPlayer(string name, INumberGenerator numberGenerator) : base(name, numberGenerator) { }

        public void AddInstructions(params Instruction[] instructions)
        {
            this.instructions.AddRange(instructions);
        }
    }

    public class Instruction
    {
        public readonly Guid Id;
        public Instruction(Guid playerId) => this.Id = playerId;
    }

    public class PlaceInfrastructureInstruction : Instruction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;
        public PlaceInfrastructureInstruction(Guid playerId, uint settlementLocation, uint roadEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }
    }
}
