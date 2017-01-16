
namespace Service.UnitTests
{
  using System;
  using System.Collections.Concurrent;
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

    public virtual void ConfirmGameJoined(Guid gameToken)
    {
      this.GameToken = gameToken;
      this.GameJoined = true;
    }

    public virtual void ConfirmGameLeft()
    {
      throw new NotImplementedException();
    }

    public virtual void InitializeGame(GameInitializationData gameData)
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

    public virtual void PlayerDataForJoiningClient(PlayerData playerData)
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

  public class TestClient : MockClient, IServiceProviderCallback
  {
    private ConcurrentQueue<MessageBase> messageQueue = new ConcurrentQueue<MessageBase>();
    
    public MessageBase LastMessage
    {
      get
      {
        MessageBase message = null;
        while(!this.messageQueue.TryPeek(out message))
        {
          Thread.Sleep(50);
        }

        return message;
      }
    }

    public TestClient(String userName, GameSessionManager gameSessionManager) : base(gameSessionManager)
    {
      this.Username = userName;
    }

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

    public override void ConfirmGameJoined(Guid gameToken)
    {
      this.GameToken = gameToken;
      this.messageQueue.Enqueue(new ConfirmGameJoinedMessage(gameToken));
    }

    public override void ConfirmGameLeft()
    {
      this.messageQueue.Enqueue(new ConfirmGameLeftMessage());
    }

    public override void InitializeGame(GameInitializationData gameData)
    {
      this.messageQueue.Enqueue(new InitializeGameMessage(gameData));
    }

    public override void PlayerDataForJoiningClient(PlayerData playerData)
    {
      this.messageQueue.Enqueue(new PlayerDataReceivedMessage(playerData));
    }

    public void StartTurn(Guid token)
    {
      throw new NotImplementedException();
    }

    public void TownPlacedDuringSetup(UInt32 locationIndex)
    {
      throw new NotImplementedException();
    }

    public class MessageBase
    {
      public String MessageText { get; protected set; }
    }

    public class ConfirmGameJoinedMessage : MessageBase
    {
      public ConfirmGameJoinedMessage() : this(Guid.Empty) { }
      public ConfirmGameJoinedMessage(Guid gameToken) { this.GameToken = gameToken; }

      public Guid GameToken { get; private set; }
    }

    public class ConfirmGameLeftMessage : MessageBase
    {
    }

    public class ClientLeftMessage : MessageBase
    {
      public ClientLeftMessage(String userName)
      {
        this.MessageText = userName + " has left the game.";
      }
    }

    public class PlayerDataReceivedMessage : MessageBase
    {
      public PlayerDataReceivedMessage(PlayerData playerData)
      {
        this.PlayerData = playerData;
      }

      public PlayerData PlayerData { get; private set; }
    }

    public class InitializeGameMessage : MessageBase
    {
      public InitializeGameMessage(GameInitializationData gameData)
      {
        this.GameData = gameData;
      }

      public GameInitializationData GameData { get; private set; }
    }
  }
}