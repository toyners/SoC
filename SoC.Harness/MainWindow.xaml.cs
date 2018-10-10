
namespace SoC.Harness
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Xml;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;
  using Jabberwocky.SoC.Library.Interfaces;
  using Jabberwocky.SoC.Library.Storage;
  using SoC.Harness.ViewModels;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private LocalGameController localGameController;
    private TurnToken currentTurnToken;

    public MainWindow()
    {
      this.InitializeComponent();

      this.PlayArea.EndTurnEvent = this.EndTurnEventHandler;
      this.PlayArea.StartGameEvent = this.StartGameEventHandler;

      this.localGameController = new LocalGameController(new NumberGenerator(), new PlayerPool());
      this.localGameController.GameJoinedEvent = this.GameJoinedEventHandler;
      this.localGameController.InitialBoardSetupEvent = this.PlayArea.Initialise;
      this.localGameController.GameSetupUpdateEvent = this.PlayArea.BoardUpdatedEventHandler;
      this.localGameController.BoardUpdatedEvent = this.PlayArea.BoardUpdatedEventHandler;
      this.localGameController.StartPlayerTurnEvent = this.StartPlayerTurnEventHandler;
      this.localGameController.DiceRollEvent = this.PlayArea.DiceRollEventHandler;
    }

    private void StartPlayerTurnEventHandler(TurnToken turnToken)
    {
      this.currentTurnToken = turnToken;
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

    private void GameJoinedEventHandler(PlayerDataModel[] playerDataModels)
    {
      string firstPlayerIconPath = @"..\resources\icons\blue_icon.png";
      string secondPlayerIconPath = @"..\resources\icons\red_icon.png";
      string thirdPlayerIconPath = @"..\resources\icons\green_icon.png";
      string fourthPlayerIconPath = @"..\resources\icons\yellow_icon.png";
      this.TopLeftPlayer.DataContext = new PlayerViewModel(playerDataModels[0], firstPlayerIconPath);
      this.BottomLeftPlayer.DataContext = new PlayerViewModel(playerDataModels[1], secondPlayerIconPath);
      this.TopRightPlayer.DataContext = new PlayerViewModel(playerDataModels[2], thirdPlayerIconPath);
      this.BottomRightPlayer.DataContext = new PlayerViewModel(playerDataModels[3], fourthPlayerIconPath);
      this.PlayArea.InitialisePlayerData(playerDataModels);
    }
  }
}
