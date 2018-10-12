
namespace SoC.Harness.ViewModels
{
  using System;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.Toolkit.WPF;

  public class PlayerViewModel : NotifyPropertyChangedBase
  {
    private ResourceClutch resources;
    private string resourceSummary;

    public PlayerViewModel(PlayerDataModel playerModel, string iconPath)
    {
      this.Name = playerModel.Name;
      this.IconPath = iconPath;
    }

    public string Name { get; private set; }
    public string IconPath { get; private set; }
    public string ResourceSummary
    {
      get { return this.resourceSummary; }
      private set { this.SetField(ref this.resourceSummary, value); }
    }

    public void Update(PlayerDataModel playerModel)
    {
      throw new NotImplementedException();
    }

    public void Update(ResourceClutch resources)
    {
      this.resources += resources;
      this.ResourceSummary = 
        $"B{this.resources.BrickCount} " +
        $"G{this.resources.GrainCount} " +
        $"L{this.resources.LumberCount} " +
        $"O{this.resources.OreCount} " +
        $"W{this.resources.WoolCount}";
    }
  }
}
