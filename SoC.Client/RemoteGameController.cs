using System;
using System.Collections.Generic;
using System.ServiceModel;
using Jabberwocky.SoC.Client.ServiceReference;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameBoards;
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

    #region Events
    public Action<GameBoardUpdate> BoardUpdatedEvent { get; set; }

    public Action<PlayerDataView[]> GameJoinedEvent { get; set; }

    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }

    public Action<ClientAccount> LoggedInEvent { get; set; }

    public Action<GameBoardUpdate> StartInitialSetupTurnEvent { get; set; }
    #endregion

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

    public void ChooseTownLocation()
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameIsOver()
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

    public void ConfirmGameSessionJoined(Guid gameToken, GameSessionManagerGameStates gameSession)
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameSessionReadyToLaunch()
    {
      this.GameJoinedEvent?.Invoke(new[] { new Library.PlayerDataView() });
    }

    public void ConfirmOtherPlayerHasLeftGame(String username)
    {
      throw new NotImplementedException();
    }

    public void ConfirmPlayerHasLeftGame()
    {
      throw new NotImplementedException();
    }

    public void GameJoined()
    {
      this.GameJoinedEvent?.Invoke(new[] { new Library.PlayerDataView() });
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

    public void PlayerDataForJoiningClient(ServiceReference.PlayerData playerData)
    {
      throw new NotImplementedException();
    }

    public void Quit()
    {
      throw new NotImplementedException();
    }

    public void ReceivePersonalMessage(String sender, String text)
    {
      throw new NotImplementedException();
    }

    public void JoinGame(GameOptions gameOptions)
    {
      var instanceContext = new InstanceContext(this);
      //this.serviceProviderClient = new ServiceProviderClient(instanceContext, "WSDualHttpBinding_IServiceProvider");
      var binding = new WSDualHttpBinding();
      var uri = new Uri("http://localhost:8733/Design_Time_Addresses/Jabberwocky.SoC.Service/IServiceProvider/");
      var endPointAddress = new EndpointAddress(uri);
      this.serviceProviderClient = new ServiceProviderClient(instanceContext, binding, endPointAddress);
      this.serviceProviderClient.TryJoinGameNew();
    }

    public void StartJoiningGame(GameOptions gameFilter, Guid accountToken)
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

    public void TownPlacedDuringSetup(UInt32 locationIndex)
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
