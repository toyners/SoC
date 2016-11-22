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
using Jabberwocky.SoC.Client.ServiceReference;

namespace Client.TestHarness
{
  /// <summary>
  /// Interaction logic for ClientUI.xaml
  /// </summary>
  public partial class ClientUI : UserControl
  {
    #region Fields
    private BoardDisplay boardDisplay;

    private GameClient gameClient;

    private Boolean leftMouseButtonDown;
    #endregion

    #region Construction
    public ClientUI()
    {
      InitializeComponent();
    }
    #endregion

    #region Methods
    private void BoardDisplay_Click(Int32 id)
    {
      throw new NotImplementedException();
    }

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
      this.leftMouseButtonDown = true;
    }

    private void DisplayAreaMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
    {
      if (this.leftMouseButtonDown)
      {
        var mousePoint = e.GetPosition(this.ControlArea);
        this.leftMouseButtonDown = false;
      }
    }

    private void GameInitializationEventHandler(GameInitializationData gameData)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.boardDisplay = new BoardDisplay(this.gameClient.Board, this.DisplayArea, this.ControlArea);
        this.boardDisplay.ButtonClickEventHandler += BoardDisplay_Click;
        this.boardDisplay.LayoutBoard(gameData);
        this.boardDisplay.OverlayTownPlacement();
      });
    }

    private void GameJoinedEventHandler()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.GameIdLabel.Content = this.gameClient.GameToken.ToString();
        this.ConnectButton.IsEnabled = true;
        this.ConnectButton.Content = "Disconnect";
      });
    }

    private void GameLeftEventHandler()
    {
      this.gameClient = null;
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.boardDisplay.Clear();
        this.GameIdLabel.Content = "<unconnected>";
        this.ConnectButton.IsEnabled = true;
        this.ConnectButton.Content = "Connect";
      });
    }

    private void InitializeGameClient()
    {
      this.gameClient = new GameClient();
      this.gameClient.GameJoinedEvent += this.GameJoinedEventHandler;
      this.gameClient.GameInitializationEvent += this.GameInitializationEventHandler;
      this.gameClient.GameLeftEvent += this.GameLeftEventHandler;
    }

    private void TurnButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
