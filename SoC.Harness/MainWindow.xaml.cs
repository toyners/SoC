
namespace SoC.Harness
{
    using System;
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

            this.PlayArea.Initialise(this.controllerViewModel);
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
    }
}
