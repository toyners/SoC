
namespace SoC.Harness
{
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class SettlementControl : UserControl
  {
    public string ImageName { get; private set; }

    public SettlementControl()
    {
      this.ImageName = @"resources\settlements\blue_settlement.png";
      this.InitializeComponent();
    }
  }
}
