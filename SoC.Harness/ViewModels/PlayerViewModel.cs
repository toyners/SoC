
namespace SoC.Harness.ViewModels
{
  using Jabberwocky.SoC.Library;

  public class PlayerViewModel
  {
    public PlayerViewModel(PlayerDataModel playerModel, string iconPath)
    {
      this.Name = playerModel.Name;
      this.IconPath = iconPath;
    }

    public string Name { get; private set; }
    public string IconPath { get; private set; }

    public void Update(PlayerDataModel playerModel)
    {
      
    }
  }
}
