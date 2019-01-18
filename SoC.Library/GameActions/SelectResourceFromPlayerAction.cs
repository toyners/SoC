
namespace Jabberwocky.SoC.Library.GameActions
{
    public class SelectResourceFromPlayerAction : ComputerPlayerAction
    {
        public readonly System.Guid PlayerId;
        public SelectResourceFromPlayerAction(System.Guid playerId) : base(0)
        {
            this.PlayerId = playerId;
        }
    }
}
