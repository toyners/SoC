﻿
namespace Service.IntegrationTests
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;

  public class MockClient : IServiceProviderCallback
  {
    public Guid GameToken;

    public Boolean GameJoined;

    public Boolean GameInitialized;

    public UInt32 Id;

    public Boolean PlaceTownMessageReceived;

    public UInt32 TownPlacedRank;

    public UInt32[] TownLocations;

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

    public void WaitUntilClientReceivesPlaceTownMessage()
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      while (!this.PlaceTownMessageReceived && stopWatch.ElapsedMilliseconds <= 2000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      if (!this.PlaceTownMessageReceived)
      {
        throw new TimeoutException("Timed out waiting for client to receive place town message.");
      }
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }
  }
}
