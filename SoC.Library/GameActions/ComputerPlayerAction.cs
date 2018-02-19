
namespace Jabberwocky.SoC.Library.GameActions
{
  using Enums;

  public class ComputerPlayerAction
  {
    public readonly ComputerPlayerActionTypes Action;

    public ComputerPlayerAction(ComputerPlayerActionTypes action)
    {
      this.Action = action;
    }
  }
}
