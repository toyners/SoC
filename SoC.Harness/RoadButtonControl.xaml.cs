using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoC.Harness
{
  /// <summary>
  /// Interaction logic for RoadButtonControl.xaml
  /// </summary>
  public partial class RoadButtonControl : UserControl
  {
    private Action<RoadButtonControl> clickEventHandler;
    public readonly string Id;
    public readonly double X;
    public readonly double Y;
    public readonly string ImagePath;
    public string IndicatorImagePath { get; private set; }

    public RoadButtonControl(string id, double x, double y, string indicatorImagePath, string imagePath, Action<RoadButtonControl> clickEventHandler)
    {
      this.Id = id;
      this.X = x;
      this.Y = y;
      this.DataContext = this;
      this.IndicatorImagePath = indicatorImagePath;
      this.ImagePath = imagePath;
      this.InitializeComponent();

      this.clickEventHandler = clickEventHandler;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.clickEventHandler?.Invoke(this);
    }
  }
}
