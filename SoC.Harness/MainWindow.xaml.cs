
namespace SoC.Harness
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Forms;
    using Jabberwocky.SoC.Library;
    using SoC.Harness.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool canSave;
        private ControllerViewModel controllerViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public bool CanSave
        {
            get { return this.canSave; }
            set
            {
                this.canSave = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("CanSave"));
            }
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
            this.CanSave = true;
            this.controllerViewModel = new ControllerViewModel(new LocalGameController(new TestNumberGenerator(), new PlayerPool()));
            this.controllerViewModel.GameJoinedEvent += this.GameJoinedEventHandler;

            this.PlayArea.Initialise(this.controllerViewModel);
            this.PlayArea.StartGame();
        }

        private void Open_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Title = "Select game file to open";
            ofd.Filter = "Game save files (*.soc)|*.soc";
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
            var saveFilePath = $"Game_{DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")}.soc";
            this.controllerViewModel.Save(saveFilePath);
            System.Windows.MessageBox.Show("Game Saved.", "Save Status");
        }

        private void CommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CanSave;
        }
    }
}
