
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;

  public interface IPlayer
  {
    #region Properties
    Int32 BrickCount { get; }
    Int32 GrainCount { get; }
    UInt32 KnightCards { get; }
    Int32 LumberCount { get; }
    Int32 OreCount { get; }
    Int32 WoolCount { get; }
    Guid Id { get; }
    String Name { get; }
    Int32 ResourcesCount { get; }
    Boolean IsComputer { get; }
    UInt32 VictoryPoints { get; }
    Int32 RemainingRoadSegments { get; }
    Int32 RoadSegmentsBuilt { get; }
    Int32 RemainingSettlements { get; }
    Int32 RemainingCities { get; }
    #endregion

    #region Methods
    void AddResources(ResourceClutch resourceClutch);
    PlayerDataView GetDataView();
    void PayForDevelopmentCard();
    void PlaceCity();
    void PlaceKnightDevelopmentCard();
    void PlaceRoadSegment();
    void PlaceSettlement();
    void PlaceStartingInfrastructure();
    void RemoveResources(ResourceClutch resourceClutch);
    ResourceClutch LoseResourceAtIndex(Int32 resourceIndex);
    ResourceClutch LoseResourcesOfType(ResourceTypes resourceType);
    #endregion
  }
}
