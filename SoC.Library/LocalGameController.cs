
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

    public void StartJoiningGame(GameOptions gameOptions)
    {
      if (gameOptions == null)
      {
        gameOptions = new GameOptions { MaxPlayers = 1, MaxAIPlayers = 3 };
      }

      var players = new PlayerBase[gameOptions.MaxPlayers + gameOptions.MaxAIPlayers];

      var index = 0;
      while ((gameOptions.MaxPlayers--) > 0)
      {
        players[index++] = new PlayerData();
      }

      while ((gameOptions.MaxAIPlayers--) > 0)
      {
        players[index++] = new PlayerView();
      }

      this.GameJoinedEvent?.Invoke(players);
    }

    public void StartJoiningGame(GameOptions gameOptions, Guid accountToken)
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
