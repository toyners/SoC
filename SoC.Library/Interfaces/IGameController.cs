
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;

  public interface IGameController
  {
    Guid GameId { get; }

    Boolean RequestConnectionToGame();

    void PlaceTown(Location location);

    void UpgradeToCity(Location location);

    void BuildRoad(Location startingLocation, Location finishingLocation);

    DevelopmentCard BuyDevelopmentCard();

    ResourceTypes TradeResourcesWithBank();

    ResourceTypes TradeResourcesAtPort(Location location);

    ICollection<Offer> MakeOffer(Offer offer);

    void AcceptOffer(Offer offer);
  }

  public struct Offer
  {
    Player player;

    Int32 OfferedOre;
    Int32 OfferedWheat;
    Int32 OfferedSheep;
    Int32 OfferedLumber;
    Int32 OfferedBrick;

    Int32 WantedOreCount;
    Int32 WantedWheatCount;
    Int32 WantedSheepCount;
    Int32 WantedLumberCount;
    Int32 WantedBrickCount;
  }
}
