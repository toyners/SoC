
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;

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
    private ConcurrentQueue<IServiceProviderCallback> waitingForGameQueue;
    private Task matchingTask;
    private UInt32 maximumPlayerCount;
    private IDiceRollerFactory diceRollerFactory;
    private IGameManagerFactory gameManagerFactory;
    private CancellationTokenSource cancellationTokenSource;
    #endregion

    #region Construction
    public GameSessionManager(IGameManagerFactory gameManagerFactory, UInt32 maximumPlayerCount)
    {
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGameQueue = new ConcurrentQueue<IServiceProviderCallback>();
      this.gameSessions = new Dictionary<Guid, GameSession>();
      this.maximumPlayerCount = maximumPlayerCount;
      this.cancellationTokenSource = new CancellationTokenSource();
      this.gameManagerFactory = gameManagerFactory;
      this.State = States.Stopped;
    }
    #endregion

    #region Properties
    public States State { get; private set; }
    #endregion

    #region Methods
    public void AddClient(IServiceProviderCallback client)
    {
      // TODO: Check for null reference
      this.waitingForGameQueue.Enqueue(client);
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

    private GameSession AddToNewGameSession(IServiceProviderCallback client, CancellationToken cancellationToken)
    {
      var gameSession = new GameSession(this.gameManagerFactory.Create(), this.maximumPlayerCount, cancellationToken);
      gameSession.AddClient(client);
      this.gameSessions.Add(gameSession.GameToken, gameSession);
      return gameSession;
    }

    private GameSession GetGameSession(Guid gameToken)
    {
      if (!this.gameSessions.ContainsKey(gameToken))
      {
        throw new NotImplementedException(); //TODO: Change for Meaningful exception
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

          IServiceProviderCallback client;
          var gotClient = this.waitingForGameQueue.TryDequeue(out client);
          if (!gotClient)
          {
            // Couldn't get the client from the queue (probably because another thread got it).
            continue;
          }

          GameSession gameSession = null;
          if (!this.TryAddToCurrentGameSession(client, out gameSession))
          {
            gameSession = this.AddToNewGameSession(client, cancellationToken);
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

    private Boolean TryAddToCurrentGameSession(IServiceProviderCallback client, out GameSession gameSession)
    {
      gameSession = null;
      foreach (var kv in this.gameSessions)
      {
        gameSession = kv.Value;
        if (gameSession.NeedsClient)
        {
          gameSession.AddClient(client);
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
      private Task gameTask;
      private MessagePump messagePump;
      #endregion

      #region Construction
      public GameSession(IGameManager gameManager, UInt32 clientCount, CancellationToken cancellationToken)
      {
        this.GameToken = Guid.NewGuid();
        this.gameManager = gameManager;
        this.clients = new IServiceProviderCallback[clientCount];
        this.messagePump = new MessagePump();
        this.cancellationToken = cancellationToken;
        //var board = new Board(BoardSizes.Standard);
        //this.Game = new GameManager(board, playerCount, diceRoller, new DevelopmentCardPile());
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

      public void AddClient(IServiceProviderCallback client)
      {
        for (var i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] == null)
          {
            this.clients[i] = client;
            this.clientCount++;
            client.ConfirmGameJoined(this.GameToken);
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
            client.ConfirmGameLeft();
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
            this.PlaceTownsInOrder(this.gameManager.GetFirstSetupPassOrder());

            this.PlaceTownsInOrder(this.gameManager.GetFirstSetupPassOrder());
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

      private void PlaceTownsInOrder(UInt32[] playerIndexes)
      {
        Message message = null;
        var selectedTownLocations = new List<UInt32>(8);
        var playersThatHavePlacedTown = new List<IServiceProviderCallback>(3);
        for (var index = 0; index < this.clientCount; index++)
        {
          var playerIndex = playerIndexes[index];

          this.clients[playerIndex].ChooseTownLocation(selectedTownLocations);
          Debug.Print("Sent: Index " + playerIndex);
          while (!this.messagePump.TryDequeue(Message.Types.RequestTownPlacement, out message))
          {
            this.cancellationToken.ThrowIfCancellationRequested();

            Thread.Sleep(50);
          }

          var placeTownMessage = (PlaceTownMessage)message;
          this.gameManager.PlaceTown(placeTownMessage.Location);
          selectedTownLocations.Add(placeTownMessage.Location);

          foreach (var client in playersThatHavePlacedTown)
          {
            client.TownPlacedDuringSetup(placeTownMessage.Location);
          }

          playersThatHavePlacedTown.Add(this.clients[playerIndex]);
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
    #endregion
  }
}
