
namespace SoC.Harness.Views
{
  using System;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for RoadButtonControl.xaml
  /// </summary>
  public partial class RoadButtonControl : UserControl
  {
    public enum RoadImageTypes
    {
      Horizontal = 0,
      Left,
      Right
    }

    private Action<RoadButtonControl> clickEventHandler;
    public readonly uint Start, End;
    public readonly double X;
    public readonly double Y;
    public RoadImageTypes RoadImageType;
    public string ImagePath { get; private set; }

    public RoadButtonControl(uint start, uint end, double x, double y, string imagePath, RoadImageTypes roadImageType, Action<RoadButtonControl> clickEventHandler)
    {
      this.Start = start;
      this.End = end;
      this.X = x;
      this.Y = y;
      this.DataContext = this;
      this.ImagePath = imagePath;
      this.InitializeComponent();
      this.Visibility = Visibility.Hidden;
      this.RoadImageType = roadImageType;

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
