
namespace SoC.Harness
{
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class SettlementControl : UserControl
  {
    public string ImagePath { get; private set; }

    public SettlementControl(string imagePath)
    {
      this.DataContext = this;
      this.ImagePath = imagePath;
      this.InitializeComponent();
    }
  }
}
