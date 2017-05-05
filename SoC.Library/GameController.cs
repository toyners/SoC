
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Interfaces;

  public class GameController : IGameController
  {
    public Guid GameId { get; private set; }

    public void AcceptOffer(Offer offer)
    {
      throw new NotImplementedException();
    }

    public void BuildRoad(Location startingLocation, Location finishingLocation)
    {
      throw new NotImplementedException();
    }

    public DevelopmentCard BuyDevelopmentCard()
    {
      throw new NotImplementedException();
    }

    public ICollection<Offer> MakeOffer(Offer offer)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(Location location)
    {
      throw new NotImplementedException();
    }

    public Boolean RequestConnectionToGame()
    {
      throw new NotImplementedException();
    }

    public ResourceTypes TradeResourcesAtPort(Location location)
    {
      throw new NotImplementedException();
    }

    public ResourceTypes TradeResourcesWithBank()
    {
      throw new NotImplementedException();
    }

    public void UpgradeToCity(Location location)
    {
      throw new NotImplementedException();
    }
  }
}
