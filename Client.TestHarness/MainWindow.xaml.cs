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
    private GameClient gameClient;

    private BoardTranslator boardTranslator;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void ConnectButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.gameClient == null)
      {
        this.ConnectButton.Content = "Connecting...";
        this.ConnectButton.IsEnabled = false;
         
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

        this.gameClient.GameInitializationEvent = () =>
        {
          Application.Current.Dispatcher.Invoke(() =>
          {
            this.DisplayArea.NavigateToString(this.gameClient.Document);
          });
        };

        this.boardTranslator = new BoardTranslator(this.gameClient.Board);

        Task.Factory.StartNew(() =>
        {
          this.gameClient.Connect();
        });
      }
      else
      {
        this.ConnectButton.Content = "Disconnecting...";
        this.ConnectButton.IsEnabled = false;

        this.gameClient.GameLeftEvent = () =>
        {
          this.gameClient = null;
          Application.Current.Dispatcher.Invoke(() =>
          {
            this.GameIdLabel.Content = "(unconnected)";
            this.ConnectButton.IsEnabled = true;
            this.ConnectButton.Content = "Connect";
          });
        };

        Task.Factory.StartNew(() => 
        {
          this.gameClient.Disconnect();
        });
      }
    }

    private void DisplayAreaMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
    {
      
    }

    private void DisplayAreaMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
    {
      
    }
  }
}
