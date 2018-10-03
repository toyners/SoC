
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
    #region Fields
    private IGameBoard board;
    private SettlementButtonControl[] settlementButtonControls;
    private Dictionary<string, RoadButtonControl> roadButtonControls;
    uint workingLocation;
    RoadButtonControl workingRoadControl;
    #endregion

    #region Construction
    public PlayAreaControl()
    {
      this.InitializeComponent();
    }
    #endregion

    #region Events
    public Action<int, object> EndTurnEvent;
    #endregion

    #region Methods
    public void Initialise(IGameBoard board)
    {
      this.board = board;

      this.InitialiseBoardLayer();

      this.InitialiseSettlementSelectionLayer();

      this.InitialiseRoadSelectionLayer();
    }

    private void InitialiseBoardLayer()
    {
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
    }

    private void InitialiseSettlementSelectionLayer()
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

    private void InitialiseRoadSelectionLayer()
    {
      string roadLeftIndicatorImagePath = @"resources\roads\road_left_indicator.png";
      string roadLeftImagePath = @"resources\roads\road_left.png";
      string roadRightIndicatorImagePath = @"resources\roads\road_right_indicator.png";
      string roadRightImagePath = @"resources\roads\road_right.png";
      string roadHorizontalIndicatorImagePath = @"resources\roads\road_horizontal_indicator.png";
      string roadHorizontalImagePath = @"resources\roads\road_horizontal.png";
      this.roadButtonControls = new Dictionary<string, RoadButtonControl>();

      var verticalRoadLayoutData = this.GetVerticalRoadLayoutData();

      string indicatorImagePath = null;
      string imagePath = null;
      foreach (var verticalRoadLayout in verticalRoadLayoutData)
      {
        var useRightImage = verticalRoadLayout.StartWithRightImage;
        for (var index = 0; index < verticalRoadLayout.Locations.Length; index++)
        {
          if (useRightImage)
          {
            indicatorImagePath = roadRightIndicatorImagePath;
            imagePath = roadRightImagePath;
          }
          else
          {
            indicatorImagePath = roadLeftIndicatorImagePath;
            imagePath = roadLeftImagePath;
          }

          var locationA = verticalRoadLayout.Locations[index];
          var locationB = locationA - 1;
          var control = this.PlaceRoadButtonControl(locationA, locationB, verticalRoadLayout.XCoordinate, verticalRoadLayout.YCoordinates[index], indicatorImagePath, imagePath);
          useRightImage = !useRightImage;

          this.roadButtonControls.Add(control.Id, control);
          this.roadButtonControls.Add(control.AlternativeId, control);
        }
      }

      var horizontalRoadLayoutData = this.GetHorizontalRoadLayoutData();

      foreach (var horizontalRoadLayout in horizontalRoadLayoutData)
      {
        for (var index = 0; index < horizontalRoadLayout.Locations.Length; index++)
        {
          var locationA = horizontalRoadLayout.Locations[index].Start;
          var locationB = horizontalRoadLayout.Locations[index].End;
          var control = this.PlaceRoadButtonControl(locationA, locationB, horizontalRoadLayout.XCoordinate, horizontalRoadLayout.YCoordinates[index], roadHorizontalIndicatorImagePath, roadHorizontalImagePath);

          this.roadButtonControls.Add(control.Id, control);
          this.roadButtonControls.Add(control.AlternativeId, control);
        }
      }
    }

    private HorizontalRoadLayoutData[] GetHorizontalRoadLayoutData()
    {
      return new HorizontalRoadLayoutData[]
      {
        new HorizontalRoadLayoutData
        {
          XCoordinate = 246,
          Locations = new [] { new RoadData(0, 8), new RoadData(2, 10), new RoadData(4, 12), new RoadData(6, 14)},
          YCoordinates = new int[] { 87, 175, 264, 352 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 312,
          Locations = new [] { new RoadData(7, 17), new RoadData(9, 19), new RoadData(11, 21), new RoadData(13, 23), new RoadData(15, 25)},
          YCoordinates = new int[] { 42, 130, 218, 308, 395 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 378,
          Locations = new [] { new RoadData(16, 27), new RoadData(18, 29), new RoadData(20, 31), new RoadData(22, 33), new RoadData(24, 35), new RoadData(26, 37)},
          YCoordinates = new int[] { -3, 85, 174, 263, 354, 441 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 444,
          Locations = new [] { new RoadData(28, 38), new RoadData(30, 40), new RoadData(32, 42), new RoadData(34, 44), new RoadData(36, 46)},
          YCoordinates = new int[] { 42, 130, 218, 308, 395 }
        },
        new HorizontalRoadLayoutData
        {
          XCoordinate = 510,
          Locations = new [] { new RoadData(39, 47), new RoadData(41, 49), new RoadData(43, 51), new RoadData(45, 53)},
          YCoordinates = new int[] { 87, 175, 264, 352 }
        }
      };
    }

    private VerticalRoadLayoutData[] GetVerticalRoadLayoutData()
    {
      return new VerticalRoadLayoutData[]
      {
        new VerticalRoadLayoutData
        {
          XCoordinate = 210,
          StartWithRightImage = true,
          Locations = new uint[] { 1, 2, 3, 4, 5, 6 },
          YCoordinates = new int[] { 89, 132, 177, 220, 266, 309 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 277,
          StartWithRightImage = true,
          Locations = new uint[] { 8, 9, 10, 11, 12, 13, 14, 15 },
          YCoordinates = new int[] { 44, 89, 132, 177, 220, 266, 309, 354 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 342,
          StartWithRightImage = true,
          Locations = new uint[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 },
          YCoordinates = new int[] { 1, 44, 89, 132, 177, 220, 266, 309, 354, 399 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 408,
          StartWithRightImage = false,
          Locations = new uint[] { 28, 29, 30, 31, 32, 33, 34, 35, 36, 37 },
          YCoordinates = new int[] { 1, 44, 89, 132, 177, 220, 266, 309, 354, 399 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 472,
          StartWithRightImage = false,
          Locations = new uint[] { 39, 40, 41, 42, 43, 44, 45, 46 },
          YCoordinates = new int[] { 44, 89, 132, 177, 220, 266, 309, 354 }
        },
        new VerticalRoadLayoutData
        {
          XCoordinate = 538,
          StartWithRightImage = false,
          Locations = new uint[] { 48, 49, 50, 51, 52, 53 },
          YCoordinates = new int[] { 89, 132, 177, 220, 266, 309 }
        }
      };
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

    private RoadButtonControl PlaceRoadButtonControl(uint start, uint end, double x, double y, string imagePath, string path)
    {
      var control = new RoadButtonControl(start, end, x, y, imagePath, path, this.RoadSelectedEventHandler);
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

    private void PlaceBuildingControl(double x, double y, string toolTip, string imagePath, Canvas canvas)
    {
      var control = new BuildingControl(imagePath);
      control.ToolTip = toolTip;
      canvas.Children.Add(control);
      Canvas.SetLeft(control, x);
      Canvas.SetTop(control, y);
    }

    private void RoadSelectedEventHandler(RoadButtonControl roadButtonControl)
    {
      this.workingRoadControl = roadButtonControl;
      roadButtonControl.Visibility = Visibility.Hidden;

      this.PlaceBuildingControl(roadButtonControl.X, roadButtonControl.Y, string.Empty, roadButtonControl.ImagePath, this.RoadLayer);

      this.RoadSelectionLayer.Visibility = Visibility.Hidden;

      this.EndTurnButton.Visibility = Visibility.Visible;
      this.TopLayer.Visibility = Visibility.Visible;
    }

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

      this.PlaceBuildingControl(settlementButtonControl.X, settlementButtonControl.Y, string.Empty, @"resources\settlements\blue_settlement.png", this.SettlementLayer);
      
      // Turn on the possible road controls for the location
      var roadEndLocations = this.board.BoardQuery.GetValidConnectedLocationsFrom(this.workingLocation);
      for (var index = 0; index < roadEndLocations.Length; index++)
      {
        var id = string.Format("{0}-{1}", this.workingLocation, roadEndLocations[index]);
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

    private void EndTurnButton_Click(object sender, RoutedEventArgs e)
    {
      var roadEndLocation = this.workingRoadControl.Start == this.workingLocation ? this.workingRoadControl.End : this.workingRoadControl.Start;
      var infrastructureInstructions = new Tuple<uint, uint>(this.workingLocation, roadEndLocation);
      this.EndTurnEvent?.Invoke(1, infrastructureInstructions);
    }
    #endregion

    #region Structures
    private struct HexLayoutData
    {
      public int X, Y;
      public uint Count;
    }

    public struct RoadData
    {
      public uint Start, End;

      public RoadData(uint start, uint end)
      {
        this.Start = start;
        this.End = end;
      }
    }

    public struct SettlementLayoutData
    {
      public int X, Y, Dx, Dy, DirectionX;
      public uint Count;
    }

    public struct VerticalRoadLayoutData
    {
      public int XCoordinate;
      public uint[] Locations;
      public int[] YCoordinates;
      public bool StartWithRightImage;
    }

    public struct HorizontalRoadLayoutData
    {
      public int XCoordinate;
      public RoadData[] Locations;
      public int[] YCoordinates;
    }
    #endregion
  }
}
