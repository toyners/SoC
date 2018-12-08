
namespace SoC.Harness.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.PlayerData;
    using Jabberwocky.Toolkit.WPF;

    public class PlayerViewModel : NotifyPropertyChangedBase
    {
        private string resourceText;
        private readonly Queue<string> historyLines = new Queue<string>();
        private string historyText;

        public PlayerViewModel(PlayerFullDataModel playerModel, string iconPath)
        {
            this.Id = playerModel.Id;
            this.Name = playerModel.Name;
            this.IconPath = iconPath;
            this.UpdateHistory(this.Name + " initialised");
            this.Resources = ResourceClutch.Zero;
            this.ResourceText =
              $"Brick {this.Resources.BrickCount} " +
              $"Grain {this.Resources.GrainCount} " +
              $"Lumber {this.Resources.LumberCount} " +
              $"Ore {this.Resources.OreCount} " +
              $"Wool {this.Resources.WoolCount}";
        }

        public string HistoryText
        {
            get { return this.historyText; }
            private set { this.SetField(ref this.historyText, value); }
        }
        public string IconPath { get; private set; }
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public ResourceClutch Resources { get; private set; }
        public string ResourceText
        {
            get { return this.resourceText; }
            private set { this.SetField(ref this.resourceText, value); }
        }

        public void Update(PlayerDataModel playerModel)
        {
            throw new NotImplementedException();
        }

        public void Update(ResourceClutch resources, bool addResources)
        {
            if (addResources)
            {
                this.Resources += resources;
            }
            else
            {
                this.Resources -= resources;
            }

            this.ResourceText =
              $"Brick {this.Resources.BrickCount} " +
              $"Grain {this.Resources.GrainCount} " +
              $"Lumber {this.Resources.LumberCount} " +
              $"Ore {this.Resources.OreCount} " +
              $"Wool {this.Resources.WoolCount}";
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
