
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using Interfaces;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.Enums;
    using Jabberwocky.SoC.Library.PlayerData;
    using Jabberwocky.SoC.Library.Store;

    [DebuggerDisplay("Name: {Name}, Id: {Id}")]
    public class Player : IPlayer
    {
        #region Fields
        public const int TotalRoadSegments = 15;
        public const int TotalSettlements = 5;
        public const int TotalCities = 4;

        private bool hasLargestArmy;
        private bool hasLongestRoad;
        #endregion

        #region Construction
        public Player() : this(null, Guid.NewGuid())
        {
        }

        public Player(string name) : this(name, Guid.NewGuid())
        {
        }

        public Player(string name, Guid id)
        {
            this.Id = id;
            this.Name = name;
            this.HeldCards = new List<DevelopmentCard>();
            this.PlayedCards = new List<DevelopmentCard>();
        }

        [Obsolete("Deprecated. Use Player::ctor(PlayerModel) instead.")]
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

        public Player(PlayerModel playerModel)
        {
            this.PlacedCities = playerModel.CitiesBuilt;
            this.HeldCards = playerModel.HeldCards;
            this.Id = playerModel.Id;
            this.PlayedKnightCards = playerModel.KnightCards;
            this.Name = playerModel.Name;
            this.PlayedCards = playerModel.PlayedCards;
            this.Resources = playerModel.Resources;
            this.PlacedRoadSegments = playerModel.RoadSegmentsBuilt;
            this.PlacedSettlements = playerModel.SettlementsBuilt;
        }
        #endregion

        #region Properties
        public bool CanBuyDevelopmentCard => this.Resources >= ResourceClutch.DevelopmentCard;
        public PlayerPlacementStatusCodes CanPlaceCity => this.Resources < ResourceClutch.City ? PlayerPlacementStatusCodes.NotEnoughResources : PlayerPlacementStatusCodes.Success;
        public PlayerPlacementStatusCodes CanPlaceRoadSegment
        {
            get
            {
                if (this.RemainingRoadSegments < 0)
                    throw new Exception();
                if (this.RemainingRoadSegments == 0)
                    return PlayerPlacementStatusCodes.NoRoadSegments;
                else if (this.Resources < ResourceClutch.RoadSegment)
                    return PlayerPlacementStatusCodes.NotEnoughResources;
                return PlayerPlacementStatusCodes.Success;
            }
        }
        public PlayerPlacementStatusCodes CanPlaceSettlement
        {
            get
            {
                if (this.RemainingSettlements <= 0)
                    return PlayerPlacementStatusCodes.NoSettlements;
                else if (this.Resources < ResourceClutch.Settlement)
                    return PlayerPlacementStatusCodes.NotEnoughResources;
                return PlayerPlacementStatusCodes.Success;
            }
        }
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
        public int PlayedKnightCards { get; private set; }
        public string Name { get; private set; }
        public List<DevelopmentCard> PlayedCards { get; protected set; }
        public int RemainingCities { get { return TotalCities - this.PlacedCities; } }
        public int RemainingRoadSegments { get { return TotalRoadSegments - this.PlacedRoadSegments; } }
        public int RemainingSettlements { get { return TotalSettlements - this.PlacedSettlements; } }
        public ResourceClutch Resources { get; protected set; }
        public int PlacedCities { get; protected set; }
        public int PlacedRoadSegments { get; protected set; }
        public int PlacedSettlements { get; protected set; }
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
        public void AddResources(ResourceClutch resourceClutch)
        {
            this.Resources += resourceClutch;
            this.BrickCount += resourceClutch.BrickCount;
            this.GrainCount += resourceClutch.GrainCount;
            this.LumberCount += resourceClutch.LumberCount;
            this.OreCount += resourceClutch.OreCount;
            this.WoolCount += resourceClutch.WoolCount;
        }

        public PlayerDataBase GetDataModel(bool provideFullPlayerData)
        {
            PlayerDataBase result = null;

            if (provideFullPlayerData)
                result = new PlayerFullDataModel(this);
            else
                result = new PlayerDataModel(this);

            return result;
        }

        public ResourceClutch LoseResourceAtIndex(int index)
        {
            if (this.ResourcesCount == 0)
            {
                return ResourceClutch.Zero;
            }

            if (index < 0 || index >= this.ResourcesCount)
            {
                throw new IndexOutOfRangeException("Index " + index + " is out of bounds (0.." + (this.ResourcesCount - 1) + ").");
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

        public void BuyDevelopmentCard()
        {
            this.Resources -= ResourceClutch.DevelopmentCard;
        }

        public void PlaceCity()
        {
            this.Resources -= ResourceClutch.City;
            this.PlacedCities++;
            this.PlacedSettlements--;
            this.VictoryPoints++;
        }

        public void PlayDevelopmentCard(DevelopmentCard card)
        {
            this.HeldCards.Remove(card);
            this.PlayedCards.Add(card);

            if (card is KnightDevelopmentCard)
                this.PlayedKnightCards++;
        }

        public void PlaceRoadSegment(bool deductResources = true)
        {
            if (deductResources)
                this.Resources -= ResourceClutch.RoadSegment;
            this.PlacedRoadSegments++;

            if (this.PlacedRoadSegments > TotalRoadSegments)
                throw new Exception();
        }

        public void PlaceSettlement()
        {
            this.Resources -= ResourceClutch.Settlement;
            this.PlacedSettlements++;
            this.VictoryPoints++;
        }

        public void PlaceStartingInfrastructure()
        {
            this.PlacedRoadSegments++;
            this.PlacedSettlements++;
            this.VictoryPoints++;
        }

        public void RemoveResources(ResourceClutch resourceClutch)
        {
            if (this.Resources.BrickCount - resourceClutch.BrickCount < 0 ||
                this.Resources.GrainCount - resourceClutch.GrainCount < 0 ||
                this.Resources.LumberCount - resourceClutch.LumberCount < 0 ||
                this.Resources.OreCount - resourceClutch.OreCount < 0 ||
                this.Resources.WoolCount - resourceClutch.WoolCount < 0)
            {
                throw new ArithmeticException("No resource count can be negative.");
            }

            this.Resources -= resourceClutch;
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
                if (!string.IsNullOrEmpty(idValue))
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

            if (string.IsNullOrEmpty(this.Name))
            {
                throw new Exception("No name found for player in stream.");
            }
        }

        private ResourceClutch GetResourceForIndex(int index)
        {
            if (index < this.BrickCount)
            {
                this.BrickCount--;
                this.Resources -= ResourceClutch.OneBrick;
                return ResourceClutch.OneBrick;
            }

            if (index < this.BrickCount + this.GrainCount)
            {
                this.GrainCount--;
                this.Resources -= ResourceClutch.OneGrain;
                return ResourceClutch.OneGrain;
            }

            if (index < this.BrickCount + this.GrainCount + this.LumberCount)
            {
                this.LumberCount--;
                this.Resources -= ResourceClutch.OneLumber;
                return ResourceClutch.OneLumber;
            }

            if (index < this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount)
            {
                this.OreCount--;
                this.Resources -= ResourceClutch.OneOre;
                return ResourceClutch.OneOre;
            }

            if (index < this.BrickCount + this.GrainCount + this.LumberCount + this.OreCount + this.WoolCount)
            {
                this.WoolCount--;
                this.Resources -= ResourceClutch.OneWool;
                return ResourceClutch.OneWool;
            }

            throw new NotImplementedException("Should not get here");
        }

        private int GetValueOrZero(XmlReader reader, String attributeName)
        {
            var value = reader.GetAttribute(attributeName);
            return !string.IsNullOrEmpty(value) ? int.Parse(value) : 0;
        }
        #endregion
    }
}
