
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using Interfaces;
    using Jabberwocky.SoC.Library.Store;

    [DebuggerDisplay("Name: {Name}, Id: {Id}")]
    public class Player : IPlayer
    {
        #region Fields
        private const int TotalRoadSegments = 15;
        private const int TotalSettlements = 5;
        private const int TotalCities = 4;

        private bool hasLargestArmy;
        private bool hasLongestRoad;
        #endregion

        #region Construction
        public Player()
        {
            this.Id = Guid.NewGuid();
        }

        public Player(string name) : this()
        {
            this.Name = name;
        }

        [Obsolete("Deprecated. Use Player::ctor(PlayerSaveObject) instead.")]
        public Player(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data)
        {
            this.Id = data.GetIdentityValue(GameDataValueKeys.PlayerId);
            this.Name = data.GetStringValue(GameDataValueKeys.PlayerName);
            this.BrickCount = data.GetIntegerValue(GameDataValueKeys.PlayerBrick);
            this.GrainCount = data.GetIntegerValue(GameDataValueKeys.PlayerGrain);
            this.LumberCount = data.GetIntegerValue(GameDataValueKeys.PlayerLumber);
            this.OreCount = data.GetIntegerValue(GameDataValueKeys.PlayerOre);
            this.WoolCount = data.GetIntegerValue(GameDataValueKeys.PlayerWool);
        }

        public Player(PlayerModel playerSaveObject)
        {
            this.CitiesBuilt = playerSaveObject.CitiesBuilt;
            this.HeldCards = playerSaveObject.HeldCards;
            this.Id = playerSaveObject.Id;
            this.KnightCards = playerSaveObject.KnightCards;
            this.Name = playerSaveObject.Name;
            this.PlayedCards = playerSaveObject.PlayedCards;
            this.Resources = playerSaveObject.Resources;
            this.RoadSegmentsBuilt = playerSaveObject.RoadSegmentsBuilt;
            this.SettlementsBuilt = playerSaveObject.SettlementsBuilt;
        }
        #endregion

        #region Properties
        public int CitiesBuilt { get; protected set; }
        public bool HasLargestArmy
        {
            get { return this.hasLargestArmy; }
            set
            {
                if (this.hasLargestArmy == value)
                {
                    return;
                }

                this.hasLargestArmy = value;
                if (this.hasLargestArmy)
                {
                    this.VictoryPoints += 2u;
                }
                else
                {
                    this.VictoryPoints -= 2u;
                }
            }
        }
        public bool HasLongestRoad
        {
            get { return this.hasLongestRoad; }
            set
            {
                if (this.hasLongestRoad == value)
                {
                    return;
                }

                this.hasLongestRoad = value;
                if (this.hasLongestRoad)
                {
                    this.VictoryPoints += 2u;
                }
                else
                {
                    this.VictoryPoints -= 2u;
                }
            }
        }
        public List<DevelopmentCard> HeldCards { get; protected set; }
        public Guid Id { get; private set; }
        public virtual bool IsComputer { get { return false; } }
        public uint KnightCards { get; private set; }
        public string Name { get; private set; }
        public List<DevelopmentCard> PlayedCards { get; protected set; }
        public int RemainingCities { get { return TotalCities - this.CitiesBuilt; } }
        public int RemainingRoadSegments { get { return TotalRoadSegments - this.RoadSegmentsBuilt; } }
        public int RemainingSettlements { get { return TotalSettlements - this.SettlementsBuilt; } }
        public ResourceClutch Resources { get; protected set; }
        public int RoadSegmentsBuilt { get; protected set; }
        public int SettlementsBuilt { get; protected set; }
        public uint VictoryPoints { get; protected set; }

        // TODO: Obsolete. 
        public int BrickCount { get; protected set; }
        public int GrainCount { get; protected set; }
        public int LumberCount { get; protected set; }
        public int OreCount { get; protected set; }
        public int WoolCount { get; protected set; }
        public virtual int ResourcesCount
        {
            get
            {
                return this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount + this.WoolCount;
            }
        }
        #endregion

        #region Methods
        public static IPlayer CreatePlayer(PlayerModel playerSaveModel)
        {
            if (playerSaveModel.IsComputer)
            {
                return new ComputerPlayer
                {
                    Id = playerSaveModel.Id,
                    Name = playerSaveModel.Name
                };
            }

            return new Player
            {
                CitiesBuilt = playerSaveModel.CitiesBuilt
            };
        }

        public void AddResources(ResourceClutch resourceClutch)
        {
            this.Resources += resourceClutch;
            this.BrickCount += resourceClutch.BrickCount;
            this.GrainCount += resourceClutch.GrainCount;
            this.LumberCount += resourceClutch.LumberCount;
            this.OreCount += resourceClutch.OreCount;
            this.WoolCount += resourceClutch.WoolCount;
        }

        public PlayerDataModel GetDataView()
        {
            var dataView = new PlayerDataModel();

            dataView.Id = this.Id;
            dataView.Name = this.Name;
            dataView.ResourceCards = this.ResourcesCount;
            dataView.HiddenDevelopmentCards = 0;
            dataView.DisplayedDevelopmentCards = null;
            dataView.IsComputer = this.IsComputer;

            return dataView;
        }

        public ResourceClutch LoseResourceAtIndex(int index)
        {
            if (index < 0 || index >= this.ResourcesCount)
            {
                throw new IndexOutOfRangeException("Index " + index + " is out of bounds (0.." + (this.ResourcesCount - 1) + ").");
            }

            if (this.ResourcesCount == 0)
            {
                return ResourceClutch.Zero;
            }

            return this.GetResourceForIndex(index);
        }

        public ResourceClutch LoseResourcesOfType(ResourceTypes resourceType)
        {
            if (resourceType == ResourceTypes.Brick)
            {
                var resourceClutch = new ResourceClutch(this.BrickCount, 0, 0, 0, 0);
                this.BrickCount = 0;
                return resourceClutch;
            }

            if (resourceType == ResourceTypes.Grain)
            {
                var resourceClutch = new ResourceClutch(0, this.GrainCount, 0, 0, 0);
                this.GrainCount = 0;
                return resourceClutch;
            }

            if (resourceType == ResourceTypes.Lumber)
            {
                var resourceClutch = new ResourceClutch(0, 0, this.LumberCount, 0, 0);
                this.LumberCount = 0;
                return resourceClutch;
            }

            if (resourceType == ResourceTypes.Ore)
            {
                var resourceClutch = new ResourceClutch(0, 0, 0, this.OreCount, 0);
                this.OreCount = 0;
                return resourceClutch;
            }

            if (resourceType == ResourceTypes.Wool)
            {
                var resourceClutch = new ResourceClutch(0, 0, 0, 0, this.WoolCount);
                this.WoolCount = 0;
                return resourceClutch;
            }

            throw new NotImplementedException("Should not get here");
        }

        public void PayForDevelopmentCard()
        {
            this.GrainCount--;
            this.OreCount--;
            this.WoolCount--;
        }

        public void PlaceCity()
        {
            this.GrainCount -= Constants.GrainForBuildingCity;
            this.OreCount -= Constants.OreForBuildingCity;
            this.CitiesBuilt++;
            this.VictoryPoints++;
        }

        public void PlaceKnightDevelopmentCard()
        {
            this.KnightCards++;
        }

        public void PlaceRoadSegment()
        {
            this.BrickCount--;
            this.LumberCount--;
            this.RoadSegmentsBuilt++;
        }

        public void PlaceSettlement()
        {
            this.BrickCount--;
            this.GrainCount--;
            this.LumberCount--;
            this.WoolCount--;
            this.SettlementsBuilt++;
            this.VictoryPoints++;
        }

        public void PlaceStartingInfrastructure()
        {
            this.RoadSegmentsBuilt++;
            this.SettlementsBuilt++;
            this.VictoryPoints++;
        }

        public void RemoveResources(ResourceClutch resourceClutch)
        {
            if (this.BrickCount - resourceClutch.BrickCount < 0 ||
                this.GrainCount - resourceClutch.GrainCount < 0 ||
                this.LumberCount - resourceClutch.LumberCount < 0 ||
                this.OreCount - resourceClutch.OreCount < 0 ||
                this.WoolCount - resourceClutch.WoolCount < 0)
            {
                throw new ArithmeticException("No resource count can be negative.");
            }

            this.BrickCount -= resourceClutch.BrickCount;
            this.GrainCount -= resourceClutch.GrainCount;
            this.LumberCount -= resourceClutch.LumberCount;
            this.OreCount -= resourceClutch.OreCount;
            this.WoolCount -= resourceClutch.WoolCount;
        }

        /// <summary>
        /// Loads player properties from XML reader.
        /// </summary>
        /// <param name="reader">XML reader containing player properties.</param>
        internal void Load(XmlReader reader)
        {
            this.Id = Guid.Empty;

            try
            {
                var idValue = reader.GetAttribute("id");
                if (!String.IsNullOrEmpty(idValue))
                {
                    this.Id = Guid.Parse(idValue);
                }

                this.Name = reader.GetAttribute("name");
                this.BrickCount = this.GetValueOrZero(reader, "brick");
                this.GrainCount = this.GetValueOrZero(reader, "grain");
                this.LumberCount = this.GetValueOrZero(reader, "lumber");
                this.OreCount = this.GetValueOrZero(reader, "ore");
                this.WoolCount = this.GetValueOrZero(reader, "wool");
            }
            catch (Exception e)
            {
                throw new Exception("Ëxception thrown during player loading.", e);
            }

            if (this.Id == Guid.Empty)
            {
                throw new Exception("No id found for player in stream.");
            }

            if (String.IsNullOrEmpty(this.Name))
            {
                throw new Exception("No name found for player in stream.");
            }
        }

        private ResourceClutch GetResourceForIndex(int index)
        {
            if (index < this.BrickCount)
            {
                this.BrickCount--;
                return ResourceClutch.OneBrick;
            }

            if (index < this.BrickCount + this.GrainCount)
            {
                this.GrainCount--;
                return ResourceClutch.OneGrain;
            }

            if (index < this.BrickCount + this.GrainCount + this.LumberCount)
            {
                this.LumberCount--;
                return ResourceClutch.OneLumber;
            }

            if (index < this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount)
            {
                this.OreCount--;
                return ResourceClutch.OneOre;
            }

            if (index < this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount + this.WoolCount)
            {
                this.WoolCount--;
                return ResourceClutch.OneWool;
            }

            throw new NotImplementedException("Should not get here");
        }

        private int GetValueOrZero(XmlReader reader, String attributeName)
        {
            var value = reader.GetAttribute(attributeName);
            return !String.IsNullOrEmpty(value) ? int.Parse(value) : 0;
        }
        #endregion
    }
}
