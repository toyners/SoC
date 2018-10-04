
namespace SoC.Harness
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;
  using Jabberwocky.SoC.Library.Interfaces;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    LocalGameController localGameController;

    public MainWindow()
    {
      this.InitializeComponent();

      this.PlayArea.EndTurnEvent = this.EndTurnEventHandler;
      this.PlayArea.StartGameEvent = this.StartGameEventHandler;

      this.localGameController = new LocalGameController(new TestDice());
      this.localGameController.GameJoinedEvent = this.GameJoinedEventHandler;
      this.localGameController.InitialBoardSetupEvent = this.InitialBoardSetupEventHandler;
    }

    private void StartGameEventHandler()
    {
      this.localGameController.JoinGame();
      this.localGameController.LaunchGame();
      this.localGameController.StartGameSetup();
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

    private void GameJoinedEventHandler(PlayerDataView[] playerDataViews)
    {

    }

    private void InitialBoardSetupEventHandler(GameBoard board)
    {
      this.PlayArea.Initialise(board);
    }
  }

  public class TestDice : INumberGenerator
  {
    private Dice dice;
    private Queue<uint> diceRolls; 
    public TestDice()
    {
      this.diceRolls = new Queue<uint>(new uint[] { 12, 6, 4, 3 });
      this.dice = new Dice();
    }

    public int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum)
    {
      return this.dice.GetRandomNumberBetweenZeroAndMaximum(exclusiveMaximum);
    }

    public uint RollTwoDice()
    {
      if (this.diceRolls.Count > 0)
      {
        return this.diceRolls.Dequeue();
      }

      return this.dice.RollTwoDice();
    }
  }
}
