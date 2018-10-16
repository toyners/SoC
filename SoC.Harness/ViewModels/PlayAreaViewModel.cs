
namespace SoC.Harness.ViewModels
{
  using Jabberwocky.SoC.Library;

  public class PlayAreaViewModel
  {
    private LocalGameController localGameController;

    public PlayAreaViewModel(LocalGameController localGameController)
    {
      this.localGameController = localGameController;
    }
  }
}
