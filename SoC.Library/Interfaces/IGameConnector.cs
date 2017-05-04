
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public interface IGameController
  {
    Guid GameId { get; }

    Boolean RequestConnectionToGame();

    void PlaceTown(Location location);

    void UpgradeToCity(Location location);

    void BuildRoad(Location startingLocation, Location finishingLocation);

    DevelopmentCard BuyDevelopmentCard();

    ResourceTypes TradeResourcesWithBank();

    void TradeResourcesAtPort(Location location);

    ICollection<Offer> MakeOffer(Offer offer);

    void AcceptOffer(Offer offer);
  }

  public struct Offer
  {
    Player player;

    OfferComponent? OfferedOre;
    OfferComponent? OfferedWheat;
    OfferComponent? OfferedSheep;
    OfferComponent? OfferedLumber;
    OfferComponent? OfferedBrick;

    OfferComponent? WantedOreCount;
    OfferComponent? WantedWheatCount;
    OfferComponent? WantedSheepCount;
    OfferComponent? WantedLumberCount;
    OfferComponent? WantedBrickCount;
  }

  public struct OfferComponent
  {
    ResourceTypes ResourceType;
    UInt32? Number;
  }
}
