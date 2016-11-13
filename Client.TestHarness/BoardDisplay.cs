
namespace Client.TestHarness
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media.Imaging;
  using Jabberwocky.SoC.Library;

  public class BoardDisplay : IBoardDisplay
  {
    private Board board;

    private Canvas canvas;

    private Image pastureImage;

    private Image brickImage;

    private Image wheatImage;

    private Image lumberImage;

    private Image oreImage;

    public BoardDisplay(Board board, Canvas canvas)
    {
      //Todo null reference check
      this.board = board;
      this.canvas = canvas;
      var woolBitmap = new BitmapImage(new Uri(@"C:\projects\wool.png"));
      var brickBitmap = new BitmapImage(new Uri(@"C:\projects\brick.png"));
      var wheatBitmap = new BitmapImage(new Uri(@"C:\projects\wheat.png"));
      var lumberBitmap = new BitmapImage(new Uri(@"C:\projects\lumber.png"));
      var oreBitmap = new BitmapImage(new Uri(@"C:\projects\ore.png"));

      this.pastureImage = this.CreateImage(woolBitmap, "Wool1");
      this.brickImage = this.CreateImage(brickBitmap, "Brick1");
      this.wheatImage = this.CreateImage(wheatBitmap, "Wheat1");
      this.lumberImage = this.CreateImage(lumberBitmap, "Lumber1");
      this.oreImage = this.CreateImage(oreBitmap, "Ore1");
    }

    public void Clear()
    {
      this.canvas.Children.Clear();
    }

    public void Draw()
    {
      this.Clear();

      // Row 1
      this.canvas.Children.Add(this.pastureImage);
      Canvas.SetLeft(this.pastureImage, 10);
      Canvas.SetTop(this.pastureImage, 10);

      this.canvas.Children.Add(this.brickImage);
      Canvas.SetLeft(this.brickImage, 10);
      Canvas.SetTop(this.brickImage, 55);

      this.canvas.Children.Add(this.wheatImage);
      Canvas.SetLeft(this.wheatImage, 10);
      Canvas.SetTop(this.wheatImage, 100);

      // Row 2
      this.canvas.Children.Add(this.oreImage);
      Canvas.SetLeft(this.oreImage, 44);
      Canvas.SetTop(this.oreImage, 32);

      this.canvas.Children.Add(this.lumberImage);
      Canvas.SetLeft(this.lumberImage, 44);
      Canvas.SetTop(this.lumberImage, 77);
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
  }
}
