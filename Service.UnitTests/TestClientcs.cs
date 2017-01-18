
namespace Service.UnitTests
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;

  public class TestClient : IServiceProviderCallback
  {
    private static UInt32 NextClientId;

    private static UInt32 NextTownPlacedRank;

    private GameSessionManager gameSessionManager;

    private ConcurrentQueue<MessageBase> messageQueue;

    public TestClient(String userName, GameSessionManager gameSessionManager)
    {
      this.messageQueue = new ConcurrentQueue<MessageBase>();
      this.gameSessionManager = gameSessionManager;
      this.Username = userName;
    }

    public Guid GameToken { get; private set; }

    public String Username { get; private set; }

    public static void SetupBeforeEachTest()
    {
      TestClient.NextTownPlacedRank = 1;
      TestClient.NextClientId = 1;
    }

    public MessageBase GetLastMessage()
    {
      MessageBase message = null;
      while (this.messageQueue.Count > 0)
      {
        while (!this.messageQueue.TryDequeue(out message))
        {
          Thread.Sleep(50);
        }
      }

      return message;
    }

    public void LeaveGame()
    {
      this.gameSessionManager.RemoveClient(this.GameToken, this);
    }

    public Boolean MessageHasType<T>()
    {
      var message = this.Peek();

      return message != null && message.GetType() == typeof(T);
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

    public void ChooseTownLocation()
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameJoined(Guid gameToken)
    {
      this.GameToken = gameToken;
      this.messageQueue.Enqueue(new ConfirmGameJoinedMessage(gameToken));
    }

    public void ConfirmPlayerHasLeftGame()
    {
      this.messageQueue.Enqueue(new PlayerHasLeftGameMessage());
    }

    public void InitializeGame(GameInitializationData gameData)
    {
      this.messageQueue.Enqueue(new InitializeGameMessage(gameData));
    }

    public void PlayerDataForJoiningClient(PlayerData playerData)
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

    public void ConfirmOtherPlayerHasLeftGame(String username)
    {
      this.messageQueue.Enqueue(new OtherPlayerHasLeftGameMessage(username));
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

    public class PlayerHasLeftGameMessage : MessageBase
    {
    }

    public class OtherPlayerHasLeftGameMessage : MessageBase
    {
      public OtherPlayerHasLeftGameMessage(String userName)
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