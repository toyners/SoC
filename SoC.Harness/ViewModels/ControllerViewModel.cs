
namespace SoC.Harness.ViewModels
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;

  public class ControllerViewModel
  {
    private readonly LocalGameController localGameController;
    private TurnToken currentTurnToken;
    private PlayerViewModel player;
    private Dictionary<Guid, PlayerViewModel> playerViewModelsById = new Dictionary<Guid, PlayerViewModel>();

    public ControllerViewModel(LocalGameController localGameController)
    {
      this.localGameController = localGameController;
      this.localGameController.GameJoinedEvent = this.GameJoinedEventHandler;
      this.localGameController.GameSetupUpdateEvent = this.GameSetupUpdateEventHandler;
      this.localGameController.BoardUpdatedEvent = this.BoardUpdatedEvent;
      this.localGameController.StartPlayerTurnEvent = this.StartPlayerTurnEventHandler;
      this.localGameController.GameSetupResourcesEvent = this.GameSetupResourcesEventHandler;
      this.localGameController.InitialBoardSetupEvent = this.InitialBoardSetupEventHandler;
      this.localGameController.DiceRollEvent = this.DiceRollEventHandler;
      this.localGameController.ResourcesCollectedEvent = this.ResourcesCollectedEventHandler;
      this.localGameController.RobberEvent = this.RobberEventHandler;
    }

    public void ResourceSelectedEventHandler(int arg1, int arg2, int arg3, int arg4, int arg5)
    {
      throw new NotImplementedException();
    }

    public event Action<PlayerViewModel, PlayerViewModel, PlayerViewModel, PlayerViewModel> GameJoinedEvent;
    public event Action<IGameBoard> InitialBoardSetupEvent;
    public event Action<GameBoardUpdate> BoardUpdatedEvent;
    public event Action<uint, uint> DiceRollEvent;
    public event Action<PlayerViewModel, int> RobberEvent;

    public void StartGame()
    {
      this.localGameController.JoinGame();
      this.localGameController.LaunchGame();
      this.localGameController.StartGameSetup();
    }

    public void EndTurnEventHandler(EventTypes eventType, object data)
    {
      switch (eventType)
      {
        case EventTypes.EndFirstSetupTurn:
          {
            var tuple = (Tuple<uint, uint>)data;
            this.localGameController.ContinueGameSetup(tuple.Item1, tuple.Item2);
            break;
          }
        case EventTypes.EndSecondSetupTurn:
          {
            var tuple = (Tuple<uint, uint>)data;
            this.localGameController.CompleteGameSetup(tuple.Item1, tuple.Item2);
            this.localGameController.FinalisePlayerTurnOrder();
            this.localGameController.StartGamePlay();
            break;
          }
        default: throw new NotImplementedException();
      }
    }

    private void DiceRollEventHandler(uint arg1, uint arg2)
    {
      this.DiceRollEvent?.Invoke(arg1, arg2);
    }

    private void GameJoinedEventHandler(PlayerDataModel[] playerDataModels)
    {
      string firstPlayerIconPath = @"..\resources\icons\blue_icon.png";
      string secondPlayerIconPath = @"..\resources\icons\red_icon.png";
      string thirdPlayerIconPath = @"..\resources\icons\green_icon.png";
      string fourthPlayerIconPath = @"..\resources\icons\yellow_icon.png";

      var playerViewModel1 = new PlayerViewModel(playerDataModels[0], firstPlayerIconPath);
      this.player = playerViewModel1;
      this.playerViewModelsById.Add(playerDataModels[0].Id, playerViewModel1);

      var playerViewModel2 = new PlayerViewModel(playerDataModels[1], secondPlayerIconPath);
      this.playerViewModelsById.Add(playerDataModels[1].Id, playerViewModel2);

      var playerViewModel3 = new PlayerViewModel(playerDataModels[2], thirdPlayerIconPath);
      this.playerViewModelsById.Add(playerDataModels[2].Id, playerViewModel3);

      var playerViewModel4 = new PlayerViewModel(playerDataModels[3], fourthPlayerIconPath);
      this.playerViewModelsById.Add(playerDataModels[3].Id, playerViewModel4);

      this.GameJoinedEvent?.Invoke(playerViewModel1, playerViewModel2, playerViewModel3, playerViewModel4);
    }

    private void GameSetupResourcesEventHandler(ResourceUpdate resourceUpdate)
    {
      foreach (var resourceData in resourceUpdate.Resources)
      {
        this.playerViewModelsById[resourceData.Key].Update(resourceData.Value);
      }
    }

    private void GameSetupUpdateEventHandler(GameBoardUpdate boardUpdate)
    {
      if (boardUpdate == null)
      {
        return;
      }

      foreach (var settlementDetails in boardUpdate.NewSettlements)
      {
        var location = settlementDetails.Item1;
        var playerId = settlementDetails.Item2;

        var playerViewModel = this.playerViewModelsById[playerId];
        var line = "Built settlement at " + location;
        playerViewModel.UpdateHistory(line);
      }

      foreach (var roadDetails in boardUpdate.NewRoads)
      {
        var startLocation = roadDetails.Item1;
        var endLocation = roadDetails.Item2;
        var playerId = roadDetails.Item3;

        var playerViewModel = this.playerViewModelsById[playerId];
        var line = "Built road from " + startLocation + " to " + endLocation;
        playerViewModel.UpdateHistory(line);
      }

      this.BoardUpdatedEvent?.Invoke(boardUpdate);
    }

    private void InitialBoardSetupEventHandler(GameBoard gameBoard)
    {
      this.InitialBoardSetupEvent?.Invoke(gameBoard);
    }

    private void ResourcesCollectedEventHandler(Dictionary<Guid, ResourceCollection[]> resources)
    {
      foreach (var entry in resources)
      {
        foreach (var rc in entry.Value)
        {
          this.playerViewModelsById[entry.Key].Update(rc.Resources);
        }
      }
    }

    private void RobberEventHandler(int numberOfResourcesToSelect)
    {
      numberOfResourcesToSelect = 1;
      this.RobberEvent?.Invoke(this.player, numberOfResourcesToSelect);
    }

    private void StartPlayerTurnEventHandler(TurnToken turnToken)
    {
      this.currentTurnToken = turnToken;
    }
  }
}
