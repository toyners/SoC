
namespace SoC.Library.ScenarioTests.Instructions
{
    using System.Diagnostics;

    [DebuggerDisplay("Label: {Label}")]
    internal class LabelInstruction : Instruction
    {
        public LabelInstruction(string playerName, string label) : base(playerName)
        {
            this.Label = label;
        }

        public string Label { get; private set; }
    }
}
