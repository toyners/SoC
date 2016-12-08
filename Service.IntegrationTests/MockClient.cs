
namespace Service.IntegrationTests
{
  using System;
  using Jabberwocky.SoC.Service;

  public class MockClient : IServiceProviderCallback
  {
    public Guid GameToken;

    public Boolean GameJoined;

    public Boolean GameInitialized;

    public UInt32 Id;

    public Boolean PlaceTownMessageReceived;

    public UInt32 TownPlacedRank;

    private static UInt32 NextTownPlacedRank;

    private static UInt32 NextClientId;

    public MockClient()
    {
      this.Id = MockClient.NextClientId++;
    }

    public static void SetupBeforeEachTest()
    {
      MockClient.NextTownPlacedRank = 1;
      MockClient.NextClientId = 1;
    }

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
      this.PlaceTownMessageReceived = true;
      this.TownPlacedRank = MockClient.NextTownPlacedRank++;
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }
  }
}
