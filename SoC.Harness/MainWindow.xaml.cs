
namespace SoC.Harness
{
    using System;
    using System.Windows;
    using System.Windows.Forms;
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
        }

        private void ErrorRaisedEventHandler(ErrorDetails obj)
        {
            throw new NotImplementedException();
        }

        private void GameJoinedEventHandler(PlayerViewModel topLeftPlayerViewModel, PlayerViewModel bottomLeftPlayerViewModel, PlayerViewModel topRightPlayerViewModel, PlayerViewModel bottomRightPlayerViewModel)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                this.TopLeftPlayer.DataContext = topLeftPlayerViewModel;
                this.BottomLeftPlayer.DataContext = bottomLeftPlayerViewModel;
                this.TopRightPlayer.DataContext = topRightPlayerViewModel;
                this.BottomRightPlayer.DataContext = bottomRightPlayerViewModel;
            });
        }

        private void New_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.controllerViewModel = new ControllerViewModel(new LocalGameController(new TestNumberGenerator(), new PlayerPool()));
            this.controllerViewModel.GameJoinedEvent += this.GameJoinedEventHandler;

            this.PlayArea.Initialise(this.controllerViewModel);
            this.PlayArea.StartGame();
        }

        private void Open_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            var dialogResult = ofd.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.controllerViewModel = ControllerViewModel.Load(ofd.FileName);
                this.controllerViewModel.GameJoinedEvent += this.GameJoinedEventHandler;
                this.PlayArea.Initialise(this.controllerViewModel);
            }
        }

        private void Save_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }
    }
}
