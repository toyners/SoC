
namespace Jabberwocky.SoC.Library.GameActions
{
    public class PlayKnightCardAction : ComputerPlayerAction
    {
        public readonly uint NewRobberHex;
        public PlayKnightCardAction(uint newRobberHex) : base(Enums.ComputerPlayerActionTypes.PlayKnightCard)
        {
            this.NewRobberHex = newRobberHex;
        }
    }
}
