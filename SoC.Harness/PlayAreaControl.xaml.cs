
namespace SoC.Harness
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media.Imaging;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;

  /// <summary>
  /// Interaction logic for BoardUI.xaml
  /// </summary>
  public partial class PlayAreaControl : UserControl
  {
    private IGameBoard board;

    #region Construction
    public PlayAreaControl()
    {
      this.InitializeComponent();
    }
    #endregion

    public Action<uint, uint> settlementSelection;

    #region Methods
    public void Initialise(IGameBoard board)
    {
      this.board = board;

      var resourceBitmaps = this.CreateResourceBitmaps();
      var numberBitmaps = this.CreateNumberBitmaps();

      var middleX = (int)this.BoardLayer.Width / 2;
      var middleY = (int)this.BoardLayer.Height / 2;
      const int cellHeight = 90;
      const int cellWidth = 90;

      var hexLayoutData = new[]
      {
        new HexLayoutData { X = middleX - (cellWidth * 2) + 4, Y = middleY - (cellHeight / 2) - cellHeight, Count = 3 },
        new HexLayoutData { X = middleX - (cellWidth / 4) * 5, Y = middleY - (cellHeight * 2), Count = 4 },
        new HexLayoutData { X = middleX - (cellWidth / 2), Y = middleY - (cellHeight / 2) - (cellHeight * 2), Count = 5 },
        new HexLayoutData { X = middleX + (cellWidth / 4) - 2, Y = middleY - (cellHeight * 2), Count = 4 },
        new HexLayoutData { X = middleX + cellWidth - 4, Y = middleY - (cellHeight / 2) - cellHeight, Count = 3 }
      };

      BitmapImage resourceBitmap = null;
      BitmapImage numberBitmap = null;
      var hexData = this.board.GetHexInformation();
      int hexDataIndex = 0;

      foreach (var hexLayout in hexLayoutData)
      {
        var count = hexLayout.Count;
        var x = hexLayout.X;
        var y = hexLayout.Y;

        while (count-- > 0)
        {
          this.GetBitmaps(hexData[hexDataIndex++], resourceBitmaps, numberBitmaps, out resourceBitmap, out numberBitmap);
          this.PlaceHex(resourceBitmap, numberBitmap, x, y);
          y += cellHeight;
        }
      }

      this.InitialiseSettlementLayer();
    }

    public void InitialiseSettlementLayer()
    {
      //var dx = 21;
      //var dy = 44;
      uint location = 0;
      var settlementLayoutData = new[]
      {
        new SettlementLayoutData { X = 240, Y = 82, Dx = 21, Dy = 44, DirectionX = -1, Count = 7 },
        new SettlementLayoutData { X = 304, Y = 38, Dx = 21, Dy = 44, DirectionX = -1, Count = 9 },
        new SettlementLayoutData { X = 368, Y = -6, Dx = 21, Dy = 44, DirectionX = -1, Count = 11 },
        new SettlementLayoutData { X = 412, Y = -6, Dx = 21, Dy = 44, DirectionX = 1, Count = 11 },
        new SettlementLayoutData { X = 476, Y = 38, Dx = 21, Dy = 44, DirectionX = 1, Count = 9 },
        new SettlementLayoutData { X = 540, Y = 82, Dx = 21, Dy = 44, DirectionX = 1, Count = 7 },
      };

      foreach (var settlementData in settlementLayoutData)
      {
        var count = settlementData.Count;
        var x = settlementData.X;
        var y = settlementData.Y;
        var direction = settlementData.DirectionX;
        var dx = settlementData.Dx;
        var dy = settlementData.Dy;

        while (count-- > 0)
        {
          this.PlaceSettlementButton(x, y, location++, "Test");
          y += dy;

          x += direction * dx;
          direction = direction == -1 ? 1 : -1;
        }
      }
    }

    private Image CreateImage(BitmapImage bitmapImage, String name)
    {
      return new Image
      {
        Width = bitmapImage.Width * 2,
        Height = bitmapImage.Height * 2,
        Name = name,
        Source = bitmapImage,
        StretchDirection = StretchDirection.Both
      };
    }

    private Dictionary<uint, BitmapImage> CreateNumberBitmaps()
    {
#pragma warning disable IDE0009 // Member access should be qualified.
      return new Dictionary<uint, BitmapImage>
      {
        { 2, new BitmapImage(new Uri(@"resources\productionfactors\2.png", UriKind.Relative)) },
        { 3, new BitmapImage(new Uri(@"resources\productionfactors\3.png", UriKind.Relative)) },
        { 4, new BitmapImage(new Uri(@"resources\productionfactors\4.png", UriKind.Relative)) },
        { 5, new BitmapImage(new Uri(@"resources\productionfactors\5.png", UriKind.Relative)) },
        { 6, new BitmapImage(new Uri(@"resources\productionfactors\6.png", UriKind.Relative)) },
        { 8, new BitmapImage(new Uri(@"resources\productionfactors\8.png", UriKind.Relative)) },
        { 9, new BitmapImage(new Uri(@"resources\productionfactors\9.png", UriKind.Relative)) },
        { 10, new BitmapImage(new Uri(@"resources\productionfactors\10.png", UriKind.Relative)) },
        { 11, new BitmapImage(new Uri(@"resources\productionfactors\11.png", UriKind.Relative)) },
        { 12, new BitmapImage(new Uri(@"resources\productionfactors\12.png", UriKind.Relative)) }
      };
#pragma warning restore IDE0009 // Member access should be qualified.
    }

    private Dictionary<ResourceTypes, BitmapImage> CreateResourceBitmaps()
    {
#pragma warning disable IDE0009 // Member access should be qualified.
      return new Dictionary<ResourceTypes, BitmapImage>
      {
        { ResourceTypes.Brick, new BitmapImage(new Uri(@"resources\hextypes\brick.png", UriKind.Relative)) },
        { ResourceTypes.Grain, new BitmapImage(new Uri(@"resources\hextypes\grain.png", UriKind.Relative)) },
        { ResourceTypes.Lumber, new BitmapImage(new Uri(@"resources\hextypes\lumber.png", UriKind.Relative)) },
        { ResourceTypes.Ore, new BitmapImage(new Uri(@"resources\hextypes\ore.png", UriKind.Relative)) },
        { ResourceTypes.Wool, new BitmapImage(new Uri(@"resources\hextypes\wool.png", UriKind.Relative)) }
      };
#pragma warning restore IDE0009 // Member access should be qualified.
    }

    private void GetBitmaps(Tuple<ResourceTypes?, uint> hexData, Dictionary<ResourceTypes, BitmapImage> resourceBitmaps, Dictionary<uint, BitmapImage> numberBitmaps, out BitmapImage resourceBitmap, out BitmapImage numberBitmap)
    {
      if (!hexData.Item1.HasValue)
      {
        resourceBitmap = new BitmapImage(new Uri(@"resources\hextypes\desert.png", UriKind.Relative));
      }
      else
      {
        resourceBitmap = resourceBitmaps[hexData.Item1.Value];
      }

      numberBitmap = (hexData.Item2 != 0 ? numberBitmaps[hexData.Item2] : null);
    }

    private void PlaceHex(BitmapImage resourceBitmap, BitmapImage numberBitmap, int x, int y)
    {
      var resourceImage = this.CreateImage(resourceBitmap, string.Empty);
      this.BoardLayer.Children.Add(resourceImage);
      Canvas.SetLeft(resourceImage, x);
      Canvas.SetTop(resourceImage, y);

      if (numberBitmap == null)
      {
        return;
      }

      var numberImage = this.CreateImage(numberBitmap, string.Empty);
      this.BoardLayer.Children.Add(numberImage);
      Canvas.SetLeft(numberImage, x);
      Canvas.SetTop(numberImage, y);
    }

    private void PlaceSettlementButton(double x, double y, uint id, string toolTip)
    {
      var button = new SettlementButtonControl(id, this.SettlementSelectedEventHandler);
      button.ToolTip = toolTip;
      this.SettlementLayer.Children.Add(button);
      Canvas.SetLeft(button, x);
      Canvas.SetTop(button, y);
    }

    uint workingLocation;

    private void SettlementSelectedEventHandler(uint location)
    {
      this.workingLocation = location;

      var roadEndLocations = this.board.BoardQuery.GetValidConnectedLocationsFrom(location);
    }

    private void StartGameButton_Click(object sender, RoutedEventArgs e)
    {
      this.StartGameButton.Visibility = Visibility.Hidden;
      this.TopLayer.Visibility = Visibility.Hidden;

      this.BoardLayer.Visibility = Visibility.Visible;
      this.SettlementLayer.Visibility = Visibility.Visible;
    }
    #endregion

    #region Structures
    private struct HexLayoutData
    {
      public int X, Y;
      public uint Count;
    }

    public struct SettlementLayoutData
    {
      public int X, Y, Dx, Dy, DirectionX;
      public uint Count;
    }
    #endregion
  }
}
