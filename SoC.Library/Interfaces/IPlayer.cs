
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.Enums;
    using Jabberwocky.SoC.Library.PlayerData;

    public interface IPlayer
    {
        #region Properties
        bool CanBuyDevelopmentCard { get; }
        PlayerPlacementVerificationStates CanPlaceCity { get; }
        PlayerPlacementVerificationStates CanPlaceRoadSegment { get; }
        PlayerPlacementVerificationStates CanPlaceSettlement { get; }
        int PlacedCities { get; }
        bool HasLargestArmy { get; set; }
        bool HasLongestRoad { get; set; }
        List<DevelopmentCard> HeldCards { get; }
        Guid Id { get; }
        bool IsComputer { get; }
        int PlayedKnightCards { get; }
        string Name { get; }
        List<DevelopmentCard> PlayedCards { get; }
        int RemainingCities { get; }
        int RemainingRoadSegments { get; }
        int RemainingSettlements { get; }
        ResourceClutch Resources { get; }
        int PlacedRoadSegments { get; }
        int PlacedSettlements { get; }
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
        PlayerDataBase GetDataModel(bool provideFullPlayerData);
        void BuyDevelopmentCard();
        void PlaceCity();
        void PlaceRoadSegment();
        void PlaceSettlement();
        void PlaceStartingInfrastructure();
        void PlayDevelopmentCard(DevelopmentCard card);
        void RemoveResources(ResourceClutch resourceClutch);
        ResourceClutch LoseResourceAtIndex(int resourceIndex);
        ResourceClutch LoseResourcesOfType(ResourceTypes resourceType);
        #endregion
    }
}
