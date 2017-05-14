using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Jabberwocky.SoC.Client.ServiceReference;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;

namespace Jabberwocky.SoC.Client
{
  public class RemoteGameController : IGameController, IServiceProviderCallback
  {
    #region Fields
    private ServiceProviderClient serviceProviderClient;
    #endregion

    public Guid GameId
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Action<PlayerBase[]> GameJoinedEvent { get; set; }

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

    public void ConfirmGameJoined(Guid gameToken)
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameLeft()
    {
      throw new NotImplementedException();
    }

    public void InitializeGame(GameInitializationData gameData)
    {
      throw new NotImplementedException();
    }

    public ICollection<Offer> MakeOffer(Offer offer)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown()
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(Location location)
    {
      throw new NotImplementedException();
    }

    public void StartJoiningGame(GameFilter gameFilter)
    {
      var instanceContext = new InstanceContext(this);
      //this.serviceProviderClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      var binding = new WSDualHttpBinding();
      var uri = new Uri("http://localhost:8733/Design_Time_Addresses/Jabberwocky.SoC.Service/IServiceProvider/");
      var endPointAddress = new EndpointAddress(uri);
      this.serviceProviderClient = new ServiceProviderClient(instanceContext, binding, endPointAddress);
      this.serviceProviderClient.TryJoinGame();
    }

    public void StartJoiningGame(GameFilter gameFilter, Guid accountToken)
    {
      throw new NotImplementedException();
    }

    public void StartLogIntoAccount(String username, String password)
    {
      throw new NotImplementedException();
    }

    public void StartTurn(Guid token)
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
