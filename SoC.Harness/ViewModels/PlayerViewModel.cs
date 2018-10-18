
namespace SoC.Harness.ViewModels
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.Toolkit.WPF;

  public class PlayerViewModel : NotifyPropertyChangedBase
  {
    private ResourceClutch resources;
    private string resourceText;
    private readonly Queue<string> historyLines = new Queue<string>();
    private string historyText;

    public PlayerViewModel(PlayerDataModel playerModel, string iconPath)
    {
      this.Id = playerModel.Id;
      this.Name = playerModel.Name;
      this.IconPath = iconPath;
      this.UpdateHistory(this.Name + " initialised");
    }

    public string HistoryText
    {
      get { return this.historyText; }
      private set { this.SetField(ref this.historyText, value); }
    }
    public string IconPath { get; private set; }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string ResourceText
    {
      get { return this.resourceText; }
      private set { this.SetField(ref this.resourceText, value); }
    }

    public void Update(PlayerDataModel playerModel)
    {
      throw new NotImplementedException();
    }

    public void Update(ResourceClutch resources)
    {
      this.resources += resources;
      this.ResourceText = 
        $"B{this.resources.BrickCount} " +
        $"G{this.resources.GrainCount} " +
        $"L{this.resources.LumberCount} " +
        $"O{this.resources.OreCount} " +
        $"W{this.resources.WoolCount}";

      var line = "Received ";
      if (resources.BrickCount > 0)
      {
        line += "B" + resources.BrickCount + " ";
      }

      if (resources.GrainCount > 0)
      {
        line += "G" + resources.GrainCount + " ";
      }

      if (resources.LumberCount > 0)
      {
        line += "L" + resources.LumberCount + " ";
      }

      if (resources.OreCount > 0)
      {
        line += "O" + resources.OreCount + " ";
      }

      if (resources.WoolCount > 0)
      {
        line += "W" + resources.WoolCount + " ";
      }

      this.UpdateHistory(line);
    }

    public void UpdateHistory(string line)
    {
      if (this.historyLines.Count >= 150)
      {
        this.historyLines.Dequeue();
      }

      this.historyLines.Enqueue(line);

      this.HistoryText = string.Join("\n", this.historyLines);
    }
  }
}
