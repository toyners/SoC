
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
    private Dictionary<Guid, PlayerViewModel> playerViewModelsById = new Dictionary<Guid, PlayerViewModel>();

    public ControllerViewModel(LocalGameController localGameController)
    {
      this.localGameController = localGameController;
    }

    public event Action<PlayerViewModel, PlayerViewModel, PlayerViewModel, PlayerViewModel> GameJoinedEvent;
    public event Action<PlayerViewModel, PlayerViewModel, PlayerViewModel, PlayerViewModel> PlayerUpdateEvent;
    public event Action<IGameBoard> InitialBoardSetupEvent;

    public void StartGame()
    {
      this.localGameController.JoinGame();
      this.localGameController.LaunchGame();
      this.localGameController.StartGameSetup();
    }
  }
}
