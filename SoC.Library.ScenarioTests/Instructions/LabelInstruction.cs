
namespace SoC.Library.ScenarioTests.Instructions
{
    internal class LabelInstruction : Instruction
    {
        public LabelInstruction(string playerName, string label) : base(playerName)
        {
            this.Label = label;
        }

        public string Label { get; private set; }
    }
}
