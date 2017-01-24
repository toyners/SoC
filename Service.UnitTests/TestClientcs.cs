﻿
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
    #region Fields
    private static UInt32 NextClientId;

    private static UInt32 NextTownPlacedRank;

    private GameSessionManager gameSessionManager;

    private ConcurrentQueue<MessageBase> messageQueue;
    #endregion

    #region Construction
    public TestClient(String userName, GameSessionManager gameSessionManager)
    {
      this.messageQueue = new ConcurrentQueue<MessageBase>();
      this.gameSessionManager = gameSessionManager;
      this.Username = userName;
    }
    #endregion

    #region Properties
    public Guid GameToken { get; private set; }

    public String Username { get; private set; }
    #endregion

    #region Methods
    public static void SetupBeforeEachTest()
    {
      TestClient.NextTownPlacedRank = 1;
      TestClient.NextClientId = 1;
    }

    /// <summary>
    /// Gets the first message from test client. Is not thread safe.
    /// </summary>
    /// <returns>First message that the client received.</returns>
    public MessageBase GetFirstMessage()
    {
      if (this.messageQueue.IsEmpty)
      {
        throw new Exception("Message queue is empty.");
      }

      var messages = this.messageQueue.ToArray();
      return messages[0];
    }

    /// <summary>
    /// Gets the last message from the test client. Is not thread safe.
    /// </summary>
    /// <returns>Most current message that the client received.</returns>
    public MessageBase GetLastMessage()
    {
      if (this.messageQueue.IsEmpty)
      {
        throw new Exception("Message queue is empty.");
      }

      var messages = this.messageQueue.ToArray();
      return messages[messages.Length - 1];
    }

    public void LeaveGame()
    {
      this.gameSessionManager.RemoveClient(this.GameToken, this);
    }

    public void ChooseTownLocation()
    {
      throw new NotImplementedException();
    }

    public void ConfirmGameJoined(Guid gameToken, GameSessionManager.GameStates gameState)
    {
      this.GameToken = gameToken;
      this.messageQueue.Enqueue(new ConfirmGameJoinedMessage(gameToken, gameState));
    }

    public void ConfirmOtherPlayerHasLeftGame(String username)
    {
      this.messageQueue.Enqueue(new OtherPlayerHasLeftGameMessage(username));
    }

    public void ConfirmPlayerHasLeftGame()
    {
      this.messageQueue.Enqueue(new PlayerHasLeftGameMessage());
    }

    public void ContainMessagesInOrder(int startingIndex, params MessageBase[] expectedMessages)
    {
      var messages = this.messageQueue.ToArray();
      var index = startingIndex;

      foreach (var expectedMessage in expectedMessages)
      {
        var message = messages[index++];

        if (!message.IsSameAs(expectedMessage))
        {
          var exceptionMessage = String.Format("Message at index {0} is not the same as expected message. Expected {1} but found {2}", index, expectedMessage, message);
          throw new Exception(exceptionMessage);
        }
      }
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
    #endregion

    #region Classes
    public class MessageBase
    {
      public String MessageText { get; protected set; }

      public virtual Boolean IsSameAs(MessageBase messageBase)
      {
        if (this.Equals(messageBase))
        {
          throw new Exception("Same Object");
        }

        return (this.GetType() == messageBase.GetType() && String.CompareOrdinal(this.MessageText, messageBase.MessageText) == 0);
      }
    }

    public class ConfirmGameJoinedMessage : MessageBase
    {
      public ConfirmGameJoinedMessage(Guid gameToken, GameSessionManager.GameStates gameState)
      {
        this.GameToken = gameToken;
        this.GameState = gameState;
      }

      public Object GameState { get; internal set; }
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

      public override Boolean IsSameAs(MessageBase messageBase)
      {
        if (!base.IsSameAs(messageBase))
        {
          return false;
        }

        var playerDataReceivedMessage = (PlayerDataReceivedMessage)messageBase;
        return this.PlayerData.IsAnonymous == playerDataReceivedMessage.PlayerData.IsAnonymous &&
          this.PlayerData.Username == playerDataReceivedMessage.PlayerData.Username;
      }

      public override String ToString()
      {
        return String.Format("{0}, IsAnonymous: {1}, Username: {2}", this.GetType(), this.PlayerData.IsAnonymous, this.PlayerData.Username);
      }
    }

    public class InitializeGameMessage : MessageBase
    {
      public InitializeGameMessage(GameInitializationData gameData)
      {
        this.GameData = gameData;
      }

      public GameInitializationData GameData { get; private set; }
    }

    public class GameSessionLaunchMessage : MessageBase
    {
    }
    #endregion
  }
}