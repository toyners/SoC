
namespace SoC.Harness.ViewModels
{
  using Jabberwocky.SoC.Library;

  public class PlayerViewModel
  {
    public PlayerViewModel(PlayerDataModel playerModel)
    {
      this.Name = playerModel.Name;
    }

    public string Name { get; private set; }

    public void Update(PlayerDataModel playerModel)
    {
      
    }
  }
}
