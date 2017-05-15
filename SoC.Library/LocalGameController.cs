
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Interfaces;

  public class LocalGameController : IGameController
  {
    public Action<PlayerBase[]> GameJoinedEvent { get; set; }

    public Guid GameId { get; private set; }

    public Action<ClientAccount> LoggedInEvent { get; set; }

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

    public void StartJoiningGame(GameOptions gameFilter)
    {
      if (gameFilter == null)
      {
        gameFilter = new GameOptions { MaxPlayers = 1, MaxAIPlayers = 3 };
      }

      var players = new PlayerBase[gameFilter.MaxPlayers + gameFilter.MaxAIPlayers];

      var index = 0;
      while ((gameFilter.MaxPlayers--) > 0)
      {
        players[index++] = new PlayerData();
      }

      while ((gameFilter.MaxAIPlayers--) > 0)
      {
        players[index++] = new PlayerView();
      }

      this.GameJoinedEvent?.Invoke(players);
    }

    public void StartJoiningGame(GameOptions gameFilter, Guid accountToken)
    {
      throw new NotImplementedException();
    }

    public void StartLogIntoAccount(String username, String password)
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
