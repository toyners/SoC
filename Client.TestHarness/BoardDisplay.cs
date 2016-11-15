
namespace Client.TestHarness
{
  using System;
  using System.Windows.Controls;
  using System.Windows.Media.Imaging;
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
    public void LayoutBoard()
    {
      this.Clear();

      var desertBitmap = new BitmapImage(new Uri(@"C:\projects\desert.png"));
      var brickBitmap = new BitmapImage(new Uri(@"C:\projects\brick.png"));
      var lumberBitmap = new BitmapImage(new Uri(@"C:\projects\lumber.png"));
      var oreBitmap = new BitmapImage(new Uri(@"C:\projects\ore.png"));
      var wheatBitmap = new BitmapImage(new Uri(@"C:\projects\wheat.png"));
      var woolBitmap = new BitmapImage(new Uri(@"C:\projects\wool.png"));

      // Column 1
      Image image;
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
      Canvas.SetTop(image, 77);
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
