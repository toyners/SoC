using System.Windows.Controls;

namespace SoC.Harness.Views
{
  /// <summary>
  /// Interaction logic for RoadControl.xaml
  /// </summary>
  public partial class RoadControl : UserControl
  {
    public string ImagePath { get; private set; }

    public RoadControl(string imagePath)
    {
      this.DataContext = this;
      this.ImagePath = imagePath;
      this.InitializeComponent();
    }
  }
}
