
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;

  public class MockClient : IServiceProviderCallback
  {
    #region Fields
    public const Int64 TimeOut = 2000;

    public Boolean ChooseTownLocationMessageReceived;

    public Boolean GameJoined;

    public Boolean GameInitialized;

    public Guid GameToken;

    public UInt32 Id;

    public UInt32 NewTownLocation;

    public List<PlayerData> ReceivedPlayerData;

    public UInt32 TownPlacedRank;

    public String Username;
    
    private static UInt32 NextTownPlacedRank;

    private static UInt32 NextClientId;

    private GameSessionManager gameSessionManager;
    #endregion

    #region Construction
    public MockClient()
    {
      this.Id = MockClient.NextClientId++;
      this.ReceivedPlayerData = new List<PlayerData>();
    }

    public MockClient(GameSessionManager gameSessionManager) : this()
    {
      this.gameSessionManager = gameSessionManager;
    }
    #endregion

    #region Methods
    public static void SetupBeforeEachTest()
    {
      MockClient.NextTownPlacedRank = 1;
      MockClient.NextClientId = 1;
    }

    public void ChooseTownLocation()
    {
      this.ChooseTownLocationMessageReceived = true;
      this.TownPlacedRank = MockClient.NextTownPlacedRank++;
    }

    public void ConfirmGameInitialized()
    {
      this.gameSessionManager.ConfirmGameInitialized(this.GameToken, this);
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

    public void PlaceTown(UInt32 locationIndex)
    {
      this.gameSessionManager.ConfirmTownPlacement(this.GameToken, this, locationIndex);
    }

    public void PlayerDataForJoiningClient(PlayerData playerData)
    {
      this.ReceivedPlayerData.Add(playerData);
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }

    public void TownPlacedDuringSetup(UInt32 locationIndex)
    {
      this.NewTownLocation = locationIndex;
    }

    public void WaitUntilClientReceivesPlaceTownMessage()
    {
      this.ChooseTownLocationMessageReceived = false;
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      while (!this.ChooseTownLocationMessageReceived
#if !DEBUG
        && stopWatch.ElapsedMilliseconds <= MockClient.TimeOut
#endif
        )
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      if (!this.ChooseTownLocationMessageReceived)
      {
        throw new TimeoutException("Timed out waiting for client to receive place town message.");
      }
    }
    #endregion
  }
}
