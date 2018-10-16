
namespace SoC.Harness.ViewModels
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.SoC.Library;

  public class ControllerViewModel
  {
    private readonly LocalGameController localGameController;
    private TurnToken currentTurnToken;
    private Dictionary<Guid, PlayerViewModel> playerViewModelsById = new Dictionary<Guid, PlayerViewModel>();

    public ControllerViewModel(LocalGameController localGameController)
    {
      this.localGameController = localGameController;
    }

    public Action<PlayerViewModel, PlayerViewModel, PlayerViewModel, PlayerViewModel> PlayerUpdateEvent;


  }
}
