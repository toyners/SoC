
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
    private SettlementButtonControl[] settlementButtonControls;
    private RoadButtonControl[] roadButtonControls;

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

      this.InitialiseSettlementSelectionLayer();

      this.InitialiseRoadSelectionLayer();
    }

    public void InitialiseSettlementSelectionLayer()
    {
      this.settlementButtonControls = new SettlementButtonControl[GameBoard.StandardBoardLocationCount];

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
          var control = this.PlaceSettlementButtonControl(x, y, location, "Test");
          this.settlementButtonControls[location++] = control;
          y += dy;

          x += direction * dx;
          direction = direction == -1 ? 1 : -1;
        }
      }
    }

    public void InitialiseRoadSelectionLayer()
    {
      string leftRoadImagePath = @"resources\roads\road_left_indicator.png";
      string rightRoadImagePath = @"resources\roads\road_right_indicator.png";
      string horzRoadImagePath = @"resources\roads\road_horizontal_indicator.png";
      this.roadButtonControls = new RoadButtonControl[GameBoard.StandardBoardTrailCount];

      // Column 1 Vertical
      int dy = 43 + 2;
      this.PlaceRoadButtonControl(210, 89, rightRoadImagePath);
      this.PlaceRoadButtonControl(210, 132, leftRoadImagePath);
      this.PlaceRoadButtonControl(210, 177, rightRoadImagePath);
      this.PlaceRoadButtonControl(210, 220, leftRoadImagePath);
      this.PlaceRoadButtonControl(210, 266, rightRoadImagePath);
      this.PlaceRoadButtonControl(210, 309, leftRoadImagePath);

      // Column 2 Vertical
      this.PlaceRoadButtonControl(277, 44, rightRoadImagePath);
      this.PlaceRoadButtonControl(277, 89, leftRoadImagePath);
      this.PlaceRoadButtonControl(277, 132, rightRoadImagePath);
      this.PlaceRoadButtonControl(277, 177, leftRoadImagePath);
      this.PlaceRoadButtonControl(277, 220, rightRoadImagePath);
      this.PlaceRoadButtonControl(277, 266, leftRoadImagePath);
      this.PlaceRoadButtonControl(277, 309, rightRoadImagePath);
      this.PlaceRoadButtonControl(277, 354, leftRoadImagePath);

      // Column 3 Vertical
      this.PlaceRoadButtonControl(342, 1, rightRoadImagePath);
      this.PlaceRoadButtonControl(342, 44, leftRoadImagePath);
      this.PlaceRoadButtonControl(342, 89, rightRoadImagePath);
      this.PlaceRoadButtonControl(342, 132, leftRoadImagePath);
      this.PlaceRoadButtonControl(342, 177, rightRoadImagePath);
      this.PlaceRoadButtonControl(342, 220, leftRoadImagePath);
      this.PlaceRoadButtonControl(342, 266, rightRoadImagePath);
      this.PlaceRoadButtonControl(342, 309, leftRoadImagePath);
      this.PlaceRoadButtonControl(342, 354, rightRoadImagePath);
      this.PlaceRoadButtonControl(342, 399, leftRoadImagePath);

      // Column 4 Vertical
      this.PlaceRoadButtonControl(408, 1, leftRoadImagePath);
      this.PlaceRoadButtonControl(408, 44, rightRoadImagePath);
      this.PlaceRoadButtonControl(408, 89, leftRoadImagePath);
      this.PlaceRoadButtonControl(408, 132, rightRoadImagePath);
      this.PlaceRoadButtonControl(408, 177, leftRoadImagePath);
      this.PlaceRoadButtonControl(408, 220, rightRoadImagePath);
      this.PlaceRoadButtonControl(408, 266, leftRoadImagePath);
      this.PlaceRoadButtonControl(408, 309, rightRoadImagePath);
      this.PlaceRoadButtonControl(408, 354, leftRoadImagePath);
      this.PlaceRoadButtonControl(408, 399, rightRoadImagePath);

      // Column 5 Vertical
      this.PlaceRoadButtonControl(472, 44, leftRoadImagePath);
      this.PlaceRoadButtonControl(472, 89, rightRoadImagePath);
      this.PlaceRoadButtonControl(472, 132, leftRoadImagePath);
      this.PlaceRoadButtonControl(472, 177, rightRoadImagePath);
      this.PlaceRoadButtonControl(472, 220, leftRoadImagePath);
      this.PlaceRoadButtonControl(472, 266, rightRoadImagePath);
      this.PlaceRoadButtonControl(472, 309, leftRoadImagePath);
      this.PlaceRoadButtonControl(472, 354, rightRoadImagePath);

      // Column 6 Vertical
      this.PlaceRoadButtonControl(538, 89, leftRoadImagePath);
      this.PlaceRoadButtonControl(538, 132, rightRoadImagePath);
      this.PlaceRoadButtonControl(538, 177, leftRoadImagePath);
      this.PlaceRoadButtonControl(538, 220, rightRoadImagePath);
      this.PlaceRoadButtonControl(538, 266, leftRoadImagePath);
      this.PlaceRoadButtonControl(538, 309, rightRoadImagePath);

      // Column 1 Horizontal
      dy = 88 + 1;
      this.PlaceRoadButtonControl(246, 87, horzRoadImagePath);
      this.PlaceRoadButtonControl(246, 175, horzRoadImagePath);
      this.PlaceRoadButtonControl(246, 264, horzRoadImagePath);
      this.PlaceRoadButtonControl(246, 352, horzRoadImagePath);

      // Column 2 Horizontal
      this.PlaceRoadButtonControl(312, 42, horzRoadImagePath);
      this.PlaceRoadButtonControl(312, 130, horzRoadImagePath);
      this.PlaceRoadButtonControl(312, 218, horzRoadImagePath);
      this.PlaceRoadButtonControl(312, 308, horzRoadImagePath);
      this.PlaceRoadButtonControl(312, 395, horzRoadImagePath);

      // Column 3 Horizontal
      this.PlaceRoadButtonControl(378, -3, horzRoadImagePath);
      this.PlaceRoadButtonControl(378, 85, horzRoadImagePath);
      this.PlaceRoadButtonControl(378, 174, horzRoadImagePath);
      this.PlaceRoadButtonControl(378, 263, horzRoadImagePath);
      this.PlaceRoadButtonControl(378, 354, horzRoadImagePath);
      this.PlaceRoadButtonControl(378, 441, horzRoadImagePath);

      // Column 4 Horizontal
      this.PlaceRoadButtonControl(444, 42, horzRoadImagePath);
      this.PlaceRoadButtonControl(444, 130, horzRoadImagePath);
      this.PlaceRoadButtonControl(444, 218, horzRoadImagePath);
      this.PlaceRoadButtonControl(444, 308, horzRoadImagePath);
      this.PlaceRoadButtonControl(444, 395, horzRoadImagePath);

      // Column 5 Horizontal
      this.PlaceRoadButtonControl(510, 87, horzRoadImagePath);
      this.PlaceRoadButtonControl(510, 175, horzRoadImagePath);
      this.PlaceRoadButtonControl(510, 264, horzRoadImagePath);
      this.PlaceRoadButtonControl(510, 352, horzRoadImagePath);
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

    private void PlaceRoadControl(double x, double y, string imagePath)
    {
      var control = new RoadControl(imagePath);
      this.RoadLayer.Children.Add(control);
      Canvas.SetLeft(control, x);
      Canvas.SetTop(control, y);
    }

    private RoadButtonControl PlaceRoadButtonControl(double x, double y, string imagePath)
    {
      var control = new RoadButtonControl(imagePath, null);
      this.RoadSelectionLayer.Children.Add(control);
      Canvas.SetLeft(control, x);
      Canvas.SetTop(control, y);

      return control;
    }

    private SettlementButtonControl PlaceSettlementButtonControl(double x, double y, uint id, string toolTip)
    {
      var control = new SettlementButtonControl(id, x, y, this.SettlementSelectedEventHandler);
      control.ToolTip = toolTip;
      this.SettlementSelectionLayer.Children.Add(control);
      Canvas.SetLeft(control, x);
      Canvas.SetTop(control, y);

      return control;
    }

    private void PlaceSettlementControl(double x, double y, string toolTip, string imagePath)
    {
      var control = new SettlementControl(imagePath);
      control.ToolTip = toolTip;
      this.SettlementLayer.Children.Add(control);
      Canvas.SetLeft(control, x);
      Canvas.SetTop(control, y);
    }

    uint workingLocation;

    private void SettlementSelectedEventHandler(SettlementButtonControl settlementButtonControl)
    {
      this.workingLocation = settlementButtonControl.Location;

      // Turn off the controls for the location and its neighbours
      settlementButtonControl.Visibility = Visibility.Hidden;
      var neighbouringLocations = this.board.BoardQuery.GetNeighbouringLocationsFrom(this.workingLocation);
      for (var index = 0; index < neighbouringLocations.Length; index++)
      {
        this.settlementButtonControls[index].Visibility = Visibility.Hidden;
      }

      this.PlaceSettlementControl(settlementButtonControl.X, settlementButtonControl.Y, "Test", @"resources\settlements\blue_settlement.png");
      
      // Turn on the possible road controls for the location
      var roadEndLocations = this.board.BoardQuery.GetValidConnectedLocationsFrom(this.workingLocation);
      for (var index = 0; index < roadEndLocations.Length; index++)
      {
        this.roadButtonControls[index].Visibility = Visibility.Visible;
      }

      this.SettlementSelectionLayer.Visibility = Visibility.Hidden;
      this.RoadSelectionLayer.Visibility = Visibility.Visible;
    }

    private void StartGameButton_Click(object sender, RoutedEventArgs e)
    {
      this.StartGameButton.Visibility = Visibility.Hidden;
      this.TopLayer.Visibility = Visibility.Hidden;

      this.BoardLayer.Visibility = Visibility.Visible;
      this.SettlementSelectionLayer.Visibility = Visibility.Visible;
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
