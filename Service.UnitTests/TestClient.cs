
namespace Service.UnitTests
{
  using System;
  using System.Collections.Concurrent;
  using Jabberwocky.SoC.Service;
  using Messages;

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
        return null;
      }

      var messages = this.messageQueue.ToArray();
      return messages[messages.Length - 1];
    }

    public void JoinGame()
    {
      this.gameSessionManager.AddPlayer(this, this.Username);
    }

    public void LeaveGame()
    {
      this.gameSessionManager.RemoveClient(this.GameToken, this);
    }

    public void ChooseTownLocation()
    {
      this.messageQueue.Enqueue(new PlaceTownMessage());
    }

    public void ConfirmGameInitialized()
    {
      this.gameSessionManager.ConfirmGameInitialized(this.GameToken, this);
    }

    public void ConfirmGameSessionJoined(Guid gameToken, GameSessionManager.GameStates gameState)
    {
      this.GameToken = gameToken;
      this.messageQueue.Enqueue(new ConfirmGameJoinedMessage(gameToken, gameState));
    }

    public void ConfirmGameSessionReadyToLaunch()
    {
      this.messageQueue.Enqueue(new GameSessionReadyToLaunchMessage());
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

    public void ReceivePersonalMessage(String sender, String text)
    {
      this.messageQueue.Enqueue(new PersonalMessage(sender, text));
    }

    public void SendLaunchGameMessage()
    {
      this.gameSessionManager.LaunchGame(this.GameToken, this);
    }

    public void SendPersonalMessage(String messageText)
    {
      this.gameSessionManager.ProcessPersonalMessage(this.GameToken, this, messageText);
    }

    public void SendTownLocation(UInt32 positionIndex)
    {
      this.gameSessionManager.ConfirmTownPlacement(this.GameToken, this, positionIndex);
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
  }
}