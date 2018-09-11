
namespace SoC.Harness
{
  using System.Windows;
  using Jabberwocky.SoC.Library;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    LocalGameController localGameController;

    public MainWindow()
    {
      this.InitializeComponent();
      
      //this.Board.Initialise()
    }
  }
}
