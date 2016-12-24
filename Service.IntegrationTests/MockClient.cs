
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;

  public class MockClient : IServiceProviderCallback
  {
    private const UInt32 NotSet = UInt32.MaxValue;

    public Boolean ChooseTownLocationMessageReceived;

    public Boolean GameJoined;

    public Boolean GameInitialized;

    public Guid GameToken;

    public UInt32 Id;

    public UInt32 NewTownLocation;

    public List<UInt32> SelectedTownLocations;

    public UInt32 TownPlacedRank;

    private static UInt32 NextTownPlacedRank;

    private static UInt32 NextClientId;

    private GameSessionManager gameSessionManager;

    public MockClient()
    {
      this.Id = MockClient.NextClientId++;
      this.NewTownLocation = MockClient.NotSet;
    }

    public MockClient(GameSessionManager gameSessionManager) : this()
    {
      this.gameSessionManager = gameSessionManager;
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

    public void ChooseTownLocation(List<UInt32> selectedTownLocations)
    {
      this.ChooseTownLocationMessageReceived = true;
      this.TownPlacedRank = MockClient.NextTownPlacedRank++;
      this.SelectedTownLocations = selectedTownLocations;
    }

    public void WaitUntilClientReceivesPlaceTownMessage()
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      while (!this.ChooseTownLocationMessageReceived && stopWatch.ElapsedMilliseconds <= 2000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      if (!this.ChooseTownLocationMessageReceived)
      {
        throw new TimeoutException("Timed out waiting for client to receive place town message.");
      }
    }

    public void ResetForNextTownPlacement()
    {
      this.SelectedTownLocations = null;
      this.NewTownLocation = MockClient.NotSet;
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(UInt32 locationIndex)
    {
      this.gameSessionManager.ConfirmTownPlacement(this.GameToken, this, locationIndex);
    }

    public void TownPlacedDuringSetup(UInt32 locationIndex)
    {
      if (locationIndex == MockClient.NotSet)
      {
        throw new ArgumentException("Location index is invalid.");
      }

      this.NewTownLocation = locationIndex;
    }
  }
}
