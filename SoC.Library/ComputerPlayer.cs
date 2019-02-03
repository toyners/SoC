
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Generic;
    using GameBoards;
    using Interfaces;
    using GameActions;
    using System.Xml;
    using Jabberwocky.SoC.Library.Store;
    using Jabberwocky.SoC.Library.Enums;
    using Jabberwocky.SoC.Library.PlayerData;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using System.Linq;
    using Jabberwocky.SoC.Library.GameEvents;

    public class ComputerPlayer : Player, IComputerPlayer
    {
        private readonly Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
        private readonly GameBoard gameBoard;
        private readonly INumberGenerator numberGenerator;
        private readonly List<uint> settlementCandidates = new List<uint>();
        private readonly DecisionMaker decisionMaker;

        #region Construction
        public ComputerPlayer() { } // For use when inflating from file. 

        public ComputerPlayer(string name, GameBoard gameBoard, INumberGenerator numberGenerator, IInfrastructureAI infrastructureAI) : base(name)
        {
            this.gameBoard = gameBoard;
            this.numberGenerator = numberGenerator;
            this.decisionMaker = new DecisionMaker(this.numberGenerator);
        }

        [Obsolete("Deprecated. Use ComputerPlayer::ctor(PlayerSaveObject) instead")]
        public ComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator) : base(data)
        {
            this.gameBoard = board;
            this.numberGenerator = numberGenerator;
            this.decisionMaker = new DecisionMaker(this.numberGenerator);
        }

        public ComputerPlayer(string name, INumberGenerator numberGenerator) : base(name)
        {
            this.numberGenerator = numberGenerator;
            this.decisionMaker = new DecisionMaker(this.numberGenerator);
        }
        public ComputerPlayer(PlayerModel playerModel, INumberGenerator numberGenerator) : base(playerModel)
        {
            this.numberGenerator = numberGenerator;
            this.decisionMaker = new DecisionMaker(this.numberGenerator);
        }
        #endregion

        #region Properties
        public override bool IsComputer { get { return true; } }
        #endregion

        #region Methods
        public static ComputerPlayer CreateFromXML(XmlReader reader)
        {
            var computerPlayer = new ComputerPlayer();
            computerPlayer.Load(reader);
            return computerPlayer;
        }

        public virtual void AddDevelopmentCard(DevelopmentCard developmentCard)
        {
            this.HeldCards.Add(developmentCard);
        }

        public virtual void BuildInitialPlayerActions(PlayerDataModel[] otherPlayerData, bool rolledSeven)
        {
            this.decisionMaker.Reset();

            var resourceClutch = new ResourceClutch(this.BrickCount, this.GrainCount, this.LumberCount, this.OreCount, this.WoolCount);

            if (resourceClutch >= ResourceClutch.RoadSegment && this.RemainingRoadSegments > 0)
            {
                // Get the current road segment build candidates 
                var roadPathCandidates = this.gameBoard.BoardQuery.GetRoadPathCandidates(this.settlementCandidates);

                if (roadPathCandidates != null)
                {
                    // Can build at least one road segment
                    // Get the total number of road segments that we could build regardless of resources
                    var roadSegmentCandidateCount = 0;
                    foreach (var rpc in roadPathCandidates)
                    {
                        roadSegmentCandidateCount += rpc.Value.Count;
                    }

                    // if building the next road segment wins the game then do this
                    if (this.VictoryPoints >= 8 && !this.HasLongestRoad)
                    {
                        var requiredRoadSegmentCount = 0;
                        foreach (var otherPlayer in otherPlayerData)
                        {
                            if (!otherPlayer.HasLongestRoad)
                            {
                                continue;
                            }

                            requiredRoadSegmentCount = this.gameBoard.BoardQuery.GetLongestRoadForPlayer(otherPlayer.Id).Count -
                              this.gameBoard.BoardQuery.GetLongestRoadForPlayer(this.Id).Count;

                            break;
                        }

                        // Number of road segments to build to have the longest road 
                        requiredRoadSegmentCount++;

                        if (requiredRoadSegmentCount <= roadSegmentCandidateCount)
                        {
                            // The number of road segments that is required to have the longest road is smaller than
                            // the number of possible road segments that can be built.
                            var requiredResources = ResourceClutch.RoadSegment * requiredRoadSegmentCount;
                            if (requiredResources <= resourceClutch && requiredRoadSegmentCount <= this.RemainingRoadSegments)
                            {
                                // Got the required resources to build the required road segments
                                var roadPathCandidateIndex = 0;
                                var roadPathCandidate = roadPathCandidates[roadPathCandidateIndex++];
                                var index = 0;
                                while (requiredRoadSegmentCount-- > 0)
                                {
                                    if (index == roadPathCandidate.Value.Count)
                                    {
                                        index = 0;
                                        roadPathCandidate = roadPathCandidates[roadPathCandidateIndex++];
                                    }

                                    var destination = roadPathCandidate.Value[index++];

                                    var roadBuildSegmentAction = new BuildRoadSegmentAction(roadPathCandidate.Key, destination);
                                    this.actions.Enqueue(roadBuildSegmentAction);
                                }
                            }
                        }
                    }

                    var workingRemainingRoadSegments = this.RemainingRoadSegments;

                    foreach (var kv in roadPathCandidates)
                    {
                        foreach (var destination in kv.Value)
                        {
                            // Can build road - boost it if road builder strategy or if building the next road segment
                            // will capture the 2VP
                            uint multiplier = 1;

                            Action action = () =>
                            {
                                var roadBuildSegmentAction = new BuildRoadSegmentAction(kv.Key, destination);
                                this.actions.Enqueue(roadBuildSegmentAction);
                            };

                            this.decisionMaker.AddDecision(action, multiplier);
                        }
                    }
                }
            }

            if (resourceClutch >= ResourceClutch.Settlement && this.RemainingSettlements > 0)
            {
                // Got the resources to build settlement. Find locations that can be build on 
                // Boost it if can build on target settlement - increase boost multiplier based on desirablity of target settlement
                // If building the settlement on any location will win the game then do it
            }

            if (resourceClutch >= ResourceClutch.City && this.RemainingCities > 0 && this.SettlementsBuilt > this.CitiesBuilt)
            {
                // Got resources to build city and got settlements to promote.
                // If building the city will win the game then do it

                // Otherwise include city promotion as a possible decision
            }

            if (resourceClutch >= ResourceClutch.DevelopmentCard)
            {
                // Can buy development card
                // Boost it if playing development card buyer strategy
            }

            var decision = this.decisionMaker.DetermineDecision();
            if (decision != null)
                decision.Invoke();
        }

        public virtual uint ChooseCityLocation()
        {
            throw new NotImplementedException();
        }

        public virtual void ChooseInitialInfrastructure(out UInt32 settlementLocation, out UInt32 roadEndLocation)
        {
            if (this.SettlementsBuilt >= 2)
            {
                throw new Exception("Should not get here");
            }

            var settlementIndex = -1;
            var bestLocations = this.gameBoard.BoardQuery.GetLocationsWithBestYield(5);
            var n = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(100);

            if (n < 55)
            {
                settlementIndex = 0;
            }
            else if (n < 75)
            {
                settlementIndex = 1;
            }
            else if (n < 85)
            {
                settlementIndex = 2;
            }
            else if (n < 95)
            {
                settlementIndex = 3;
            }
            else
            {
                settlementIndex = 4;
            }

            settlementLocation = bestLocations[settlementIndex];

            // Build road towards another random location 
            var roadDestinationIndex = -1;
            do
            {
                n = this.numberGenerator.GetRandomNumberBetweenZeroAndMaximum(100);

                if (n < 55)
                {
                    roadDestinationIndex = 0;
                }
                else if (n < 75)
                {
                    roadDestinationIndex = 1;
                }
                else if (n < 85)
                {
                    roadDestinationIndex = 2;
                }
                else if (n < 95)
                {
                    roadDestinationIndex = 3;
                }
                else
                {
                    roadDestinationIndex = 4;
                }

            } while (roadDestinationIndex == settlementIndex);

            var trek = this.gameBoard.GetPathBetweenLocations(settlementLocation, bestLocations[roadDestinationIndex]);

            roadEndLocation = trek[trek.Count - 1];
        }

        public virtual KnightDevelopmentCard GetKnightCard()
        {
            var card = this.HeldCards.Where(c => c.Type == DevelopmentCardTypes.Knight).FirstOrDefault();
            this.HeldCards.Remove(card);
            this.PlayedCards.Add(card);
            return (KnightDevelopmentCard)card;
        }

        public virtual IPlayer ChoosePlayerToRob(IEnumerable<IPlayer> otherPlayers)
        {
            throw new NotImplementedException();
        }

        public virtual MonopolyDevelopmentCard ChooseMonopolyCard()
        {
            throw new NotImplementedException();
        }

        public virtual uint ChooseRobberLocation()
        {
            throw new NotImplementedException();
        }

        public virtual ResourceClutch ChooseResourcesToCollectFromBank()
        {
            throw new NotImplementedException();
        }

        public virtual ResourceClutch ChooseResourcesToDrop()
        {
            throw new NotImplementedException();
        }

        public virtual ResourceTypes ChooseResourceTypeToRob()
        {
            throw new NotImplementedException();
        }

        public virtual UInt32 ChooseSettlementLocation()
        {
            // Find location that has the highest chance of a return for any roll.
            var bestLocationIndex = 0u;
            if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(this.gameBoard, out bestLocationIndex))
            {
                throw new Exception("Should not get here"); //TODO: Clean up
            }

            return bestLocationIndex;
        }

        public virtual YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard()
        {
            throw new NotImplementedException();
        }

        // TODO: Pass in interface that only contains the methods required by the computer player e.g build settlement
        public ComputerPlayerAction PlayTurn(PlayerDataModel[] otherPlayerData, LocalGameController localGameController)
        {
            throw new NotImplementedException();
        }

        public void DropResources(int resourceCount)
        {
            throw new NotImplementedException();
        }

        public virtual ComputerPlayerAction GetPlayerAction()
        {
            if (this.actions.Count == 0)
            {
                return null;
            }

            var action = this.actions.Dequeue();
            if (action.ActionType == ComputerPlayerActionTypes.EndTurn)
            {
                return null;
            }

            return action;
        }

        private int CalculateChanceOfReturnOnRoll(uint[] productionValues)
        {
            int totalChance = 0;
            foreach (var productionValue in productionValues)
            {
                switch (productionValue)
                {
                    case 2:
                    case 12:
                        totalChance += 1;
                        break;
                    case 3:
                    case 11:
                        totalChance += 2;
                        break;
                    case 4:
                    case 10:
                        totalChance += 3;
                        break;
                    case 5:
                    case 9:
                        totalChance += 4;
                        break;
                    case 6:
                    case 8:
                        totalChance += 5;
                        break;
                }
            }

            return totalChance;
        }

        private void ChooseRoad(GameBoard gameBoardData, out UInt32 roadStartLocation, out UInt32 roadEndLocation)
        {
            var settlementsForPlayer = gameBoardData.GetSettlementsForPlayer(this.Id);
            if (settlementsForPlayer == null || settlementsForPlayer.Count == 0)
            {
                throw new Exception("No settlements found for player with id " + this.Id);
            }

            UInt32 bestLocationIndex = 0;
            if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
            {
                throw new Exception("Should not get here"); // TODO: Clean up
            }

            Tuple<UInt32, List<UInt32>> shortestPathInformation = null;
            foreach (var locationIndex in settlementsForPlayer)
            {
                var path = gameBoardData.GetPathBetweenLocations(locationIndex, bestLocationIndex);

                if (shortestPathInformation == null || shortestPathInformation.Item2.Count > path.Count)
                {
                    shortestPathInformation = new Tuple<UInt32, List<UInt32>>(locationIndex, path);
                }
            }

            roadStartLocation = shortestPathInformation.Item1;
            var shortestPath = shortestPathInformation.Item2;
            roadEndLocation = shortestPath[shortestPath.Count - 1];
        }

        private Boolean TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(GameBoard gameBoardData, out UInt32 bestLocationIndex)
        {
            // Find location that has the highest chance of a return for any roll.
            var bestChanceOfReturnOnRoll = 0;
            var gotBestLocationIndex = false;
            bestLocationIndex = 0;

            // Iterate over every location and determine the chance of return for all resource providers
            for (UInt32 index = 0; index < gameBoardData.Length; index++)
            {
                //Guid playerId;
                var canPlaceResult = gameBoardData.CanPlaceSettlement(this.Id, index);
                if (canPlaceResult.Status == GameBoard.VerificationStatus.LocationForSettlementIsInvalid ||
                    canPlaceResult.Status == GameBoard.VerificationStatus.LocationIsOccupied ||
                    canPlaceResult.Status == GameBoard.VerificationStatus.TooCloseToSettlement)
                {
                    continue;
                }

                var productionValues = gameBoardData.GetProductionValuesForLocation(index);
                var chanceOfReturnOnRoll = this.CalculateChanceOfReturnOnRoll(productionValues);
                if (chanceOfReturnOnRoll > bestChanceOfReturnOnRoll)
                {
                    bestChanceOfReturnOnRoll = chanceOfReturnOnRoll;
                    bestLocationIndex = index;
                    gotBestLocationIndex = true;
                }
            }

            return gotBestLocationIndex;
        }

        private List<UInt32> GetPathToLocationThatHasBestChanceOfReturnOnRoll(GameBoard gameBoardData, UInt32 locationIndex)
        {
            var bestLocationIndex = 0u;

            if (!this.TryGetIndexOfLocationThatHasBestChanceOfReturnOnRoll(gameBoardData, out bestLocationIndex))
            {
                throw new Exception("Should not get here"); // TODO: Clean up
            }

            return gameBoardData.GetPathBetweenLocations(locationIndex, bestLocationIndex);
        }

        public virtual DropResourcesAction GetDropResourcesAction()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
