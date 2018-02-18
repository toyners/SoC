
namespace Jabberwocky.SoC.Library.GameActions
{
  using Interfaces;

  public class PlayerAction
  {
    public readonly PlayerActionTypes Action;

    public PlayerAction(PlayerActionTypes action)
    {
      this.Action = action;
    }
  }
}
