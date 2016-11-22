
namespace Client.TestHarness
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media.Imaging;
  using Jabberwocky.SoC.Client.ServiceReference;
  using Jabberwocky.SoC.Library;

  public class BoardDisplay
  {
    #region Fields
    private Board board;

    private Canvas backgroundCanvas;

    private Canvas foregroundCanvas;
    #endregion

    #region Construction
    public BoardDisplay(Board board, Canvas backgroundCanvas, Canvas foregroundCanvas)
    {
      //Todo null reference check
      this.board = board;
      this.backgroundCanvas = backgroundCanvas;
      this.foregroundCanvas = foregroundCanvas;
    }
    #endregion

    public event Action<Int32> ButtonClickEventHandler;

    #region Methods
    public void Clear()
    {
      this.backgroundCanvas.Children.Clear();
      this.ClearForeground();
    }

    public void ClearForeground()
    {
      this.foregroundCanvas.Children.Clear();
    }

    public void LayoutBoard(GameInitializationData gameData)
    {
      // 0 = desert, 1 = brick, 2 = grain, 3 = lumber, 4 = ore, 5 = wool
      // 20 = 2 on dice, 30 = 3 on dice, 40 = 4 on dice, .... 110 = 11 on dice, 120 = 12 on dice 
      var resourceBitmaps = new[]
      {
        new BitmapImage(new Uri(@"C:\projects\desert.png")),
        new BitmapImage(new Uri(@"C:\projects\brick.png")),
        new BitmapImage(new Uri(@"C:\projects\grain.png")),
        new BitmapImage(new Uri(@"C:\projects\lumber.png")),
        new BitmapImage(new Uri(@"C:\projects\ore.png")),
        new BitmapImage(new Uri(@"C:\projects\wool.png"))
      };

      var numberBitmaps = new Dictionary<Int32, BitmapImage>();
      numberBitmaps.Add(2, new BitmapImage(new Uri(@"C:\projects\2.png")));
      numberBitmaps.Add(3, new BitmapImage(new Uri(@"C:\projects\3.png")));
      numberBitmaps.Add(4, new BitmapImage(new Uri(@"C:\projects\4.png")));
      numberBitmaps.Add(5, new BitmapImage(new Uri(@"C:\projects\5.png")));
      numberBitmaps.Add(6, new BitmapImage(new Uri(@"C:\projects\6.png")));
      numberBitmaps.Add(8, new BitmapImage(new Uri(@"C:\projects\8.png")));
      numberBitmaps.Add(9, new BitmapImage(new Uri(@"C:\projects\9.png")));
      numberBitmaps.Add(10, new BitmapImage(new Uri(@"C:\projects\10.png")));
      numberBitmaps.Add(11, new BitmapImage(new Uri(@"C:\projects\11.png")));
      numberBitmaps.Add(12, new BitmapImage(new Uri(@"C:\projects\12.png")));

      var layoutColumnData = new[] 
      {
        new LayoutColumnData { X = 10, Y = 54, Count = 3 },
        new LayoutColumnData { X = 44, Y = 32, Count = 4 },
        new LayoutColumnData { X = 78, Y = 10, Count = 5 },
        new LayoutColumnData { X = 112, Y = 32, Count = 4 },
        new LayoutColumnData { X = 146, Y = 54, Count = 3 }
      };

      const Int32 cellHeight = 45;
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
          GetBitmap(gameData.BoardData[dataIndex++], resourceBitmaps, numberBitmaps, out resourceBitmap, out numberBitmap);
          this.PlaceHex(resourceBitmap, numberBitmap, x, y);
          y += cellHeight;
        }
      }
    }

    public void OverlayTownPlacement()
    {
      var button = new CircleButton(this.ButtonClickEventHandler, 0);
      this.foregroundCanvas.Children.Add(button);
      Canvas.SetLeft(button, 10);
      Canvas.SetTop(button, 10);
    }

    private void LocationClick(Object sender, RoutedEventArgs routedEventArgs)
    {
      throw new NotImplementedException();
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
      var resourceImage = this.CreateImage(resourceBitmap, String.Empty);
      this.backgroundCanvas.Children.Add(resourceImage);
      Canvas.SetLeft(resourceImage, x);
      Canvas.SetTop(resourceImage, y);

      if (numberBitmap == null)
      {
        return;
      }

      var numberImage = this.CreateImage(numberBitmap, String.Empty);
      this.backgroundCanvas.Children.Add(numberImage);
      Canvas.SetLeft(numberImage, x);
      Canvas.SetTop(numberImage, y);

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
    #endregion

    #region Structures
    private struct LayoutColumnData
    {
      public Int32 X;
      public Int32 Y;
      public UInt32 Count;
    }

    #endregion
  }
}
