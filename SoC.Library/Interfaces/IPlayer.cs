﻿
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IPlayer
    {
        #region Properties
        int CitiesBuilt { get; }
        bool HasLargestArmy { get; set; }
        bool HasLongestRoad { get; set; }
        List<DevelopmentCard> HeldCards { get; }
        Guid Id { get; }
        bool IsComputer { get; }
        uint KnightCards { get; }
        string Name { get; }
        List<DevelopmentCard> PlayedCards { get; }
        int RemainingCities { get; }
        int RemainingRoadSegments { get; }
        int RemainingSettlements { get; }
        ResourceClutch Resources { get; }
        int RoadSegmentsBuilt { get; }
        int SettlementsBuilt { get; }
        uint VictoryPoints { get; }

        // TODO: Obsolete properties
        int BrickCount { get; }
        int GrainCount { get; }
        int LumberCount { get; }
        int OreCount { get; }
        int WoolCount { get; }
        int ResourcesCount { get; }
        #endregion

        #region Methods
        void AddResources(ResourceClutch resourceClutch);
        PlayerDataModel GetDataView();
        void PayForDevelopmentCard();
        void PlaceCity();
        void PlaceKnightDevelopmentCard();
        void PlaceRoadSegment();
        void PlaceSettlement();
        void PlaceStartingInfrastructure();
        void RemoveResources(ResourceClutch resourceClutch);
        ResourceClutch LoseResourceAtIndex(int resourceIndex);
        ResourceClutch LoseResourcesOfType(ResourceTypes resourceType);
        #endregion
    }
}
