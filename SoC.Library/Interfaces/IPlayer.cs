
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;

  public interface IPlayer
  {
    #region Properties
    Int32 BrickCount { get; }
    Int32 GrainCount { get; }
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
    PlayerDataView GetDataView();
    void AddResources(ResourceClutch resourceClutch);
    void RemoveResources(ResourceClutch resourceClutch);
    void PayForDevelopmentCard();
    void PlaceCity();
    void PlaceRoadSegment();
    void PlaceSettlement();
    void PlaceStartingInfrastructure();
    #endregion
  }
}
