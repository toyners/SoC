
namespace SoC.Harness.Views
{
  using System;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class SettlementButtonControl : UserControl
  {
    private Action<SettlementButtonControl> clickEventHandler;
    public readonly uint Location;
    public readonly double X;
    public readonly double Y;

    public SettlementButtonControl(uint location, double x, double y, Action<SettlementButtonControl> clickEventHandler)
    {
      this.InitializeComponent();
      
      this.Location = location;
      this.X = x;
      this.Y = y;
      this.clickEventHandler = clickEventHandler;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.clickEventHandler?.Invoke(this);
    }
  }
}
