
namespace Service.IntegrationTests
{
  using System;
  using Jabberwocky.SoC.Service;

  public class MockClient : IServiceProviderCallback
  {
    public Guid GameToken;

    public Boolean GameJoined;

    public Boolean GameInitialized;

    public Boolean TownPlaced;

    public void ConfirmGameJoined(Guid gameToken)
    {
      this.GameToken = gameToken;
      this.GameJoined = true;
    }

    public void ConfirmGameLeft()
    {
      throw new NotImplementedException();
    }

    public void InitializeGame(GameInitializationData gameData)
    {
      this.GameInitialized = true;
    }

    public void PlaceTown()
    {
      this.TownPlaced = true;
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }
  }
}
