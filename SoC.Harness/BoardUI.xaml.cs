
namespace SoC.Harness
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Controls;
  using System.Windows.Media.Imaging;

  /// <summary>
  /// Interaction logic for BoardUI.xaml
  /// </summary>
  public partial class BoardUI : UserControl
  {
    public BoardUI()
    {
      this.InitializeComponent();

      //var bs = BitmapSource.Create()
      
      var resourceBitmaps = new[]
      {
        new BitmapImage(new Uri(@"resources\desert.png")),
        new BitmapImage(new Uri(@"resources\brick.png")),
        new BitmapImage(new Uri(@"resources\grain.png")),
        new BitmapImage(new Uri(@"resources\lumber.png")),
        new BitmapImage(new Uri(@"resources\ore.png")),
        new BitmapImage(new Uri(@"resources\wool.png"))
      };

      var numberBitmaps = new Dictionary<int, BitmapImage>();
      numberBitmaps.Add(2, new BitmapImage(new Uri(@"resources\2.png")));
      numberBitmaps.Add(3, new BitmapImage(new Uri(@"resources\3.png")));
      numberBitmaps.Add(4, new BitmapImage(new Uri(@"resources\4.png")));
      numberBitmaps.Add(5, new BitmapImage(new Uri(@"resources\5.png")));
      numberBitmaps.Add(6, new BitmapImage(new Uri(@"resources\6.png")));
      numberBitmaps.Add(8, new BitmapImage(new Uri(@"resources\8.png")));
      numberBitmaps.Add(9, new BitmapImage(new Uri(@"resources\9.png")));
      numberBitmaps.Add(10, new BitmapImage(new Uri(@"resources\10.png")));
      numberBitmaps.Add(11, new BitmapImage(new Uri(@"resources\11.png")));
      numberBitmaps.Add(12, new BitmapImage(new Uri(@"resources\12.png")));

      var layoutColumnData = new[]
      {
        new LayoutColumnData { X = 10, Y = 54, Count = 3 },
        new LayoutColumnData { X = 44, Y = 32, Count = 4 },
        new LayoutColumnData { X = 78, Y = 10, Count = 5 },
        new LayoutColumnData { X = 112, Y = 32, Count = 4 },
        new LayoutColumnData { X = 146, Y = 54, Count = 3 }
      };

      const int cellHeight = 45;
      var dataIndex = 0;
      BitmapImage resourceBitmap = null;
      BitmapImage numberBitmap = null;

      foreach (var columnData in layoutColumnData)
      {
        var count = columnData.Count;
        var x = columnData.X;
        var y = columnData.Y;

        while (count-- > 0)
        {
          //GetBitmap(gameData.BoardData[dataIndex++], resourceBitmaps, numberBitmaps, out resourceBitmap, out numberBitmap);
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

    private void GetBitmap(Byte hexData, BitmapImage[] resourceBitmaps, Dictionary<Int32, BitmapImage> numberBitmaps, out BitmapImage resourceBitmap, out BitmapImage numberBitmap)
    {
      var index = hexData % 10;
      resourceBitmap = resourceBitmaps[index];

      if (hexData == 0)
      {
        numberBitmap = null;
      }
      else
      {
        numberBitmap = numberBitmaps[hexData / 10];
      }
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
