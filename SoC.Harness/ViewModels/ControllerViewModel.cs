
namespace SoC.Harness.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;

    public class ControllerViewModel
    {
        public enum States
        {
            Start,
            StartTurn,
        }

        #region Fields
        private readonly LocalGameController localGameController;
        private TurnToken currentTurnToken;
        private PlayerViewModel player;
        private Dictionary<Guid, PlayerViewModel> playerViewModelsById = new Dictionary<Guid, PlayerViewModel>();
        private States state;
        private PlayerActions playerActions = new PlayerActions();
        #endregion

        #region Construction
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
            this.localGameController.ResourcesLostEvent = this.ResourcesLostEventHandler;
            this.localGameController.RobbingChoicesEvent = this.RobbingChoicesEventHandler;
            this.localGameController.ResourcesTransferredEvent = this.ResourcesTransferredEventHandler;
        }
        #endregion

        #region Events
        public event Action<PlayerViewModel, PlayerViewModel, PlayerViewModel, PlayerViewModel> GameJoinedEvent;
        public event Action<IGameBoard> InitialBoardSetupEvent;
        public event Action<GameBoardUpdate> BoardUpdatedEvent;
        public event Action<uint, uint> DiceRollEvent;
        public event Action<PlayerViewModel, int> RobberEvent;
        public event Action<List<Tuple<Guid, string, int>>> RobbingChoicesEvent;
        public event Action<PlayerActions> StartTurnEvent;
        #endregion

        #region Methods
        public void CompleteFirstInfrastructureSetup(uint settlementLocation, uint roadEndLocation)
        {
            this.localGameController.ContinueGameSetup(settlementLocation, roadEndLocation);
        }

        public void CompleteSecondInfrastructureSetup(uint settlementLocation, uint roadEndLocation)
        {
            this.localGameController.CompleteGameSetup(settlementLocation, roadEndLocation);
            this.localGameController.FinalisePlayerTurnOrder();
            this.localGameController.StartGamePlay();
        }

        public void DropResourcesFromPlayer(ResourceClutch dropResources)
        {
            this.localGameController.DropResources(dropResources);
            this.player.Update(dropResources, false);
        }

        public void GetRandomResourceFromOpponent(Guid opponentId)
        {
            this.localGameController.ChooseResourceFromOpponent(opponentId);
            this.StartTurn();
        }

        public void SetRobberLocation(uint hexIndex)
        {
            this.localGameController.SetRobberHex(hexIndex);
        }

        public void StartGame()
        {
            this.localGameController.JoinGame();
            this.localGameController.LaunchGame();
            this.localGameController.StartGameSetup();
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
                this.playerViewModelsById[resourceData.Key].Update(resourceData.Value, true);
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
                var playerViewModel = this.playerViewModelsById[entry.Key];
                foreach (var rc in entry.Value)
                {
                    playerViewModel.UpdateHistory($"{playerViewModel.Name} gained {rc.Resources.ToString()} from {rc.Location}");
                    this.playerViewModelsById[entry.Key].Update(rc.Resources, true);
                }
            }
        }

        private void ResourcesLostEventHandler(ResourceUpdate resourceUpdate)
        {
            // Resources lost by computer players during robber roll
            foreach (var kv in resourceUpdate.Resources)
            {
                var playerViewModel = this.playerViewModelsById[kv.Key];
                playerViewModel.UpdateHistory($"{playerViewModel.Name} lost resources to the robber");
                playerViewModel.Update(kv.Value, false);
            }
        }

        private void ResourcesTransferredEventHandler(ResourceTransactionList resourceTransactions)
        {
            for (var i = 0; i < resourceTransactions.Count; i++)
            {
                var resourceTransaction = resourceTransactions[i];
                var givingPlayerViewModel = this.playerViewModelsById[resourceTransaction.GivingPlayerId];
                var receivingPlayerViewModel = this.playerViewModelsById[resourceTransaction.ReceivingPlayerId];
                givingPlayerViewModel.Update(resourceTransaction.Resources, false);
                receivingPlayerViewModel.Update(resourceTransaction.Resources, true);
            }
        }

        private void RobbingChoicesEventHandler(Dictionary<Guid, int> choicesByPlayerId)
        {
            if (choicesByPlayerId == null)
            {
                // No meaningful choices i.e. robber location has no adjacent players or
                // just the main player.
                this.StartTurn();
                return;
            }

            if (choicesByPlayerId.Count == 1)
            {
                // Only one opponent so the choice is automatic
                var opponentId = choicesByPlayerId.Keys.First();
                var opponentViewModel = this.playerViewModelsById[opponentId];
                this.playerViewModelsById[this.player.Id].UpdateHistory($"Robbing resource from {opponentViewModel.Name}");
                opponentViewModel.UpdateHistory($"Being robbed by {this.player.Name}");

                this.localGameController.ChooseResourceFromOpponent(opponentId);

                this.StartTurn();
                return;
            }

            var choiceList = new List<Tuple<Guid, string, int>>();
            foreach (var kv in choicesByPlayerId)
            {
                // Ensure the local player is not included.
                if (kv.Key != this.player.Id)
                {
                    choiceList.Add(new Tuple<Guid, string, int>(
                      kv.Key,
                      this.playerViewModelsById[kv.Key].Name,
                      kv.Value
                      ));
                }
            }

            this.RobbingChoicesEvent?.Invoke(choiceList);
        }

        private void StartTurn()
        {
            this.playerActions.Clear();

            var buildSettlementStatus = this.localGameController.CanBuildSettlement();
            if (buildSettlementStatus == (LocalGameController.BuildStatuses.NotEnoughResourcesForSettlement | LocalGameController.BuildStatuses.NoSettlements))
                this.playerActions.AddBuildSettlementMessages("Not enough resources for a settlement", "No settlements");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NotEnoughResourcesForSettlement)
                this.playerActions.AddBuildSettlementMessages("Not enough resources for a settlement");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NoSettlements)
                this.playerActions.AddBuildSettlementMessages("No settlements");

            buildSettlementStatus = this.localGameController.CanBuildRoadSegment();
            if (buildSettlementStatus == (LocalGameController.BuildStatuses.NotEnoughResourcesForRoad | LocalGameController.BuildStatuses.NoRoads))
                this.playerActions.AddBuildSettlementMessages("Not enough resources for a road", "No roads");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NotEnoughResourcesForRoad)
                this.playerActions.AddBuildSettlementMessages("Not enough resources for a road");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NoRoads)
                this.playerActions.AddBuildSettlementMessages("No roads");

            buildSettlementStatus = this.localGameController.CanBuildCity();
            if (buildSettlementStatus == (LocalGameController.BuildStatuses.NotEnoughResourcesForCity | LocalGameController.BuildStatuses.NoCities))
                this.playerActions.AddBuildSettlementMessages("Not enough resources for a city", "No cities");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NotEnoughResourcesForCity)
                this.playerActions.AddBuildSettlementMessages("Not enough resources for a city");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NoRoads)
                this.playerActions.AddBuildSettlementMessages("No cities");

            this.StartTurnEvent?.Invoke(this.playerActions);
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
        #endregion
    }
}
