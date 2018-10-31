
namespace SoC.Harness.Views
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media.Imaging;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;
  using SoC.Harness.ViewModels;

  /// <summary>
  /// Interaction logic for BoardUI.xaml
  /// </summary>
  public partial class PlayAreaControl : UserControl, INotifyPropertyChanged
  {
    private enum States
    {
      AwaitingFirstInfrastructure,
      AwaitingSecondInfrastructure,
      AwaitingResourceDropSelection,
      RobberLocationSelection,
      RobbedPlayerSelection,
      Unknown,
    }

    #region Fields
    private const string blueSettlementImagePath = @"..\resources\settlements\blue_settlement.png";
    private const string redSettlementImagePath = @"..\resources\settlements\red_settlement.png";
    private const string greenSettlementImagePath = @"..\resources\settlements\green_settlement.png";
    private const string yellowSettlementImagePath = @"..\resources\settlements\yellow_settlement.png";

    private const string blueRoadHorizontalImagePath = @"..\resources\roads\blue_road_horizontal.png";
    private const string blueRoadLeftImagePath = @"..\resources\roads\blue_road_left.png";
    private const string blueRoadRightImagePath = @"..\resources\roads\blue_road_right.png";
    private const string redRoadHorizontalImagePath = @"..\resources\roads\red_road_horizontal.png";
    private const string redRoadLeftImagePath = @"..\resources\roads\red_road_left.png";
    private const string redRoadRightImagePath = @"..\resources\roads\red_road_right.png";
    private const string greenRoadHorizontalImagePath = @"..\resources\roads\green_road_horizontal.png";
    private const string greenRoadLeftImagePath = @"..\resources\roads\green_road_left.png";
    private const string greenRoadRightImagePath = @"..\resources\roads\green_road_right.png";
    private const string yellowRoadHorizontalImagePath = @"..\resources\roads\yellow_road_horizontal.png";
    private const string yellowRoadLeftImagePath = @"..\resources\roads\yellow_road_left.png";
    private const string yellowRoadRightImagePath = @"..\resources\roads\yellow_road_right.png";

    private IGameBoard board;
    private SettlementButtonControl[] settlementButtonControls;
    private Dictionary<string, RoadButtonControl> roadButtonControls;
    private uint workingLocation;
    private RoadButtonControl workingRoadControl;
    private Dictionary<Guid, string> settlementImagesByPlayerId;
    private Dictionary<Guid, string[]> roadImagesByPlayerId;
    private Guid playerId;
    private HashSet<RoadButtonControl> visibleRoadButtonControls = new HashSet<RoadButtonControl>();
    private IList<ResourceButton> resourceControls = new List<ResourceButton>();
    private string resourceSelectionMessage;
    private PropertyChangedEventArgs confirmMessageChanged = new PropertyChangedEventArgs("ResourceSelectionMessage");
    private Image robberImage, selectedRobberLocationImage;
    private ControllerViewModel controllerViewModel;
    private int workingNumberOfResourcesToSelect;
    private States state = States.AwaitingFirstInfrastructure;
    private Image currentRobberLocationHoverImage = null;
    private Dictionary<Image, Tuple<uint, Point>> locationsByImage = new Dictionary<Image, Tuple<uint, Point>>();
    #endregion

    #region Construction
    public PlayAreaControl()
    {
      this.DataContext = this;
      this.InitializeComponent();
    }
    #endregion

    #region Properties
    public string DiceOneImagePath { get; private set; }
    public string DiceTwoImagePath { get; private set; }
    public string ResourceSelectionMessage
    {
      get { return this.resourceSelectionMessage; }
      private set
      {
        this.resourceSelectionMessage = value;
        this.PropertyChanged.Invoke(this, this.confirmMessageChanged);
      }
    }
    #endregion

    #region Events
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion

    #region Methods
    public void Initialise(ControllerViewModel controllerViewModel)
    {
      this.controllerViewModel = controllerViewModel;
      this.controllerViewModel.GameJoinedEvent += this.InitialisePlayerViews;
      this.controllerViewModel.InitialBoardSetupEvent += this.Initialise;
      this.controllerViewModel.BoardUpdatedEvent += this.BoardUpdatedEventHandler;
      this.controllerViewModel.DiceRollEvent += this.DiceRollEventHandler;
      this.controllerViewModel.RobberEvent += this.RobberEventHandler;
      this.controllerViewModel.RobbingChoicesEvent += this.RobbingChoicesEventHandler;
    }

    public void InitialisePlayerViews(PlayerViewModel player1, PlayerViewModel player2, PlayerViewModel player3, PlayerViewModel player4)
    {
      this.playerId = player1.Id;

      this.settlementImagesByPlayerId = new Dictionary<Guid, string>();
      this.settlementImagesByPlayerId.Add(player1.Id, blueSettlementImagePath);
      this.settlementImagesByPlayerId.Add(player2.Id, redSettlementImagePath);
      this.settlementImagesByPlayerId.Add(player3.Id, greenSettlementImagePath);
      this.settlementImagesByPlayerId.Add(player4.Id, yellowSettlementImagePath);

      this.roadImagesByPlayerId = new Dictionary<Guid, string[]>();
      this.roadImagesByPlayerId.Add(player1.Id, new[] { blueRoadHorizontalImagePath, blueRoadLeftImagePath, blueRoadRightImagePath });
      this.roadImagesByPlayerId.Add(player2.Id, new[] { redRoadHorizontalImagePath, redRoadLeftImagePath, redRoadRightImagePath });
      this.roadImagesByPlayerId.Add(player3.Id, new[] { greenRoadHorizontalImagePath, greenRoadLeftImagePath, greenRoadRightImagePath });
      this.roadImagesByPlayerId.Add(player4.Id, new[] { yellowRoadHorizontalImagePath, yellowRoadLeftImagePath, yellowRoadRightImagePath });
    }

    public void BoardUpdatedEventHandler(GameBoardUpdate boardUpdate)
    {
      if (boardUpdate == null)
      {
        return; // Should a null boardupdate be valid?
      }
      
      foreach (var settlementDetails in boardUpdate.NewSettlements)
      {
        var location = settlementDetails.Item1;
        var playerId = settlementDetails.Item2;
        var settlementImagePath = this.settlementImagesByPlayerId[playerId];

        var control = this.settlementButtonControls[location];
        this.PlaceBuildingControl(control.X, control.Y, location.ToString(), settlementImagePath, this.SettlementLayer);

        control.Visibility = Visibility.Hidden;

        var neighbouringLocations = this.board.BoardQuery.GetNeighbouringLocationsFrom(location);
        foreach (var neighbouringLocation in neighbouringLocations)
        {
          this.settlementButtonControls[neighbouringLocation].Visibility = Visibility.Hidden;
        }
      }

      foreach (var roadDetails in boardUpdate.NewRoads)
      {
        var startLocation = roadDetails.Item1;
        var endLocation = roadDetails.Item2;
        var playerId = roadDetails.Item3;

        var key = $"{startLocation}-{endLocation}";
        var control = this.roadButtonControls[key];

        var roadImagePath = this.roadImagesByPlayerId[playerId][(int)control.RoadImageType];
        
        control.Visibility = Visibility.Hidden;

        this.PlaceRoadControl(control.X, control.Y, roadImagePath);
      }

      this.SettlementSelectionLayer.Visibility = Visibility.Visible;
    }

    public void DiceRollEventHandler(uint dice1, uint dice2)
    {
      this.DiceOneImagePath = this.GetDiceImage(dice1);
      this.DiceTwoImagePath = this.GetDiceImage(dice2);

      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DiceOneImagePath"));
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DiceTwoImagePath"));
    }

    public void RobberEventHandler(PlayerViewModel player, int numberOfResourcesToSelect)
    {
      if (numberOfResourcesToSelect > 0)
      {
        // Display resources for player to discard
        this.workingNumberOfResourcesToSelect = numberOfResourcesToSelect;
        this.ResourceSelectionMessage = $"Select {this.workingNumberOfResourcesToSelect} more resources to drop";
        this.ResourceSelectionConfirmButton.IsEnabled = false;

        var width = 100;
        var gutter = 10;
        var midX = 400;
        var midY = 200;
        var x = midX - ((player.Resources.Count * width) + ((player.Resources.Count - 1) * gutter) / 2);

        int resourceIndex = 0;
        for (; resourceIndex < player.Resources.Count; resourceIndex++)
        {
          if (resourceIndex >= this.resourceControls.Count)
          {
            // Need a new resource control
            var newButton = new ResourceButton(this.ResourceSelectedEventHandler);
            this.resourceControls.Add(newButton);
            this.ResourceSelectionLayer.Children.Add(newButton);
          }
          
          var resourceButton = this.resourceControls[resourceIndex];

          var resourceType = this.GetResourceTypeAt(resourceIndex, player.Resources);
          this.GetResourceCardImages(resourceType, out var imagePath, out var selectedImagePath);
          resourceButton.OriginalImagePath = resourceButton.ImagePath = imagePath;
          resourceButton.SelectedImagePath = selectedImagePath;
          resourceButton.ResourceType = resourceType;

          Canvas.SetLeft(resourceButton, x);
          Canvas.SetTop(resourceButton, midY);
          x += width + gutter;

          resourceButton.Visibility = Visibility.Visible;
        }

        // Hide resource controls that are not needed this time.
        var resourceControlIndex = resourceIndex;
        for (; resourceControlIndex < this.resourceControls.Count; resourceControlIndex++)
        {
          this.resourceControls[resourceControlIndex].Visibility = Visibility.Hidden;
        }

        this.ResourceSelectionLayer.Visibility = Visibility.Visible;
        return;
      }

      // Select hex to place robber
      this.RobberSelectionLayer.Visibility = Visibility.Visible;
      this.state = States.RobberLocationSelection;
    }

    private ResourceTypes GetResourceTypeAt(int index, ResourceClutch resources)
    {
      if (index < resources.BrickCount)
        return ResourceTypes.Brick;

      if (index < resources.BrickCount + resources.GrainCount)
        return ResourceTypes.Grain;

      if (index < resources.BrickCount + resources.GrainCount + resources.LumberCount)
        return ResourceTypes.Lumber;

      if (index < resources.BrickCount + resources.GrainCount + resources.LumberCount + resources.OreCount)
        return ResourceTypes.Ore;

      return ResourceTypes.Wool;
    }

    private void GetResourceCardImages(ResourceTypes resourceType, out string imagePath, out string selectedImagePath)
    {
      switch (resourceType)
      {
        case ResourceTypes.Brick:
          {
            imagePath = @"..\resources\resourcecards\brickcard.png";
            selectedImagePath = @"..\resources\resourcecards\selected_brickcard.png";
            break;
          }
        case ResourceTypes.Grain:
          {
            imagePath = @"..\resources\resourcecards\graincard.png";
            selectedImagePath = @"..\resources\resourcecards\selected_graincard.png";
            break;
          }
        case ResourceTypes.Lumber:
          {
            imagePath = @"..\resources\resourcecards\lumbercard.png";
            selectedImagePath = @"..\resources\resourcecards\selected_lumbercard.png";
            break;
          }
        case ResourceTypes.Ore:
          {
            imagePath = @"..\resources\resourcecards\orecard.png";
            selectedImagePath = @"..\resources\resourcecards\selected_orecard.png";
            break;
          }
        default:
          {
            imagePath = @"..\resources\resourcecards\woolcard.png";
            selectedImagePath = @"..\resources\resourcecards\selected_woolcard.png";
            break;
          }
      }
    }

    private string GetDiceImage(uint diceRoll)
    {
      const string OneImagePath = @"..\resources\dice\one.png";
      const string TwoImagePath = @"..\resources\dice\two.png";
      const string ThreeImagePath = @"..\resources\dice\three.png";
      const string FourImagePath = @"..\resources\dice\four.png";
      const string FiveImagePath = @"..\resources\dice\five.png";
      const string SixImagePath = @"..\resources\dice\six.png";

      switch (diceRoll)
      {
        case 1: return OneImagePath;
        case 2: return TwoImagePath;
        case 3: return ThreeImagePath;
        case 4: return FourImagePath;
        case 5: return FiveImagePath;
        case 6: return SixImagePath;
      }

      throw new NotImplementedException("Should not get here");
    }

    private void Initialise(IGameBoard board)
    {
      this.board = board;

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.InitialiseBoardLayer();

        this.InitialiseSettlementSelectionLayer();

        this.InitialiseRoadSelectionLayer();
      });
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
      uint hexIndex = 0;

      foreach (var hexLayout in hexLayoutData)
      {
        var count = hexLayout.Count;
        var x = hexLayout.X;
        var y = hexLayout.Y;

        while (count-- > 0)
        {
          var hexDetails = hexData[hexIndex];

          this.GetBitmaps(hexDetails, resourceBitmaps, numberBitmaps, out resourceBitmap, out numberBitmap);

          if (hexDetails.Item1 == null)
          {
            // Initial starting location for the robber is the Desert hex
            var robberBitmap = new BitmapImage(new Uri(@"resources\robber.png", UriKind.Relative));
            this.robberImage = this.CreateImage(robberBitmap);
            this.RobberLayer.Children.Add(this.robberImage);
            Canvas.SetLeft(this.robberImage, x + 2);
            Canvas.SetTop(this.robberImage, y);

            var selectedRobberLocationBitmap = new BitmapImage(new Uri(@"resources\robber_selection.png", UriKind.Relative));
            this.selectedRobberLocationImage = this.CreateImage(selectedRobberLocationBitmap);
            this.selectedRobberLocationImage.MouseLeftButtonUp += this.Image_MouseLeftButtonUp;
            this.RobberSelectionLayer.Children.Add(this.selectedRobberLocationImage);
          }
          
          this.PlaceHex(hexIndex, resourceBitmap, numberBitmap, x, y);
          y += cellHeight;
          hexIndex++;
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
          var control = this.PlaceSettlementButtonControl(x, y, location, location.ToString());
          this.settlementButtonControls[location++] = control;
          y += dy;

          x += direction * dx;
          direction = direction == -1 ? 1 : -1;
        }
      }
    }

    private void InitialiseRoadSelectionLayer()
    {
      string roadLeftIndicatorImagePath = @"..\resources\roads\road_left_indicator.png";
      string roadLeftImagePath = @"..\resources\roads\road_left.png";
      string roadRightIndicatorImagePath = @"..\resources\roads\road_right_indicator.png";
      string roadRightImagePath = @"..\resources\roads\road_right.png";
      string roadHorizontalIndicatorImagePath = @"..\resources\roads\road_horizontal_indicator.png";
      this.roadButtonControls = new Dictionary<string, RoadButtonControl>();

      var verticalRoadLayoutData = this.GetVerticalRoadLayoutData();

      string indicatorImagePath = null;
      string imagePath = null;
      RoadButtonControl.RoadImageTypes roadImageType;
      foreach (var verticalRoadLayout in verticalRoadLayoutData)
      {
        var useRightImage = verticalRoadLayout.StartWithRightImage;
        for (var index = 0; index < verticalRoadLayout.Locations.Length; index++)
        {
          if (useRightImage)
          {
            indicatorImagePath = roadRightIndicatorImagePath;
            imagePath = roadRightImagePath;
            roadImageType = RoadButtonControl.RoadImageTypes.Right;
          }
          else
          {
            indicatorImagePath = roadLeftIndicatorImagePath;
            imagePath = roadLeftImagePath;
            roadImageType = RoadButtonControl.RoadImageTypes.Left;
          }

          var locationA = verticalRoadLayout.Locations[index];
          var locationB = locationA - 1;
          var control = this.PlaceRoadButtonControl(locationA, locationB, verticalRoadLayout.XCoordinate, verticalRoadLayout.YCoordinates[index], indicatorImagePath, roadImageType);
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
          var control = this.PlaceRoadButtonControl(locationA, locationB, horizontalRoadLayout.XCoordinate, horizontalRoadLayout.YCoordinates[index], roadHorizontalIndicatorImagePath, RoadButtonControl.RoadImageTypes.Horizontal);

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

    private Image CreateImage(BitmapImage bitmapImage)
    {
      return new Image
      {
        Width = bitmapImage.Width * 2,
        Height = bitmapImage.Height * 2,
        //Name = name,
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

    private void PlaceHex(uint hexIndex, BitmapImage resourceBitmap, BitmapImage numberBitmap, int x, int y)
    {
      var resourceImage = this.CreateImage(resourceBitmap);
      this.BoardLayer.Children.Add(resourceImage);
      Canvas.SetLeft(resourceImage, x);
      Canvas.SetTop(resourceImage, y);

      if (numberBitmap == null)
      {
        this.locationsByImage.Add(resourceImage, new Tuple<uint, Point>(hexIndex, new Point(x, y)));
        resourceImage.MouseEnter += this.Image_MouseEnter;
        return;
      }

      var numberImage = this.CreateImage(numberBitmap);
      numberImage.MouseEnter += this.Image_MouseEnter;
      this.BoardLayer.Children.Add(numberImage);
      Canvas.SetLeft(numberImage, x);
      Canvas.SetTop(numberImage, y);

      this.locationsByImage.Add(numberImage, new Tuple<uint, Point>(hexIndex, new Point(x, y)));
    }

    private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.state != States.RobberLocationSelection)
      {
        return;
      }

      var location = this.locationsByImage[this.currentRobberLocationHoverImage];
      Canvas.SetLeft(this.robberImage, location.Item2.X);
      Canvas.SetTop(this.robberImage, location.Item2.Y);
      this.RobberSelectionLayer.Visibility = Visibility.Hidden;
      //this.state = States.RobbedPlayerSelection;
      //this.PlayerSelectionLayer.Visibility = Visibility.Visible;
      this.controllerViewModel.SetRobberLocation(location.Item1);
    }

    private void Image_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.state != States.RobberLocationSelection || sender == this.currentRobberLocationHoverImage)
      {
        return;
      }

      this.currentRobberLocationHoverImage = (Image)sender;
      this.RobberSelectionLayer.Visibility = Visibility.Visible;
      var location = this.locationsByImage[this.currentRobberLocationHoverImage];
      Canvas.SetLeft(this.selectedRobberLocationImage, location.Item2.X);
      Canvas.SetTop(this.selectedRobberLocationImage, location.Item2.Y);
    }

    private void PlaceRoadControl(double x, double y, string imagePath)
    {
      var control = new RoadControl(imagePath);
      this.RoadLayer.Children.Add(control);
      Canvas.SetLeft(control, x);
      Canvas.SetTop(control, y);
    }

    private RoadButtonControl PlaceRoadButtonControl(uint start, uint end, double x, double y, string imagePath, RoadButtonControl.RoadImageTypes roadImageType)
    {
      var control = new RoadButtonControl(start, end, x, y, imagePath, roadImageType, this.RoadSelectedEventHandler);
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
      if (!string.IsNullOrEmpty(toolTip))
      {
        control.ToolTip = toolTip;
      }

      canvas.Children.Add(control);
      Canvas.SetLeft(control, x);
      Canvas.SetTop(control, y);
    }

    private void ResourceSelectedEventHandler(ResourceButton resourceButton)
    {
      this.workingNumberOfResourcesToSelect -= resourceButton.IsSelected ? 1 : -1;
      this.ResourceSelectionMessage = $"Select {this.workingNumberOfResourcesToSelect} more resources to drop";
      this.ResourceSelectionConfirmButton.IsEnabled = this.workingNumberOfResourcesToSelect == 0;
    }

    private void ResourceSelectionConfirmButton_Click(object sender, RoutedEventArgs e)
    {
      var brickCount = 0;
      var grainCount = 0;
      var lumberCount = 0;
      var oreCount = 0;
      var woolCount = 0;

      foreach (var resourceButton in this.resourceControls)
      {
        if (resourceButton.IsSelected)
        {
          switch (resourceButton.ResourceType)
          {
            case ResourceTypes.Brick: brickCount++; break;
            case ResourceTypes.Grain: grainCount++; break;
            case ResourceTypes.Lumber: lumberCount++; break;
            case ResourceTypes.Ore: oreCount++; break;
            case ResourceTypes.Wool: woolCount++; break;
          }
        }
      }

      this.ResourceSelectionLayer.Visibility = Visibility.Hidden;
      this.controllerViewModel.DropResourcesFromPlayer(new ResourceClutch(brickCount, grainCount, lumberCount, oreCount, woolCount));
      this.state = States.RobberLocationSelection;
    }

    private void RoadSelectedEventHandler(RoadButtonControl roadButtonControl)
    {
      this.workingRoadControl = roadButtonControl;
      this.workingRoadControl.Visibility = Visibility.Hidden;

      var roadImagePath = this.roadImagesByPlayerId[this.playerId][(int)this.workingRoadControl.RoadImageType];

      this.PlaceBuildingControl(roadButtonControl.X, roadButtonControl.Y, string.Empty, roadImagePath, this.RoadLayer);

      foreach (var visibleRoadButtonControl in this.visibleRoadButtonControls)
      {
        visibleRoadButtonControl.Visibility = Visibility.Hidden;
      }

      this.visibleRoadButtonControls.Clear();

      this.RoadSelectionLayer.Visibility = Visibility.Hidden;

      this.EndTurnButton.Visibility = Visibility.Visible;
      this.TopLayer.Visibility = Visibility.Visible;
    }

    private void RobbingChoicesEventHandler(Dictionary<Guid, int> choices)
    {
      var width = 60;
      var gutter = 10;
      var midX = 400;
      var midY = 200;
      var x = midX - ((choices.Count * width) + ((choices.Count - 1) * gutter) / 2);

      this.PlayerSelectionLayer.Visibility = Visibility.Visible;
    }

    private void SettlementSelectedEventHandler(SettlementButtonControl settlementButtonControl)
    {
      this.workingLocation = settlementButtonControl.Location;

      // Turn off the controls for the location and its neighbours
      settlementButtonControl.Visibility = Visibility.Hidden;
      var neighbouringLocations = this.board.BoardQuery.GetNeighbouringLocationsFrom(this.workingLocation);
      foreach (var index in neighbouringLocations)
      {
        this.settlementButtonControls[index].Visibility = Visibility.Hidden;
      }

      this.PlaceBuildingControl(settlementButtonControl.X, settlementButtonControl.Y, string.Empty, @"..\resources\settlements\blue_settlement.png", this.SettlementLayer);
      
      // Turn on the possible road controls for the location
      var roadEndLocations = this.board.BoardQuery.GetValidConnectedLocationsFrom(this.workingLocation);
      for (var index = 0; index < roadEndLocations.Length; index++)
      {
        var id = string.Format("{0}-{1}", this.workingLocation, roadEndLocations[index]);
        var roadButtonControl = this.roadButtonControls[id];
        roadButtonControl.Visibility = Visibility.Visible;
        this.visibleRoadButtonControls.Add(roadButtonControl);
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

      Task.Factory.StartNew(() => {
        this.controllerViewModel.StartGame();
      });
    }

    private void EndTurnButton_Click(object sender, RoutedEventArgs e)
    {
      this.EndTurnButton.Visibility = Visibility.Hidden;
      if (this.state == States.AwaitingFirstInfrastructure)
      {
        this.state = States.AwaitingSecondInfrastructure;
        var roadEndLocation = this.workingRoadControl.Start == this.workingLocation ? this.workingRoadControl.End : this.workingRoadControl.Start;
        this.controllerViewModel.CompleteFirstInfrastructureSetup(this.workingLocation, roadEndLocation);
      }
      else if (this.state == States.AwaitingSecondInfrastructure)
      {
        var roadEndLocation = this.workingRoadControl.Start == this.workingLocation ? this.workingRoadControl.End : this.workingRoadControl.Start;
        this.controllerViewModel.CompleteSecondInfrastructureSetup(this.workingLocation, roadEndLocation);
      }
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
