
namespace SoC.Harness.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.PlayerData;

    public class ControllerViewModel : INotifyPropertyChanged
    {
        public enum States
        {
            PlaceFirstInfrastructure,
            PlaceSecondInfrastructure,
            SelectRobberLocation,
            SelectPlayerToRob,
            ChoosePhaseAction,
        }

        #region Fields
        private TurnToken currentTurnToken;
        readonly PropertyChangedEventArgs diceOneChangedEventArgs = new PropertyChangedEventArgs("DiceOneImagePath");
        readonly PropertyChangedEventArgs diceTwoChangedEventArgs = new PropertyChangedEventArgs("DiceTwoImagePath");
        private GameBoardSetup gameBoardSetup;
        private readonly LocalGameController localGameController;
        private PlayerViewModel player;
        private Dictionary<Guid, PlayerViewModel> playerViewModelsById = new Dictionary<Guid, PlayerViewModel>();
        private PhaseActions phaseActions = new PhaseActions();
        private string setupMessage;
        private readonly PropertyChangedEventArgs setupMessagePropertyChangedEventArgs = new PropertyChangedEventArgs("SetupMessage");
        private States state;
        #endregion

        #region Construction
        private ControllerViewModel(LocalGameController localGameController)
        {
            this.localGameController = localGameController;
            this.localGameController.DiceRollEvent = this.DiceRollEventHandler;
            this.localGameController.ErrorRaisedEvent = this.ErrorRaisedEventHandler;
            this.localGameController.GameEvents = this.GameEventsHandler;
            this.localGameController.GameJoinedEvent = this.GameJoinedEventHandler;
            //this.localGameController.GameSetupUpdateEvent = this.GameSetupUpdateEventHandler;
            this.localGameController.StartPlayerTurnEvent = this.StartPlayerTurnEventHandler;
            //this.localGameController.GameSetupResourcesEvent = this.GameSetupResourcesEventHandler;
            this.localGameController.InitialBoardSetupEvent = this.InitialBoardSetupEventHandler;
            this.localGameController.ResourcesCollectedEvent = this.ResourcesCollectedEventHandler;
            this.localGameController.RobberEvent = this.RobberEventHandler;
            this.localGameController.ResourcesLostEvent = this.ResourcesLostEventHandler;
            this.localGameController.RobbingChoicesEvent = this.RobbingChoicesEventHandler;
            this.localGameController.ResourcesTransferredEvent = this.ResourcesTransferredEventHandler;
            this.localGameController.RoadSegmentBuiltEvent = this.RoadSegmentBuiltEventHandler;

            this.State = States.PlaceFirstInfrastructure;
        }
        #endregion

        #region Properties
        public string DiceOneImagePath { get; private set; }
        public string DiceTwoImagePath { get; private set; }
        public uint InitialSettlementLocation { get; set; }
        public uint InitialRoadEndLocation { get; set; }

        public States State
        {
            get { return this.state; }
            private set
            {
                this.state = value;
                switch (this.state)
                {
                    case States.PlaceFirstInfrastructure: this.SetupMessage = "Select location for FIRST Settlement and Road Segment"; break;
                    case States.PlaceSecondInfrastructure: this.SetupMessage = "Select location for SECOND Settlement and Road Segment"; break;
                    default: this.SetupMessage = null; break;
                }
            }
        }

        public string SetupMessage
        {
            get { return this.setupMessage; }
            private set
            {
                this.setupMessage = value;
                this.PropertyChanged?.Invoke(this, this.setupMessagePropertyChangedEventArgs);
            }
        }
        #endregion

        #region Events
        public event Action<ErrorDetails> ErrorRaisedEvent;
        public event Action<PlayerViewModel, PlayerViewModel, PlayerViewModel, PlayerViewModel> GameJoinedEvent;
        public event Action<GameBoardUpdate> GameSetupUpdateEvent;
        public event Action InitialBoardSetupEvent;
        public event Action<PlayerViewModel, int> RobberEvent;
        public event Action<List<Tuple<Guid, string, int>>> RobbingChoicesEvent;
        public event Action<PhaseActions> StartPhaseEvent;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Methods
        public static ControllerViewModel Load(string filePath)
        {
            var localGameController = new LocalGameController(new TestNumberGenerator(), null, true);
            var controller = new ControllerViewModel(localGameController);

            localGameController.Load(filePath);

            return controller;
        }

        public static ControllerViewModel New()
        {
            return new ControllerViewModel(new LocalGameController(new TestNumberGenerator(), new LocalPlayerPool(), true));
        }

        public void ContinueGame()
        {
            this.State = States.ChoosePhaseAction;
            this.localGameController.ContinueGamePlay();
            this.StartPhase();
        }
    
        public void DropResourcesFromPlayer(ResourceClutch dropResources)
        {
            this.State = States.SelectRobberLocation;
            this.localGameController.DropResources(dropResources);
            this.player.Update(dropResources, false);
        }

        public void EndTurn()
        {
            if (this.State == States.PlaceFirstInfrastructure)
            {
                this.localGameController.ContinueGameSetup(this.InitialSettlementLocation, this.InitialRoadEndLocation);
            }
            else if (this.State == States.PlaceSecondInfrastructure)
            {
                this.localGameController.CompleteGameSetup(this.InitialSettlementLocation, this.InitialRoadEndLocation);
                this.localGameController.FinalisePlayerTurnOrder();
                this.localGameController.StartGamePlay();
            }
            else
            {
                this.localGameController.EndTurn(this.currentTurnToken);
            }
        }

        public void GetRandomResourceFromOpponent(Guid opponentId)
        {
            this.localGameController.ChooseResourceFromOpponent(opponentId);
            this.StartPhase();
        }

        public void Save(string saveFilePath)
        {
            this.localGameController.Save(saveFilePath);
        }

        public void SetRobberLocation(uint hexIndex)
        {
            this.localGameController.SetRobberHex(hexIndex);
            this.State = States.ChoosePhaseAction;
        }

        public void StartGame()
        {
            this.localGameController.JoinGame();
            this.localGameController.LaunchGame();
            this.localGameController.StartGameSetup();
        }

        private void DiceRollEventHandler(uint dice1, uint dice2)
        {
            var diceRoll = dice1 + dice2;
            this.player.UpdateHistory("Rolled " + diceRoll);

            this.DiceOneImagePath = this.GetDiceImage(dice1);
            this.DiceTwoImagePath = this.GetDiceImage(dice2);
            this.PropertyChanged?.Invoke(this, this.diceOneChangedEventArgs);
            this.PropertyChanged?.Invoke(this, this.diceTwoChangedEventArgs);
        }

        private void ErrorRaisedEventHandler(ErrorDetails errorDetails)
        {
            this.ErrorRaisedEvent?.Invoke(errorDetails);
        }

        private void GameJoinedEventHandler(PlayerDataBase[] playerDataModels)
        {
            string firstPlayerIconPath = @"..\resources\icons\blue_icon.png";
            string secondPlayerIconPath = @"..\resources\icons\red_icon.png";
            string thirdPlayerIconPath = @"..\resources\icons\green_icon.png";
            string fourthPlayerIconPath = @"..\resources\icons\yellow_icon.png";

            var playerViewModel1 = new PlayerViewModel((PlayerFullDataModel)playerDataModels[0], firstPlayerIconPath);
            this.player = playerViewModel1;
            this.playerViewModelsById.Add(playerDataModels[0].Id, playerViewModel1);

            var playerViewModel2 = new PlayerViewModel((PlayerFullDataModel)playerDataModels[1], secondPlayerIconPath);
            this.playerViewModelsById.Add(playerDataModels[1].Id, playerViewModel2);

            var playerViewModel3 = new PlayerViewModel((PlayerFullDataModel)playerDataModels[2], thirdPlayerIconPath);
            this.playerViewModelsById.Add(playerDataModels[2].Id, playerViewModel3);

            var playerViewModel4 = new PlayerViewModel((PlayerFullDataModel)playerDataModels[3], fourthPlayerIconPath);
            this.playerViewModelsById.Add(playerDataModels[3].Id, playerViewModel4);

            this.GameJoinedEvent?.Invoke(playerViewModel1, playerViewModel2, playerViewModel3, playerViewModel4);
        }

        private void GameSetupResourcesEventHandler(ResourceUpdate resourceUpdate)
        {
            this.State = States.ChoosePhaseAction;
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

            this.State = States.PlaceSecondInfrastructure;

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

            this.GameSetupUpdateEvent?.Invoke(boardUpdate);
        }

        private string GetDiceImage(uint diceRoll)
        {
            const string OneImagePath = @"..\resources\dice\one.png";
            const string TwoImagePath = @"..\resources\dice\two.png";
            const string ThreeImagePath = @"..\resources\dice\three.png";
            const string FourImagePath = @"..\resources\dice\four.png";
            const string FiveImagePath = @"..\resources\dice\five.png";
            const string SixImagePath = @"..\resources\dice\six.png";

            switch (diceRoll)
            {
                case 1: return OneImagePath;
                case 2: return TwoImagePath;
                case 3: return ThreeImagePath;
                case 4: return FourImagePath;
                case 5: return FiveImagePath;
                case 6: return SixImagePath;
            }

            throw new NotImplementedException("Should not get here");
        }

        private void InitialBoardSetupEventHandler(GameBoardSetup gameBoardSetup)
        {
            this.gameBoardSetup = gameBoardSetup;
            this.InitialBoardSetupEvent?.Invoke();
        }

        private void GameEventsHandler(List<GameEvent> gameEvents)
        {
            foreach(var gameEvent in gameEvents)
            {
                if (gameEvent is DiceRollEvent rolledDiceEvent)
                {
                    var diceRoll = rolledDiceEvent.Dice1 + rolledDiceEvent.Dice2;
                    var playerViewModel = this.playerViewModelsById[gameEvent.PlayerId];
                    playerViewModel.UpdateHistory($"{playerViewModel.Name} rolled {diceRoll}");
                }
                else if (gameEvent is InfrastructureBuiltEvent infrastructureBuiltEvent)
                {
                    var playerViewModel = this.playerViewModelsById[infrastructureBuiltEvent.PlayerId];
                    var line = "Built settlement at " + infrastructureBuiltEvent.SettlementLocation;
                    playerViewModel.UpdateHistory(line);
                    line = $"Built road from {infrastructureBuiltEvent.SettlementLocation} to {infrastructureBuiltEvent.RoadSegmentEndLocation}";
                    playerViewModel.UpdateHistory(line);
                }
                else if (gameEvent is ResourceCollectedEvent resourcesCollectedEvent)
                {
                    this.playerViewModelsById[resourcesCollectedEvent.PlayerId]
                        .Update(resourcesCollectedEvent.Resources[0].Resources, true);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
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

            this.StartPhase();
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

        private void RoadSegmentBuiltEventHandler(PlayerDataBase playerDataBase)
        {
            throw new NotImplementedException();
        }

        private void RobbingChoicesEventHandler(Dictionary<Guid, int> choicesByPlayerId)
        {
            if (choicesByPlayerId == null)
            {
                // No meaningful choices i.e. robber location has no adjacent players or
                // just the main player.
                this.StartPhase();
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

                this.StartPhase();
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

        private void StartPhase()
        {
            this.phaseActions.Clear();

            var buildSettlementStatus = this.localGameController.CanBuildSettlement();
            if (buildSettlementStatus == (LocalGameController.BuildStatuses.NotEnoughResourcesForSettlement | LocalGameController.BuildStatuses.NoSettlements))
                this.phaseActions.AddBuildSettlementMessages("Not enough resources for a settlement", "No settlements");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NotEnoughResourcesForSettlement)
                this.phaseActions.AddBuildSettlementMessages("Not enough resources for a settlement");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NoSettlements)
                this.phaseActions.AddBuildSettlementMessages("No settlements");

            buildSettlementStatus = this.localGameController.CanBuildRoadSegment();
            if (buildSettlementStatus == (LocalGameController.BuildStatuses.NotEnoughResourcesForRoad | LocalGameController.BuildStatuses.NoRoads))
                this.phaseActions.AddBuildRoadMessages("Not enough resources for a road", "No roads");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NotEnoughResourcesForRoad)
                this.phaseActions.AddBuildRoadMessages("Not enough resources for a road");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NoRoads)
                this.phaseActions.AddBuildRoadMessages("No roads");

            buildSettlementStatus = this.localGameController.CanBuildCity();
            if (buildSettlementStatus == (LocalGameController.BuildStatuses.NotEnoughResourcesForCity | LocalGameController.BuildStatuses.NoCities))
                this.phaseActions.AddBuildCityMessages("Not enough resources for a city", "No cities");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NotEnoughResourcesForCity)
                this.phaseActions.AddBuildCityMessages("Not enough resources for a city");
            else if (buildSettlementStatus == LocalGameController.BuildStatuses.NoRoads)
                this.phaseActions.AddBuildCityMessages("No cities");

            this.StartPhaseEvent?.Invoke(this.phaseActions);
        }

        internal IList<Connection> GetValidConnectionsForPlayerInfrastructure(Guid playerId)
        {
            throw new NotImplementedException();
        }

        private void RobberEventHandler(int numberOfResourcesToSelect)
        {
            this.State = States.SelectRobberLocation;
            this.RobberEvent?.Invoke(this.player, numberOfResourcesToSelect);
        }

        private void StartPlayerTurnEventHandler(TurnToken turnToken)
        {
            this.currentTurnToken = turnToken;
        }

        public void BuildRoadSegment(uint start, uint end)
        {
            this.localGameController.BuildRoadSegment(this.currentTurnToken, start, end);
        }

        public Tuple<ResourceTypes?, uint>[] GetHexData()
        {
            return this.gameBoardSetup != null ? this.gameBoardSetup.HexData : null;
        }

        public Dictionary<uint, Guid> GetInitialSettlementData()
        {
            return this.gameBoardSetup != null ? this.gameBoardSetup.SettlementData : null;
        }

        public Tuple<uint, uint, Guid>[] GetInitialRoadSegmentData()
        {
            return this.gameBoardSetup != null ? this.gameBoardSetup.RoadSegmentData : null;
        }

        public Dictionary<uint, Guid> GetInitialCityData()
        {
            return this.gameBoardSetup != null ? this.gameBoardSetup.CityData : null;
        }

        public uint[] GetNeighbouringLocationsFrom(uint location)
        {
            return this.localGameController.GetNeighbouringLocationsFrom(location);
        }

        public uint[] GetValidConnectedLocationsFrom(uint location)
        {
            return this.localGameController.GetValidConnectedLocationsFrom(location);
        }
        #endregion
    }
}
