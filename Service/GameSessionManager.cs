
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
    #endregion

    #region Fields
    private List<IServiceProviderCallback> clients;
    private Dictionary<Guid, GameSession> gameSessions;
    private IPlayerCardRepository playerCardRepository;
    private ConcurrentQueue<JoinTicket> waitingForGameQueue;
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
      this.waitingForGameQueue = new ConcurrentQueue<JoinTicket>();
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
    public void AddClient(IServiceProviderCallback client, String username)
    {
      // TODO: Check for null reference
      this.waitingForGameQueue.Enqueue(new JoinTicket { Client = client, Username = username });
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

    public States GameSessionState(Guid gameToken)
    {
      var gameSession = this.GetGameSession(gameToken);
      return gameSession.State;
    }

    public void RemoveClient(Guid gameToken, IServiceProviderCallback client)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.RemoveClient(client);
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

    private GameSession AddToNewGameSession(JoinTicket joinTicket, CancellationToken cancellationToken)
    {
      var gameSession = new GameSession(this.gameManagerFactory.Create(), this.maximumPlayerCount, this.playerCardRepository, cancellationToken);
      gameSession.AddClient(joinTicket.Client, joinTicket.Username);
      this.gameSessions.Add(gameSession.GameToken, gameSession);
      return gameSession;
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
          while (this.waitingForGameQueue.IsEmpty)
          {
            cancellationToken.ThrowIfCancellationRequested();
            Thread.Sleep(500);
          }

          JoinTicket joinTicket;
          var gotClient = this.waitingForGameQueue.TryDequeue(out joinTicket);
          if (!gotClient)
          {
            // Couldn't get the client from the queue (probably because another thread got it).
            continue;
          }

          GameSession gameSession = null;
          if (!this.TryAddToCurrentGameSession(joinTicket, out gameSession))
          {
            gameSession = this.AddToNewGameSession(joinTicket, cancellationToken);
          }

          if (!gameSession.NeedsClient)
          {
            // Game is full so start it
            gameSession.StartGame();
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

    private Boolean TryAddToCurrentGameSession(JoinTicket joinTicket, out GameSession gameSession)
    {
      gameSession = null;
      foreach (var kv in this.gameSessions)
      {
        gameSession = kv.Value;
        if (gameSession.NeedsClient)
        {
          gameSession.AddClient(joinTicket.Client, joinTicket.Username);
          return true;
        }
      }

      return false;
    }
    #endregion

    #region Classes
    private class GameSession
    {
      #region Fields
      private IGameManager gameManager;

      public Guid GameToken;

      private CancellationToken cancellationToken;
      private Int32 clientCount;
      private IServiceProviderCallback[] clients;
      private IPlayerCardRepository playerCardRepository;
      private Dictionary<IServiceProviderCallback, PlayerData> playerCards;
      private Task gameTask;
      private MessagePump messagePump;
      #endregion

      #region Construction
      public GameSession(IGameManager gameManager, UInt32 clientCount, IPlayerCardRepository playerCardRepository, CancellationToken cancellationToken)
      {
        this.GameToken = Guid.NewGuid();
        this.gameManager = gameManager;
        this.clients = new IServiceProviderCallback[clientCount];
        this.messagePump = new MessagePump();
        this.cancellationToken = cancellationToken;
        this.playerCardRepository = playerCardRepository;
        this.playerCards = new Dictionary<IServiceProviderCallback, PlayerData>();
      }
      #endregion

      #region Properties
      public Boolean NeedsClient
      {
        get { return this.clientCount < this.clients.Length; }
      }

      public States State { get; private set; }
      #endregion

      #region Methods
      public void ConfirmGameInitialized(IServiceProviderCallback client)
      {
        var message = new Message(Message.Types.ConfirmGameInitialized, client);
        this.messagePump.Enqueue(message);
      }

      public void ConfirmTownPlacement(IServiceProviderCallback client, UInt32 positionIndex)
      {
        var message = new PlaceTownMessage(Message.Types.RequestTownPlacement, client, positionIndex);
        this.messagePump.Enqueue(message);
      }

      public void AddClient(IServiceProviderCallback client, String userName)
      {
        for (var i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] == null)
          {
            this.clients[i] = client;
            this.clientCount++;
            client.ConfirmGameJoined(this.GameToken);

            var playerCard = this.playerCardRepository.GetPlayerData(userName);
            
            for (var j = 0; j < this.clients.Length; j++)
            {
              if (j == i || this.clients[j] == null)
              {
                continue;
              }

              client.PlayerDataForJoiningClient(this.playerCards[this.clients[j]]);
              this.clients[j].PlayerDataForJoiningClient(playerCard);
            }

            this.playerCards.Add(client, playerCard);

            return;
          }
        }

        //TODO: Remove or make meaningful
        throw new NotImplementedException();
      }

      public void RemoveClient(IServiceProviderCallback client)
      {
        for (Int32 i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] == client)
          {
            this.clients[i] = null;
            this.clientCount--;
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
                awaitingGameInitializationConfirmation.Remove(message.Sender);
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

      private void PlaceTownsInFirstPassOrder(UInt32[] playerIndexes)
      {
        Message message = null;
        for (var index = 0; index < this.clientCount; index++)
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
        for (var index = 0; index < this.clientCount; index++)
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
      #endregion
    }

    private class Message
    {
      public enum Types
      {
        ConfirmGameInitialized,
        RequestTownPlacement
      }

      public readonly Types Type;

      public readonly IServiceProviderCallback Sender;

      public Message(Types type, IServiceProviderCallback sender)
      {
        this.Type = type;
        this.Sender = sender;
      }
    }

    private class PlaceTownMessage : Message
    {
      public readonly UInt32 Location;

      public PlaceTownMessage(Types type, IServiceProviderCallback sender, UInt32 location) : base(type, sender)
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
          if (message.Type == messageType)
          {
            return true;
          }

          this.messages.Enqueue(message);
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
