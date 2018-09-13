
namespace SoC.Harness
{
  using System.Windows;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    LocalGameController localGameController;

    public MainWindow()
    {
      this.InitializeComponent();

      // TODO: Investigate IoC containers for initialisation
      var numberGenerator = new Dice();
      var playerPool = new PlayerPool();
      var board = new GameBoard(BoardSizes.Standard);
      var developmentCardHolder = new DevelopmentCardHolder();

      this.localGameController = new LocalGameController(numberGenerator, playerPool, board, developmentCardHolder);

      this.PlayArea.Initialise(board);
    }
  }
}
