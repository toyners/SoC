
namespace SoC.Harness
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.SoC.Library;
  using SoC.Harness.ViewModels;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ControllerViewModel controllerViewModel;

    public MainWindow()
    {
      this.InitializeComponent();

      this.controllerViewModel = new ControllerViewModel(new LocalGameController(new TestNumberGenerator(), new PlayerPool()));
      this.controllerViewModel.GameJoinedEvent += this.GameJoinedEventHandler;
      this.controllerViewModel.GameJoinedEvent += this.PlayArea.InitialisePlayerViews;
      this.controllerViewModel.InitialBoardSetupEvent += this.PlayArea.Initialise;
      this.controllerViewModel.BoardUpdatedEvent += this.PlayArea.BoardUpdatedEventHandler;
      this.controllerViewModel.DiceRollEvent += this.PlayArea.DiceRollEventHandler;
      this.controllerViewModel.RobberEvent += this.PlayArea.RobberEventHandler;

      this.PlayArea.EndTurnEvent = this.controllerViewModel.EndTurnEventHandler;
      this.PlayArea.StartGameEvent = this.StartGameEventHandler;
      this.PlayArea.ResourcesSelectedEvent = this.controllerViewModel.ResourceSelectedEventHandler;
    }

    private void ErrorRaisedEventHandler(ErrorDetails obj)
    {
      throw new NotImplementedException();
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

    private void StartGameEventHandler()
    {
      Task.Factory.StartNew(() => {
        this.controllerViewModel.StartGame();
      });
    }
  }
}
