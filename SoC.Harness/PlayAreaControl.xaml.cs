
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
    private Dictionary<string, RoadButtonControl> roadButtonControls;

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
      this.roadButtonControls = new Dictionary<string, RoadButtonControl>();

      var verticalRoadLayoutData = new VerticalRoadLayoutData[]
      {
        new VerticalRoadLayoutData
        {
          XCoordinate = 210,
          StartWithRightImage = true,
          Locations = new int[] { 1, 2, 3, 4, 5, 6 },
          YCoordinates = new int[] { 89, 132, 177, 220, 266, 309 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 277,
          StartWithRightImage = true,
          Locations = new int[] { 8, 9, 10, 11, 12, 13, 14, 15 },
          YCoordinates = new int[] { 44, 89, 132, 177, 220, 266, 309, 354 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 342,
          StartWithRightImage = true,
          Locations = new int[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 },
          YCoordinates = new int[] { 1, 44, 89, 132, 177, 220, 266, 309, 354, 399 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 408,
          StartWithRightImage = false,
          Locations = new int[] { 28, 29, 30, 31, 32, 33, 34, 35, 36, 37 },
          YCoordinates = new int[] { 1, 44, 89, 132, 177, 220, 266, 309, 354, 399 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 472,
          StartWithRightImage = false,
          Locations = new int[] { 39, 40, 41, 42, 43, 44, 45, 46 },
          YCoordinates = new int[] { 44, 89, 132, 177, 220, 266, 309, 354 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 538,
          StartWithRightImage = false,
          Locations = new int[] { 48, 49, 50, 51, 52, 53 },
          YCoordinates = new int[] { 89, 132, 177, 220, 266, 309 }
        }
      };

      // Column 1 Vertical
      //int dy = 43 + 2;
      //int x = 210;
      //var locations = new int[] { 1, 2, 3, 4, 5, 6 };
      //var yList = new int[] { 89, 132, 177, 220, 266, 309 };
      //var useRightImage = true;
      //var imagePath = rightRoadImagePath;
      foreach (var verticalRoadLayout in verticalRoadLayoutData)
      {
        var useRightImage = verticalRoadLayout.StartWithRightImage;
        for (var index = 0; index < verticalRoadLayout.Locations.Length; index++)
        {
          var imagePath = useRightImage ? rightRoadImagePath : leftRoadImagePath;
          var locationA = verticalRoadLayout.Locations[index];
          var locationB = locationA - 1;
          var id = string.Format("{0}-{1}", locationA, locationB);
          var control = this.PlaceRoadButtonControl(id, verticalRoadLayout.XCoordinate, verticalRoadLayout.YCoordinates[index], imagePath);
          useRightImage = !useRightImage;

          this.roadButtonControls.Add(id, control);
          id = string.Format("{0}-{1}", locationB, locationA);
          this.roadButtonControls.Add(id, control);
        }
      }

      // Column 2 Vertical
      /*this.PlaceRoadButtonControl(277, 44, rightRoadImagePath);
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
      this.PlaceRoadButtonControl(538, 309, rightRoadImagePath);*/

      var horizontalRoadLayoutData = new HorizontalRoadLayoutData[]
      {
        new HorizontalRoadLayoutData
        {
          XCoordinate = 246,
          Locations = new [] { new Point(0, 8), new Point(2, 10), new Point(4, 12), new Point(6, 14)},
          YCoordinates = new int[] { 87, 175, 264, 352 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 312,
          Locations = new [] { new Point(7, 17), new Point(9, 19), new Point(11, 21), new Point(13, 23), new Point(15, 25)},
          YCoordinates = new int[] { 42, 130, 218, 308, 395 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 378,
          Locations = new [] { new Point(16, 27), new Point(18, 29), new Point(20, 31), new Point(22, 33), new Point(24, 35), new Point(26, 37)},
          YCoordinates = new int[] { -3, 85, 174, 263, 354, 441 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 444,
          Locations = new [] { new Point(28, 38), new Point(30, 40), new Point(32, 42), new Point(34, 44), new Point(36, 46)},
          YCoordinates = new int[] { 42, 130, 218, 308, 395 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 510,
          Locations = new [] { new Point(39, 47), new Point(41, 49), new Point(43, 51), new Point(45, 53)},
          YCoordinates = new int[] { 87, 175, 264, 352 }
        }
      };

      foreach (var horizontalRoadLayout in horizontalRoadLayoutData)
      {
        for (var index = 0; index < horizontalRoadLayout.Locations.Length; index++)
        {
          var locationA = horizontalRoadLayout.Locations[index].X;
          var locationB = horizontalRoadLayout.Locations[index].Y;
          var id = string.Format("{0}-{1}", locationA, locationB);
          var control = this.PlaceRoadButtonControl(id, horizontalRoadLayout.XCoordinate, horizontalRoadLayout.YCoordinates[index], horzRoadImagePath);

          this.roadButtonControls.Add(id, control);
          id = string.Format("{0}-{1}", locationB, locationA);
          this.roadButtonControls.Add(id, control);
        }
      }

      // Column 1 Horizontal
      //int dy = 88 + 1;
      /*this.PlaceRoadButtonControl(246, 87, horzRoadImagePath);
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
      this.PlaceRoadButtonControl(510, 352, horzRoadImagePath);*/
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

    private RoadButtonControl PlaceRoadButtonControl(string id, double x, double y, string imagePath)
    {
      var control = new RoadButtonControl(id, imagePath, null);
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
        var id = string.Format("{0}-{1}", workingLocation, roadEndLocations[index]);
        this.roadButtonControls[id].Visibility = Visibility.Visible;
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

    public struct VerticalRoadLayoutData
    {
      public int XCoordinate;
      public int[] Locations, YCoordinates;
      public bool StartWithRightImage;
    }

    public struct HorizontalRoadLayoutData
    {
      public int XCoordinate;
      public Point[] Locations;
      public int[] YCoordinates;
    }
    #endregion
  }
}
