
namespace SoC.Harness
{
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class BuildingControl : UserControl
  {
    public string ImagePath { get; private set; }

    public BuildingControl(string imagePath)
    {
      this.DataContext = this;
      this.ImagePath = imagePath;
      this.InitializeComponent();
    }
  }
}
