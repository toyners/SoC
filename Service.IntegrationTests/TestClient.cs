
namespace Service.IntegrationTests
{
  using System;
  using Jabberwocky.SoC.Service;

  public class TestClient : IServiceProviderCallback
  {
    public Guid GameToken;

    public Boolean GameJoined;

    public void ConfirmGameJoined(Guid gameToken)
    {
      this.GameToken = gameToken;
      this.GameJoined = true;
    }

    public void ConfirmGameLeft()
    {
      throw new NotImplementedException();
    }

    public void GameInitialization(GameInitializationData gameData)
    {
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }
  }
}
