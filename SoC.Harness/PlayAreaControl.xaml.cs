
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
      this.roadButtonControls = new RoadButtonControl[GameBoard.StandardBoardTrailCount];

      // Column 1 Vertical
      int dy = 43 + 2;
      this.PlaceRoadControl(210, 89, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(210, 132, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(210, 177, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(210, 220, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(210, 266, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(210, 309, @"resources\roads\road_left_indicator.png");

      // Column 2 Vertical
      this.PlaceRoadControl(277, 44, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(277, 89, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(277, 132, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(277, 177, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(277, 220, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(277, 266, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(277, 309, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(277, 354, @"resources\roads\road_left_indicator.png");

      // Column 3 Vertical
      this.PlaceRoadControl(342, 1, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(342, 44, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(342, 89, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(342, 132, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(342, 177, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(342, 220, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(342, 266, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(342, 309, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(342, 354, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(342, 399, @"resources\roads\road_left_indicator.png");

      // Column 4 Vertical
      this.PlaceRoadControl(409, 1, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(409, 44, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(409, 89, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(409, 132, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(409, 177, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(409, 220, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(409, 266, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(409, 309, @"resources\roads\road_right_indicator.png");
      this.PlaceRoadControl(409, 354, @"resources\roads\road_left_indicator.png");
      this.PlaceRoadControl(409, 399, @"resources\roads\road_right_indicator.png");

      // Column 1 Horizontal
      dy = 88 + 1;
      this.PlaceRoadControl(246, 87, @"resources\roads\road_horizontal_indicator.png");
      this.PlaceRoadControl(246, 175, @"resources\roads\road_horizontal_indicator.png");
      this.PlaceRoadControl(246, 264, @"resources\roads\road_horizontal_indicator.png");
      this.PlaceRoadControl(246, 352, @"resources\roads\road_horizontal_indicator.png");

      // Column 2 Horizontal
      this.PlaceRoadControl(312, 42, @"resources\roads\road_horizontal_indicator.png");
      this.PlaceRoadControl(312, 130, @"resources\roads\road_horizontal_indicator.png");
      this.PlaceRoadControl(312, 218, @"resources\roads\road_horizontal_indicator.png");
      this.PlaceRoadControl(312, 306, @"resources\roads\road_horizontal_indicator.png");
      this.PlaceRoadControl(312, 394, @"resources\roads\road_horizontal_indicator.png");
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
      this.RoadSelectionLayer.Visibility = Visibility.Hidden;
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
