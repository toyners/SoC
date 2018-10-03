using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SoC.Harness
{
  /// <summary>
  /// Interaction logic for RoadButtonControl.xaml
  /// </summary>
  public partial class RoadButtonControl : UserControl
  {
    private Action<RoadButtonControl> clickEventHandler;
    public readonly uint Start, End;
    public readonly double X;
    public readonly double Y;
    public readonly string ImagePath;
    public string IndicatorImagePath { get; private set; }

    public RoadButtonControl(uint start, uint end, double x, double y, string indicatorImagePath, string imagePath, Action<RoadButtonControl> clickEventHandler)
    {
      this.Start = start;
      this.End = end;
      this.X = x;
      this.Y = y;
      this.DataContext = this;
      this.IndicatorImagePath = indicatorImagePath;
      this.ImagePath = imagePath;
      this.InitializeComponent();

      this.clickEventHandler = clickEventHandler;
    }

    public string Id { get { return this.Start + "-" + this.End; } }

    public string AlternativeId { get { return this.End + "-" + this.Start; } }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.clickEventHandler?.Invoke(this);
    }
  }
}
