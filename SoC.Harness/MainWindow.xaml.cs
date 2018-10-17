
namespace SoC.Harness
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.SoC.Library;
  using SoC.Harness.ViewModels;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private LocalGameController localGameController;
    private TurnToken currentTurnToken;
    private Dictionary<Guid, PlayerViewModel> playerViewModelsById = new Dictionary<Guid, PlayerViewModel>();
    private ControllerViewModel controllerViewModel;

    public MainWindow()
    {
      this.controllerViewModel = new ControllerViewModel(new LocalGameController(new NumberGenerator(), new PlayerPool()));
      this.controllerViewModel.GameJoinedEvent += this.GameJoinedEventHandler;
      this.controllerViewModel.GameJoinedEvent += this.PlayArea.InitialisePlayerViews;
      this.controllerViewModel.PlayerUpdateEvent += this.PlayerUpdateEventHandler;
      this.controllerViewModel.InitialBoardSetupEvent += this.PlayArea.Initialise;
      this.controllerViewModel.BoardUpdatedEvent += this.PlayArea.BoardUpdatedEventHandler;
      this.controllerViewModel.DiceRollEvent += this.PlayArea.DiceRollEventHandler;

      this.InitializeComponent();

      this.PlayArea.EndTurnEvent = this.EndTurnEventHandler;
      this.PlayArea.StartGameEvent = this.StartGameEventHandler;
    }

    private void PlayerUpdateEventHandler(PlayerViewModel arg1, PlayerViewModel arg2, PlayerViewModel arg3, PlayerViewModel arg4)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.TopLeftPlayer.DataContext = arg1;
        this.BottomLeftPlayer.DataContext = arg2;
        this.TopRightPlayer.DataContext = arg3;
        this.BottomRightPlayer.DataContext = arg4;
        //this.PlayArea.InitialisePlayerData(new[] { arg1, arg2, arg3, arg4 });
      });
    }

    private void ErrorRaisedEventHandler(ErrorDetails obj)
    {
      throw new NotImplementedException();
    }

    private void StartGameEventHandler()
    {
      Task.Factory.StartNew(() => {
        /*this.localGameController.JoinGame();
        this.localGameController.LaunchGame();
        this.localGameController.StartGameSetup();*/
        this.controllerViewModel.StartGame();
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

    private void GameJoinedEventHandler(PlayerViewModel topLeftPlayerViewModel, PlayerViewModel bottomLeftPlayerViewModel, PlayerViewModel topRightPlayerViewModel, PlayerViewModel bottomRightPlayerViewModel)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.TopLeftPlayer.DataContext = topLeftPlayerViewModel;
        this.BottomLeftPlayer.DataContext = bottomLeftPlayerViewModel;
        this.TopRightPlayer.DataContext = topRightPlayerViewModel;
        this.BottomRightPlayer.DataContext = bottomRightPlayerViewModel;
      });
    }
  }
}
