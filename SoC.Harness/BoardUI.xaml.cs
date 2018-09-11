
namespace SoC.Harness
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Controls;
  using System.Windows.Media.Imaging;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;

  /// <summary>
  /// Interaction logic for BoardUI.xaml
  /// </summary>
  public partial class BoardUI : UserControl
  {
    public BoardUI()
    {
      this.InitializeComponent();
    }

    public void Initialise(IGameBoard board)
    {
      var resourceBitmaps = new Dictionary<ResourceTypes?, BitmapImage>
      {
        { null, new BitmapImage(new Uri(@"resources\desert.png", UriKind.Relative)) },
        { ResourceTypes.Brick, new BitmapImage(new Uri(@"resources\brick.png", UriKind.Relative)) },
        { ResourceTypes.Grain, new BitmapImage(new Uri(@"resources\grain.png", UriKind.Relative)) },
        { ResourceTypes.Lumber, new BitmapImage(new Uri(@"resources\lumber.png", UriKind.Relative)) },
        { ResourceTypes.Ore, new BitmapImage(new Uri(@"resources\ore.png", UriKind.Relative)) },
        { ResourceTypes.Wool, new BitmapImage(new Uri(@"resources\wool.png", UriKind.Relative)) }
      };

      var numberBitmaps = new Dictionary<uint, BitmapImage>();
      numberBitmaps.Add(2, new BitmapImage(new Uri(@"resources\2.png", UriKind.Relative)));
      numberBitmaps.Add(3, new BitmapImage(new Uri(@"resources\3.png", UriKind.Relative)));
      numberBitmaps.Add(4, new BitmapImage(new Uri(@"resources\4.png", UriKind.Relative)));
      numberBitmaps.Add(5, new BitmapImage(new Uri(@"resources\5.png", UriKind.Relative)));
      numberBitmaps.Add(6, new BitmapImage(new Uri(@"resources\6.png", UriKind.Relative)));
      numberBitmaps.Add(8, new BitmapImage(new Uri(@"resources\8.png", UriKind.Relative)));
      numberBitmaps.Add(9, new BitmapImage(new Uri(@"resources\9.png", UriKind.Relative)));
      numberBitmaps.Add(10, new BitmapImage(new Uri(@"resources\10.png", UriKind.Relative)));
      numberBitmaps.Add(11, new BitmapImage(new Uri(@"resources\11.png", UriKind.Relative)));
      numberBitmaps.Add(12, new BitmapImage(new Uri(@"resources\12.png", UriKind.Relative)));

      var layoutColumnData = new[]
      {
        new LayoutColumnData { X = 10, Y = 54, Count = 3 },
        new LayoutColumnData { X = 44, Y = 32, Count = 4 },
        new LayoutColumnData { X = 78, Y = 10, Count = 5 },
        new LayoutColumnData { X = 112, Y = 32, Count = 4 },
        new LayoutColumnData { X = 146, Y = 54, Count = 3 }
      };

      const int cellHeight = 45;
      BitmapImage resourceBitmap = null;
      BitmapImage numberBitmap = null;
      var hexData = board.GetHexInformation();
      var hexDataIndex = 0;

      foreach (var columnData in layoutColumnData)
      {
        var count = columnData.Count;
        var x = columnData.X;
        var y = columnData.Y;

        while (count-- > 0)
        {
          GetBitmaps(hexData[hexDataIndex], resourceBitmaps, numberBitmaps, out resourceBitmap, out numberBitmap);
          this.PlaceHex(resourceBitmap, numberBitmap, x, y);
          y += cellHeight;
        }
      }
    }

    private Image CreateImage(BitmapImage bitmapImage, String name)
    {
      return new Image
      {
        Width = bitmapImage.Width,
        Height = bitmapImage.Height,
        Name = name,
        Source = bitmapImage
      };
    }

    private void GetBitmaps(Tuple<ResourceTypes?, uint> hexData, Dictionary<ResourceTypes?, BitmapImage> resourceBitmaps, Dictionary<uint, BitmapImage> numberBitmaps, out BitmapImage resourceBitmap, out BitmapImage numberBitmap)
    {
      resourceBitmap = resourceBitmaps[hexData.Item1];
      numberBitmap = numberBitmaps[hexData.Item2];
    }

    private void PlaceHex(BitmapImage resourceBitmap, BitmapImage numberBitmap, Int32 x, Int32 y)
    {
      var resourceImage = this.CreateImage(resourceBitmap, string.Empty);
      this.Background.Children.Add(resourceImage);
      Canvas.SetLeft(resourceImage, x);
      Canvas.SetTop(resourceImage, y);

      if (numberBitmap == null)
      {
        return;
      }

      var numberImage = this.CreateImage(numberBitmap, string.Empty);
      this.Background.Children.Add(numberImage);
      Canvas.SetLeft(numberImage, x);
      Canvas.SetTop(numberImage, y);
    }

    #region Structures
    private struct LayoutColumnData
    {
      public int X, Y;
      public uint Count;
    }
    #endregion
  }
}
