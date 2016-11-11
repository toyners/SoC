
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

    private Image testImage;

    public BoardDisplay(Board board, Canvas canvas)
    {
      //Todo null reference check
      this.board = board;
      this.canvas = canvas;
      BitmapImage bitmapImage = new BitmapImage(new Uri(@"C:\projects\redsquare.bmp"));
      this.testImage = new Image
      {
        Width = bitmapImage.Width,
        Height = bitmapImage.Height,
        Name = "Test",
        Source = bitmapImage
      };
    }

    public void Clear()
    {
      this.canvas.Children.Clear();
    }

    public void Draw()
    {
      this.Clear();
      this.canvas.Children.Add(this.testImage);
      Canvas.SetTop(this.testImage, 10);
      Canvas.SetLeft(this.testImage, 20);
    }
  }
}
