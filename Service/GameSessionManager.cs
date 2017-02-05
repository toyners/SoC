
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;
  using Toolkit.Object;

  public class GameSessionManager
  {
    #region Enums
    public enum States
    {
      Stopped,
      Stopping,
      Running
    }

    public enum GameStates
    {
      Lobby,
      Setup,
      Playing
    }
    #endregion

    #region Fields
    private List<IServiceProviderCallback> clients;
    private Dictionary<Guid, GameSession> gameSessions;
    private IPlayerCardRepository playerCardRepository;
    private ConcurrentQueue<AddPlayerMessage> waitingForGameSessionQueue;
    private Task matchingTask;
    private UInt32 maximumPlayerCount;
    private IGameManagerFactory gameManagerFactory;
    private CancellationTokenSource cancellationTokenSource;
    #endregion

    #region Construction
    public GameSessionManager(IGameManagerFactory gameManagerFactory, UInt32 maximumPlayerCount, IPlayerCardRepository playerCardRepository)
    {
      // TODO: Null reference checks
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGameSessionQueue = new ConcurrentQueue<AddPlayerMessage>();
      this.gameSessions = new Dictionary<Guid, GameSession>();
      this.maximumPlayerCount = maximumPlayerCount;
      this.cancellationTokenSource = new CancellationTokenSource();
      this.gameManagerFactory = gameManagerFactory;

      playerCardRepository.VerifyThatObjectIsNotNull("Parameter 'playerCardRepository' is null.");
      this.playerCardRepository = playerCardRepository;

      this.State = States.Stopped;
    }
    #endregion

    #region Properties
    public States State { get; private set; }
    #endregion

    #region Methods
    public void AddPlayer(IServiceProviderCallback client, String username)
    {
      // TODO: Check for null reference
      this.waitingForGameSessionQueue.Enqueue(new AddPlayerMessage(client, username));
    }

    public void ConfirmGameInitialized(Guid gameToken, IServiceProviderCallback client)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.ConfirmGameInitialized(client);
    }

    public void ConfirmTownPlacement(Guid gameToken, IServiceProviderCallback client, UInt32 positionIndex)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.ConfirmTownPlacement(client, positionIndex);
    }

    /*public States GameSessionState(Guid gameToken)
    {
      var gameSession = this.GetGameSession(gameToken);
      return gameSession.State;
    }*/

    public void ProcessPersonalMessage(Guid gameToken, IServiceProviderCallback client, String text)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.ProcessPersonalMessage(client, text);
    }

    public void RemoveClient(Guid gameToken, IServiceProviderCallback client)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.RemoveClient(client);
    }

    public void LaunchGame(Guid gameToken, IServiceProviderCallback client)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.LaunchGame(client);
    }

    public void Stop()
    {
      if (this.State != States.Running)
      {
        return;
      }

      this.cancellationTokenSource.Cancel();
    }

    public void Start()
    {
      if (this.State != States.Stopped)
      {
        return;
      }

      var cancellationToken = this.cancellationTokenSource.Token;

      this.matchingTask = Task.Factory.StartNew(() => { this.MatchPlayersWithGames(cancellationToken); });
    }

    private void AddToNewGameSession(AddPlayerMessage addPlayerMessage, CancellationToken cancellationToken)
    {
      var gameSession = new GameSession(this.gameManagerFactory.Create(), this.maximumPlayerCount, this.playerCardRepository, cancellationToken);
      gameSession.Start();
      gameSession.AddPlayer(addPlayerMessage);
      this.gameSessions.Add(gameSession.GameToken, gameSession);
    }

    private GameSession GetGameSession(Guid gameToken)
    {
      if (!this.gameSessions.ContainsKey(gameToken))
      {
        var message = "Can't find game session matching game session token " + gameToken;
        NLog.LogManager.GetCurrentClassLogger().Error(message);
        throw new ArgumentException(message);
      }

      return this.gameSessions[gameToken];
    }

    private void MatchPlayersWithGames(CancellationToken cancellationToken)
    {
      try
      {
        this.State = States.Running;
        while (true)
        {
          while (this.waitingForGameSessionQueue.IsEmpty)
          {
            cancellationToken.ThrowIfCancellationRequested();
            Thread.Sleep(500);
          }

          AddPlayerMessage addPlayerMessage;
          var gotClient = this.waitingForGameSessionQueue.TryDequeue(out addPlayerMessage);
          if (!gotClient)
          {
            // Couldn't get the client from the queue (probably because another thread got it).
            continue;
          }

          if (!this.TryAddToExistingGameSession(addPlayerMessage))
          {
            this.AddToNewGameSession(addPlayerMessage, cancellationToken);
          }
        }
      }
      catch (OperationCanceledException)
      {
        // Shutting down - ignore exception
        this.State = States.Stopping;
        foreach (var gameSession in this.gameSessions.Values)
        {
          while (gameSession.State != States.Stopped)
          {
            Thread.Sleep(50);
          }
        }
      }
      finally
      {
        this.State = States.Stopped;
      }
    }

    private Boolean TryAddToExistingGameSession(AddPlayerMessage addPlayerMessage)
    {
      GameSession gameSession = null;
      var gotBusyGameSession = false;
      do
      {
        gotBusyGameSession = false;
        foreach (var kv in this.gameSessions)
        {
          gameSession = kv.Value;
          if (gameSession.GameSessionState == GameSession.GameSessionStates.AwaitingPlayer)
          {
            gameSession.AddPlayer(addPlayerMessage);
            return true;
          }

          if (gameSession.GameSessionState == GameSession.GameSessionStates.AddingPlayer)
          {
            gotBusyGameSession = true;
          }
        }

        Thread.Sleep(50);

      } while (gotBusyGameSession);

      return false;
    }
    #endregion

    #region Classes
    private class GameSession
    {
      public enum GameSessionStates
      {
        AddingPlayer = 1,
        AwaitingPlayer,
        Full,
      }

      #region Fields
      private IGameManager gameManager;

      public Guid GameToken;

      private CancellationToken cancellationToken;
      private UInt32 currentPlayerCount;
      private IServiceProviderCallback[] clients;
      private IPlayerCardRepository playerCardRepository;
      private Dictionary<IServiceProviderCallback, PlayerData> playerCards;
      private Task gameTask;
      private MessagePump messagePump;
      #endregion

      #region Construction
      public GameSession(IGameManager gameManager, UInt32 maxPlayerCount, IPlayerCardRepository playerCardRepository, CancellationToken cancellationToken)
      {
        this.GameToken = Guid.NewGuid();
        this.gameManager = gameManager;
        this.clients = new IServiceProviderCallback[maxPlayerCount];
        this.messagePump = new MessagePump();
        this.cancellationToken = cancellationToken;
        this.playerCardRepository = playerCardRepository;
        this.playerCards = new Dictionary<IServiceProviderCallback, PlayerData>();
      }
      #endregion

      #region Properties
      public GameStates GameState { get; private set; }

      public GameSessionStates GameSessionState { get; private set; }

      /*public Boolean NeedsClient
      {
        get { return this.clientCount < this.clients.Length; }
      }*/

      public States State { get; private set; }
      #endregion

      #region Methods
      public void AddPlayer(AddPlayerMessage addPlayerMessage)
      {
        this.GameSessionState = GameSessionStates.AddingPlayer;
        this.messagePump.Enqueue(addPlayerMessage);
      }

      public void ConfirmGameInitialized(IServiceProviderCallback client)
      {
        var message = new Message(Message.Types.ConfirmGameInitialized, client);
        this.messagePump.Enqueue(message);
      }

      public void ConfirmTownPlacement(IServiceProviderCallback client, UInt32 positionIndex)
      {
        var message = new PlaceTownMessage(client, positionIndex);
        this.messagePump.Enqueue(message);
      }

      public void LaunchGame(IServiceProviderCallback client)
      {
        var message = new LaunchGameMessage(client);
        this.messagePump.Enqueue(message);
      }

      public void ProcessPersonalMessage(IServiceProviderCallback client, String text)
      {
        var message = new PersonalMessage(client, text);
        this.messagePump.Enqueue(message);
      }

      public void RemoveClient(IServiceProviderCallback client)
      {
        for (Int32 i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] == client)
          {
            this.clients[i] = null;
            this.currentPlayerCount--;
            if (this.currentPlayerCount < this.clients.Length)
            {
              this.GameSessionState = GameSessionStates.AwaitingPlayer;
            }

            client.ConfirmPlayerHasLeftGame();

            var playerData = this.playerCards[client];
            for (Int32 j = 0; j < this.clients.Length; j++)
            {
              if (j != i)
              {
                this.clients[j].ConfirmOtherPlayerHasLeftGame(playerData.Username);
              }
            }

            this.playerCards.Remove(client);

            return;
          }
        }

        //TODO: Remove or make meaningful
        throw new NotImplementedException();
      }

      public void Start()
      {
        this.gameTask = Task.Factory.StartNew(() =>
        {
          try
          {
            this.State = States.Running;
            this.GameState = GameStates.Lobby;

            this.WaitForAllPlayersToJoin();
            
            this.SendConfirmGameSessionReadyToLaunchMessage();

            this.WaitForAllGameLaunchMessages();

            this.SendGameInitializationData();

            Message message;
            while (true)
            {
              if (this.messagePump.TryDequeue(Message.Types.Personal, out message))
              {
                var personalMessage = (PersonalMessage)message;
                var playerData = this.playerCards[personalMessage.Client];
                this.SendPersonalMessageToClients(personalMessage.Client, playerData.Username, personalMessage.Text);
              }

              this.cancellationToken.ThrowIfCancellationRequested();
              Thread.Sleep(50);
            }
          }
          catch (OperationCanceledException)
          {
            // Shutting down - ignore exception
            this.State = States.Stopping;
          }
          finally
          {
            this.State = States.Stopped;
          }
        });
      }

      public void StartGame()
      {
        this.gameTask = Task.Factory.StartNew(() =>
        {
          this.State = States.Running;
          try
          {
            Debug.Print("Start Game");

            var gameData = GameInitializationDataBuilder.Build(this.gameManager.Board);
            foreach (var client in this.clients)
            {
              client.InitializeGame(gameData);
            }

            // Clients confirming that they have completed game initialization.
            var awaitingGameInitializationConfirmation = new HashSet<IServiceProviderCallback>(this.clients);
            Message message = null;
            while (awaitingGameInitializationConfirmation.Count > 0)
            {
              this.cancellationToken.ThrowIfCancellationRequested();

              if (this.messagePump.TryDequeue(Message.Types.ConfirmGameInitialized, out message))
              {
                awaitingGameInitializationConfirmation.Remove(message.Client);
                Debug.Print("Received: " + awaitingGameInitializationConfirmation.Count + " left.");
                continue;
              }

              Thread.Sleep(50);
            }

            // Clients have all confirmed they received game initialization data
            // Now ask each client to place a town in dice roll order.
            this.PlaceTownsInFirstPassOrder(this.gameManager.GetFirstSetupPassOrder());

            // Do second pass of setup
            this.PlaceTownsInSecondPassOrder();
          }
          catch (OperationCanceledException)
          {
            // Shutting down - ignore exception
            this.State = States.Stopping;
          }
          finally
          {
            this.State = States.Stopped;
          }
        });
      }

      private void AddPlayer(IServiceProviderCallback client, String username)
      {
        for (var i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] != null)
          {
            continue;
          }

          this.clients[i] = client;

          client.ConfirmGameSessionJoined(this.GameToken, GameStates.Lobby);

          var playerCard = this.playerCardRepository.GetPlayerData(username);

          this.currentPlayerCount++;
          if (this.currentPlayerCount > 1)
          {
            this.SendPlayerDataToPlayers(i, client, playerCard);
          }

          this.playerCards.Add(client, playerCard);

          if (this.currentPlayerCount == this.clients.Length)
          {
            this.GameSessionState = GameSessionStates.Full;
          }
          else
          {
            this.GameSessionState = GameSessionStates.AwaitingPlayer;
          }

          return;
        }
      }

      private void PlaceTownsInFirstPassOrder(UInt32[] playerIndexes)
      {
        Message message = null;
        for (var index = 0; index < this.clients.Length; index++)
        {
          var playerIndex = playerIndexes[index];

          this.clients[playerIndex].ChooseTownLocation();
          Debug.Print("First pass: Choose town message sent for Index " + playerIndex);
          while (!this.messagePump.TryDequeue(Message.Types.RequestTownPlacement, out message))
          {
            this.cancellationToken.ThrowIfCancellationRequested();

            Thread.Sleep(50);
          }

          var placeTownMessage = (PlaceTownMessage)message;
          Debug.Print("First pass: Request town message received for location " + placeTownMessage.Location);
          this.gameManager.PlaceTown(placeTownMessage.Location);

          for (var clientIndex = 0; clientIndex < this.clients.Length; clientIndex++)
          {
            if (clientIndex == playerIndex)
            {
              continue;
            }

            this.clients[clientIndex].TownPlacedDuringSetup(placeTownMessage.Location);
          }
        }
      }

      private void PlaceTownsInSecondPassOrder()
      {
        Message message = null;
        UInt32[] playerIndexes = this.gameManager.GetSecondSetupPassOrder();
        for (var index = 0; index < this.clients.Length; index++)
        {
          var playerIndex = playerIndexes[index];

          this.clients[playerIndex].ChooseTownLocation();
          Debug.Print("Second pass: Choose town message sent for Index " + playerIndex);
          while (!this.messagePump.TryDequeue(Message.Types.RequestTownPlacement, out message))
          {
            this.cancellationToken.ThrowIfCancellationRequested();

            Thread.Sleep(50);
          }

          var placeTownMessage = (PlaceTownMessage)message;
          Debug.Print("Second pass: Request town message received for location " + placeTownMessage.Location);
          this.gameManager.PlaceTown(placeTownMessage.Location);

          for (var clientIndex = 0; clientIndex < this.clients.Length; clientIndex++)
          {
            if (clientIndex == playerIndex)
            {
              continue;
            }

            this.clients[clientIndex].TownPlacedDuringSetup(placeTownMessage.Location);
          }
        }
      }

      private void SendConfirmGameSessionReadyToLaunchMessage()
      {
        for (var i = 0; i < this.currentPlayerCount; i++)
        {
          if (this.clients[i] == null)
          {
            continue;
          }

          this.clients[i].ConfirmGameSessionReadyToLaunch();
        }
      }

      private void SendGameInitializationData()
      {
        var gameData = GameInitializationDataBuilder.Build(this.gameManager.Board);
        for (var i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] == null)
          {
            continue;
          }
          
          clients[i].InitializeGame(gameData);
        }
      }

      private void SendPlayerDataToPlayers(Int32 clientIndex, IServiceProviderCallback client, PlayerData playerCard)
      {
        for (var i = 0; i < this.clients.Length; i++)
        {
          if (i == clientIndex || this.clients[i] == null)
          {
            continue;
          }

          var otherPlayerCard = this.playerCards[this.clients[i]];
          client.PlayerDataForJoiningClient(otherPlayerCard);
          this.clients[i].PlayerDataForJoiningClient(playerCard);
        }
      }

      private void WaitForAllGameLaunchMessages()
      {
        Message message;
        var receivedMessages = new HashSet<IServiceProviderCallback>();
        while (receivedMessages.Count < this.clients.Length)
        {
          if (this.messagePump.TryDequeue(Message.Types.LaunchGame, out message))
          {
            var client = message.Client;
            if (receivedMessages.Contains(client))
            {
              continue;
            }

            receivedMessages.Add(client);
          }

          this.cancellationToken.ThrowIfCancellationRequested();
          Thread.Sleep(50);
        }

        this.GameState = GameStates.Setup;
      }

      private void WaitForAllPlayersToJoin()
      {
        Message message;
        while (this.GameSessionState != GameSessionStates.Full)
        {
          if (this.messagePump.TryDequeue(Message.Types.AddPlayer | Message.Types.Personal, out message))
          {
            if (message.Type == Message.Types.AddPlayer)
            {
              var addPlayerMessage = (AddPlayerMessage)message;
              this.AddPlayer(addPlayerMessage.Client, addPlayerMessage.Username);
            }
            else if (message.Type == Message.Types.Personal)
            {
              var personalMessage = (PersonalMessage)message;
              var playerCard = this.playerCards[personalMessage.Client];
              this.SendPersonalMessageToClients(personalMessage.Client, playerCard.Username, personalMessage.Text);
            }
          }

          this.cancellationToken.ThrowIfCancellationRequested();
          Thread.Sleep(50);
        }
      }

      private void SendPersonalMessageToClients(IServiceProviderCallback sendingClient, String sender, String text)
      {
        for (int i = 0; i < this.clients.Length; i++)
        {
          if (clients[i] == null || clients[i] == sendingClient)
          {
            continue;
          }

          clients[i].ReceivePersonalMessage(sender, text);
        }
      }
      #endregion
    }

    private class AddPlayerMessage : Message
    {
      public readonly String Username;

      public AddPlayerMessage(IServiceProviderCallback sender, String username) : base(Message.Types.AddPlayer, sender)
      {
        this.Username = username;
      }
    }

    private class LaunchGameMessage : Message
    {
      public LaunchGameMessage(IServiceProviderCallback client) : base(Types.LaunchGame, client) {}
    }

    private class Message
    {
      [Flags]
      public enum Types
      {
        AddPlayer = 1,
        ConfirmGameInitialized = 2,
        LaunchGame = 4,
        Personal = 8,
        RequestTownPlacement = 16,
        Any = AddPlayer | ConfirmGameInitialized | Personal | RequestTownPlacement
      }

      public readonly Types Type;

      public readonly IServiceProviderCallback Client;

      public Message(Types type, IServiceProviderCallback client)
      {
        this.Type = type;
        this.Client = client;
      }
    }

    private class PersonalMessage : Message
    {
      public readonly String Text;
      public PersonalMessage(IServiceProviderCallback client, String text) : base(Message.Types.Personal, client)
      {
        this.Text = text;
      }
    }

    private class PlaceTownMessage : Message
    {
      public readonly UInt32 Location;

      public PlaceTownMessage(IServiceProviderCallback client, UInt32 location) : base(Message.Types.RequestTownPlacement, client)
      {
        this.Location = location;
      }
    }

    private class MessagePump
    {
      #region Fields
      private ConcurrentQueue<Message> messages = new ConcurrentQueue<Message>();
      #endregion

      #region Methods
      public void Enqueue(Message message)
      {
        this.messages.Enqueue(message);
      }

      public Boolean TryDequeue(Message.Types messageType, out Message message)
      {
        message = null;
        if (this.messages.TryDequeue(out message))
        {
          if ((message.Type & messageType) == message.Type)
          {
            return true;
          }

          var exceptionText = String.Format("Message found with type {0}. Expected type {1}", message.Type, messageType);
          throw new Exception(exceptionText);
        }

        return false;
      }
      #endregion
    }

    private struct JoinTicket
    {
      public IServiceProviderCallback Client;

      public String Username;
    }
    #endregion
  }
}
