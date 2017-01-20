
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;

  public class MockClient3 : IServiceProviderCallback
  {
    #region Fields
    public const Int64 TimeOut = 2000;

    public Boolean ChooseTownLocationMessageReceived;

    public Boolean GameJoined;

    public Boolean GameInitialized;

    public Guid GameToken;

    public Boolean GameLeft;

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
    public MockClient3()
    {
      this.Id = MockClient3.NextClientId++;
      this.ReceivedPlayerData = new List<PlayerData>();
    }

    public MockClient3(GameSessionManager gameSessionManager) : this()
    {
      this.gameSessionManager = gameSessionManager;
    }
    #endregion

    #region Methods
    public static void SetupBeforeEachTest()
    {
      MockClient3.NextTownPlacedRank = 1;
      MockClient3.NextClientId = 1;
    }

    public void ChooseTownLocation()
    {
      this.ChooseTownLocationMessageReceived = true;
      this.TownPlacedRank = MockClient3.NextTownPlacedRank++;
    }

    public void ConfirmGameInitialized()
    {
      this.gameSessionManager.ConfirmGameInitialized(this.GameToken, this);
    }

    public virtual void ConfirmGameJoined(Guid gameToken, GameSessionManager.GameStates gameState)
    {
      this.GameToken = gameToken;
      this.GameJoined = true;
    }

    public void ConfirmPlayerHasLeftGame()
    {
      throw new NotImplementedException();
    }

    public void InitializeGame(GameInitializationData gameData)
    {
      this.GameInitialized = true;
    }

    public void LeaveGame()
    {
      this.gameSessionManager.RemoveClient(this.GameToken, this);
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

    public void ConfirmOtherPlayerHasLeftGame(String username)
    {
      throw new NotImplementedException();
    }
    #endregion
  }

  public class MockClient : MockClient3, IServiceProviderCallback
  {
    private ConcurrentQueue<MessageBase> messageQueue = new ConcurrentQueue<MessageBase>();

    public MessageBase Peek()
    {
      MessageBase message = null;
      if (this.messageQueue.TryPeek(out message))
      {
        return message;
      }

      return null;
    }

    public Boolean MessageHasType<T>()
    {
      var message = this.Peek();

      return message != null && message.GetType() == typeof(T);
    }

    public void ChooseTownLocation()
    {
      throw new NotImplementedException();
    }

    public override void ConfirmGameJoined(Guid gameToken, GameSessionManager.GameStates gameState)
    {
      this.GameToken = gameToken;
      this.messageQueue.Enqueue(new ConfirmGameJoinedMessage(gameToken));
    }

    public void ConfirmPlayerHasLeftGame()
    {
      throw new NotImplementedException();
    }

    public void InitializeGame(GameInitializationData gameData)
    {
      throw new NotImplementedException();
    }

    public void PlayerDataForJoiningClient(PlayerData playerData)
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

    public abstract class MessageBase { }

    public class ConfirmGameJoinedMessage : MessageBase
    {
      public ConfirmGameJoinedMessage() : this(Guid.Empty) { }
      public ConfirmGameJoinedMessage(Guid gameToken) { this.GameToken = gameToken; }

      public Guid GameToken { get; private set; }
    }
  }
}
