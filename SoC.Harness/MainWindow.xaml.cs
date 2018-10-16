
namespace SoC.Harness
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Xml;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;
  using Jabberwocky.SoC.Library.Interfaces;
  using Jabberwocky.SoC.Library.Storage;
  using SoC.Harness.ViewModels;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private LocalGameController localGameController;
    private TurnToken currentTurnToken;
    private Dictionary<Guid, PlayerViewModel> playerViewModelsById = new Dictionary<Guid, PlayerViewModel>();

    public MainWindow()
    {
      this.InitializeComponent();

      this.PlayArea.EndTurnEvent = this.EndTurnEventHandler;
      this.PlayArea.StartGameEvent = this.StartGameEventHandler;

      this.localGameController = new LocalGameController(new NumberGenerator(), new PlayerPool());
      this.localGameController.ErrorRaisedEvent = this.ErrorRaisedEventHandler;
      this.localGameController.GameJoinedEvent = this.GameJoinedEventHandler;
      this.localGameController.InitialBoardSetupEvent = this.PlayArea.Initialise;
      this.localGameController.GameSetupUpdateEvent = this.GameSetupUpdateEventHandler;
      this.localGameController.BoardUpdatedEvent = this.PlayArea.BoardUpdatedEventHandler;
      this.localGameController.StartPlayerTurnEvent = this.StartPlayerTurnEventHandler;
      this.localGameController.DiceRollEvent = this.PlayArea.DiceRollEventHandler;
      this.localGameController.GameSetupResourcesEvent = this.GameSetupResourcesEventHandler;
      this.localGameController.TurnOrderFinalisedEvent = this.TurnOrderFinalisedEventHandler;
      this.localGameController.ResourcesCollectedEvent = this.ResourcesCollectedEventHandler;
    }

    private void ResourcesCollectedEventHandler(Dictionary<Guid, ResourceCollection[]> obj)
    {
      throw new NotImplementedException();
    }

    private void GameSetupUpdateEventHandler(GameBoardUpdate boardUpdate)
    {
      if (boardUpdate == null)
      {
        return;
      }

      foreach (var settlementDetails in boardUpdate.NewSettlements)
      {
        var location = settlementDetails.Item1;
        var playerId = settlementDetails.Item2;

        var playerViewModel = this.playerViewModelsById[playerId];
        var line = "Built settlement at " + location;
        playerViewModel.UpdateHistory(line);
      }

      foreach (var roadDetails in boardUpdate.NewRoads)
      {
        var startLocation = roadDetails.Item1;
        var endLocation = roadDetails.Item2;
        var playerId = roadDetails.Item3;

        var playerViewModel = this.playerViewModelsById[playerId];
        var line = "Built road from " + startLocation + " to " + endLocation;
        playerViewModel.UpdateHistory(line);
      }

      this.PlayArea.BoardUpdatedEventHandler(boardUpdate);
    }

    private void ErrorRaisedEventHandler(ErrorDetails obj)
    {
      throw new NotImplementedException();
    }

    private void TurnOrderFinalisedEventHandler(PlayerDataModel[] obj)
    {
      //throw new NotImplementedException();
    }

    private void GameSetupResourcesEventHandler(ResourceUpdate resourceUpdate)
    {
      foreach (var resourceData in resourceUpdate.Resources)
      {
        this.playerViewModelsById[resourceData.Key].Update(resourceData.Value);
      }
    }

    private void StartPlayerTurnEventHandler(TurnToken turnToken)
    {
      this.currentTurnToken = turnToken;
    }

    private void StartGameEventHandler()
    {
      Task.Factory.StartNew(() => {
        this.localGameController.JoinGame();
        this.localGameController.LaunchGame();
        this.localGameController.StartGameSetup();
      });
    }

    private void EndTurnEventHandler(EventTypes eventType, object data)
    {
      switch (eventType)
      {
        case EventTypes.EndFirstSetupTurn:
        {
          var tuple = (Tuple<uint, uint>)data;
          this.localGameController.ContinueGameSetup(tuple.Item1, tuple.Item2);
          break;
        }
        case EventTypes.EndSecondSetupTurn:
        {
          var tuple = (Tuple<uint, uint>)data;
          this.localGameController.CompleteGameSetup(tuple.Item1, tuple.Item2);
          this.localGameController.FinalisePlayerTurnOrder();
          this.localGameController.StartGamePlay();
          break;
        }
        default: throw new NotImplementedException();
      }
    }

    private void GameJoinedEventHandler(PlayerDataModel[] playerDataModels)
    {
      string firstPlayerIconPath = @"..\resources\icons\blue_icon.png";
      string secondPlayerIconPath = @"..\resources\icons\red_icon.png";
      string thirdPlayerIconPath = @"..\resources\icons\green_icon.png";
      string fourthPlayerIconPath = @"..\resources\icons\yellow_icon.png";

      var topLeftPlayerViewModel = new PlayerViewModel(playerDataModels[0], firstPlayerIconPath);
      this.playerViewModelsById.Add(playerDataModels[0].Id, topLeftPlayerViewModel);

      var bottomLeftPlayerViewModel = new PlayerViewModel(playerDataModels[1], secondPlayerIconPath);
      this.playerViewModelsById.Add(playerDataModels[1].Id, bottomLeftPlayerViewModel);

      var topRightPlayerViewModel = new PlayerViewModel(playerDataModels[2], thirdPlayerIconPath);
      this.playerViewModelsById.Add(playerDataModels[2].Id, topRightPlayerViewModel);

      var bottomRightPlayerViewModel = new PlayerViewModel(playerDataModels[3], fourthPlayerIconPath);
      this.playerViewModelsById.Add(playerDataModels[3].Id, bottomRightPlayerViewModel);

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.TopLeftPlayer.DataContext = topLeftPlayerViewModel;
        this.BottomLeftPlayer.DataContext = bottomLeftPlayerViewModel;
        this.TopRightPlayer.DataContext = topRightPlayerViewModel;
        this.BottomRightPlayer.DataContext = bottomRightPlayerViewModel;
        this.PlayArea.InitialisePlayerData(playerDataModels);
      });
    }
  }
}
