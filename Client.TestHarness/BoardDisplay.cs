
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

    private Canvas canvas;
    #endregion

    #region Construction
    public BoardDisplay(Board board, Canvas canvas)
    {
      //Todo null reference check
      this.board = board;
      this.canvas = canvas;
    }
    #endregion

    #region Methods
    public void LayoutBoard(GameInitializationData gameData)
    {
      this.Clear();

      // 0 = desert, 1 = brick, 2 = grain, 3 = lumber, 4 = ore, 5 = wool
      // 20 = 2 on dice, 30 = 3 on dice, 40 = 4 on dice, .... 110 = 11 on dice, 120 = 12 on dice 
      var bitmaps = new[]
      {
        new BitmapImage(new Uri(@"C:\projects\desert.png")),
        new BitmapImage(new Uri(@"C:\projects\brick.png")),
        new BitmapImage(new Uri(@"C:\projects\grain.png")),
        new BitmapImage(new Uri(@"C:\projects\lumber.png")),
        new BitmapImage(new Uri(@"C:\projects\ore.png")),
        new BitmapImage(new Uri(@"C:\projects\wool.png"))
      };

      var dataIndex = 0;
      var count = 3;
      var x = 10;
      var y = 54;
      BitmapImage bitmap = null;

      // Column 1
      while (count-- > 0)
      {
        bitmap = GetBitmap(gameData.BoardData[dataIndex++], bitmaps);
        this.PlaceHex(bitmap, x, y);
        y += 45;
      }

      // Column 2
      count = 4;
      x = 44;
      y = 32;
      while (count-- > 0)
      {
        bitmap = GetBitmap(gameData.BoardData[dataIndex++], bitmaps);
        this.PlaceHex(bitmap, x, y);
        y += 45;
      }

      // Column 3
      count = 5;
      x = 78;
      y = 10;
      while (count-- > 0)
      {
        bitmap = GetBitmap(gameData.BoardData[dataIndex++], bitmaps);
        this.PlaceHex(bitmap, x, y);
        y += 45;
      }

      // Column 4
      count = 4;
      x = 112;
      y = 32;
      while (count-- > 0)
      {
        bitmap = GetBitmap(gameData.BoardData[dataIndex++], bitmaps);
        this.PlaceHex(bitmap, x, y);
        y += 45;
      }

      // Column5
      count = 3;
      x = 146;
      y = 54;
      while (count-- > 0)
      {
        bitmap = GetBitmap(gameData.BoardData[dataIndex++], bitmaps);
        this.PlaceHex(bitmap, x, y);
        y += 45;
      }

      // Column 1
      /*Image image;
      image = this.CreateImage(woolBitmap, "Wool1");
      this.canvas.Children.Add(image);
      Canvas.SetLeft(image, 10);
      Canvas.SetTop(image, 10);

      image = this.CreateImage(brickBitmap, "Brick1");
      this.canvas.Children.Add(image);
      Canvas.SetLeft(image, 10);
      Canvas.SetTop(image, 55);

      image = this.CreateImage(wheatBitmap, "Wheat1");
      this.canvas.Children.Add(image);
      Canvas.SetLeft(image, 10);
      Canvas.SetTop(image, 100);

      // Column 2
      image = this.CreateImage(oreBitmap, "Ore1");
      this.canvas.Children.Add(image);
      Canvas.SetLeft(image, 44);
      Canvas.SetTop(image, 32);

      image = this.CreateImage(lumberBitmap, "Lumber1");
      this.canvas.Children.Add(image);
      Canvas.SetLeft(image, 44);
      Canvas.SetTop(image, 77);*/
    }

    private BitmapImage GetBitmap(Byte hexData, BitmapImage[] bitmaps)
    {
      var index = hexData % 10;
      return bitmaps[index];
    }

    private void PlaceHex(BitmapImage bitmap, Int32 x, Int32 y)
    {
      var image = this.CreateImage(bitmap, String.Empty);
      this.canvas.Children.Add(image);
      Canvas.SetLeft(image, x);
      Canvas.SetTop(image, y);
    }

    public void Clear()
    {
      this.canvas.Children.Clear();
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
  }
}
