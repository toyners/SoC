
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

    public BoardDisplay(Board board, Canvas canvas)
    {
      //Todo null reference check
      this.board = board;
      this.canvas = canvas;
      BitmapImage bitmapImage = new BitmapImage(new Uri(@"C:\projects\redsquare.bmp"));
      Image image = new Image
      {
        Width = bitmapImage.Width,
        Height = bitmapImage.Height,
        Name = "Test",
        Source = bitmapImage
      };

      this.canvas.Children.Add(image);
      Canvas.SetTop(image, 10);
      Canvas.SetLeft(image, 20);
    }

    public void Clear()
    {
      this.canvas.Children.Clear();
    }

    public void Draw()
    {

    }
  }
}
