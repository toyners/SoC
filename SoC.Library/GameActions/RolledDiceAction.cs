
namespace Jabberwocky.SoC.Library.GameActions
{
    public class RolledDiceAction : ComputerPlayerAction
    {
        public readonly uint Dice1;
        public readonly uint Dice2;

        public RolledDiceAction(uint dice1, uint dice2) : base(Enums.ComputerPlayerActionTypes.RolledDice)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
        }
    }
}
