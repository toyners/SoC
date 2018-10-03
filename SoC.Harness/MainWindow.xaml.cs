
namespace SoC.Harness
{
  using System;
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

      this.PlayArea.EndTurnEvent = this.EndTurnEventHandler;

      this.PlayArea.Initialise(board);
    }

    private void EndTurnEventHandler(int message, object data)
    {
      switch (message)
      {
        case 1: {
            var tuple = (Tuple<uint, uint>)data;
            this.localGameController.ContinueGameSetup(tuple.Item1, tuple.Item2);
            break;
          }
      }
    }
  }
}
