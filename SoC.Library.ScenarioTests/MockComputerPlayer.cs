using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    public class MockComputerPlayer : ComputerPlayer
    {
        private PlaceInfrastructureInstruction firstInstruction;
        private PlaceInfrastructureInstruction secondInstruction;
        private readonly List<Instruction> instructions = new List<Instruction>();

        public MockComputerPlayer(string name, INumberGenerator numberGenerator) : base(name, numberGenerator) { }

        public void AddSetupInstructions(PlaceInfrastructureInstruction firstInstruction, PlaceInfrastructureInstruction secondInstruction)
        {
            this.firstInstruction = firstInstruction;
            this.secondInstruction = secondInstruction;
        }

        public void AddInstructions(params Instruction[] instructions)
        {
            this.instructions.AddRange(instructions);
        }

        public override void ChooseInitialInfrastructure(out uint settlementLocation, out uint roadEndLocation)
        {
            if (this.firstInstruction != null)
            {
                settlementLocation = this.firstInstruction.SettlementLocation;
                roadEndLocation = this.firstInstruction.RoadEndLocation;
                this.firstInstruction = null;
            }
            else
            {
                settlementLocation = this.secondInstruction.SettlementLocation;
                roadEndLocation = this.secondInstruction.RoadEndLocation;
            }
        }
    }
}
