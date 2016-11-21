using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jabberwocky.SoC.Client;

namespace Client.TestHarness
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    #region Fields
    private GameClient gameClient;

    private BoardDisplay boardDisplay;

    private Boolean leftMouseButtonDown;
    #endregion

    #region Construction
    public MainWindow()
    {
      InitializeComponent();
    }
    #endregion

    #region Methods
    private void ConnectButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.gameClient == null)
      {
        this.ConnectButton.Content = "Connecting...";
        this.ConnectButton.IsEnabled = false;

        this.InitializeGameClient();

        Task.Factory.StartNew(() =>
        {
          this.gameClient.Connect();
        });
      }
      else
      {
        this.ConnectButton.Content = "Disconnecting...";
        this.ConnectButton.IsEnabled = false;

        Task.Factory.StartNew(() => 
        {
          this.gameClient.Disconnect();
        });
      }
    }

    private void DisplayAreaMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
    {
      leftMouseButtonDown = true;
    }

    private void DisplayAreaMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
    {
      if (leftMouseButtonDown)
      {
        var mousePoint = e.GetPosition(this.DisplayArea);
        leftMouseButtonDown = false;
      }  
    }

    private void InitializeGameClient()
    {
      this.gameClient = new GameClient();
      this.gameClient.GameJoinedEvent = () =>
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          this.GameIdLabel.Content = this.gameClient.GameToken.ToString();
          this.ConnectButton.IsEnabled = true;
          this.ConnectButton.Content = "Disconnect";
        });
      };

      this.gameClient.GameInitializationEvent = (gameData) =>
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          this.boardDisplay = new BoardDisplay(this.gameClient.Board, this.DisplayArea);
          this.boardDisplay.LayoutBoard(gameData);
        });
      };

      this.gameClient.GameLeftEvent = () =>
      {
        this.gameClient = null;
        Application.Current.Dispatcher.Invoke(() =>
        {
          this.boardDisplay.Clear();
          this.GameIdLabel.Content = "<unconnected>";
          this.ConnectButton.IsEnabled = true;
          this.ConnectButton.Content = "Connect";
        });
      };
    }

    private void TurnButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
