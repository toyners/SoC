
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;
  using Logging;
  using Messages;
  using Toolkit.Logging;
  using Toolkit.Object;
  using Toolkit.String;

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
    private IGameSessionTokenFactory gameSessionTokenFactory;
    private ILoggerFactory loggerFactory;
    #endregion

    #region Construction
    public GameSessionManager(UInt32 maximumPlayerCount, String logFileBasePath)
    {
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGameSessionQueue = new ConcurrentQueue<AddPlayerMessage>();
      this.gameSessions = new Dictionary<Guid, GameSession>();
      this.maximumPlayerCount = maximumPlayerCount;
      this.cancellationTokenSource = new CancellationTokenSource();
      this.gameManagerFactory = new GameManagerFactory();
      this.gameSessionTokenFactory = new GameSessionTokenFactory();
      this.playerCardRepository = new PlayerCardRepository();
      this.loggerFactory = new LogFileFactory(logFileBasePath);
      this.State = States.Stopped;
    }
    #endregion

    #region Properties
    public States State { get; private set; }

    public IGameManagerFactory GameManagerFactory
    { 
      get { return this.gameManagerFactory; } 
      set
      {
        value.VerifyThatObjectIsNotNull("Property 'GameManagerFactory' has set to null.");
        this.gameManagerFactory = value;
      }
    }

    public IGameSessionTokenFactory GameSessionTokenFactory
    {
      get { return this.gameSessionTokenFactory; }
      set
      {
        value.VerifyThatObjectIsNotNull("Property 'GameSessionTokenFactory' has been set to null.");
        this.gameSessionTokenFactory = value;
      }
    }

    public ILoggerFactory LoggerFactory
    {
      get { return this.loggerFactory; }
      set
      {
        value.VerifyThatObjectIsNotNull("Property 'LoggerFactory' has been set to null.");
        this.loggerFactory = value;
      }
    }

    public IPlayerCardRepository PlayerCardRepository
    {
      get { return this.playerCardRepository; }
      set
      {
        value.VerifyThatObjectIsNotNull("Property 'PlayerCardRepository' has been set to null.");
        this.playerCardRepository = value;
      }
    }
    #endregion

    #region Methods
    public void AddPlayer(IServiceProviderCallback client, String username)
    {
      client.VerifyThatObjectIsNotNull("Parameter 'client' is null.");
      username.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'username' is null or empty.");
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

    public void ProcessPersonalMessage(Guid gameToken, IServiceProviderCallback client, String text)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.QueuePersonalMessage(client, text);
    }

    public void RemoveClient(Guid gameToken, IServiceProviderCallback client)
    {
      var gameSession = this.GetGameSession(gameToken);
      gameSession.RemoveClient(new RemovePlayerMessage(client));
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
      var gameSessionToken = this.gameSessionTokenFactory.CreateGameSessionToken();
      var gameSession = new GameSession(this.gameManagerFactory.Create(), this.maximumPlayerCount, this.playerCardRepository, gameSessionToken,  cancellationToken, this.loggerFactory);
      gameSession.Start();
      gameSession.AddPlayer(addPlayerMessage);
      this.gameSessions.Add(gameSessionToken, gameSession);
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

          if (this.AlreadyInGameSession(addPlayerMessage.Client))
          {
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

    private Boolean AlreadyInGameSession(IServiceProviderCallback client)
    {
      foreach (var gameSession in this.gameSessions.Values)
      {
        if (gameSession.ContainsClient(client))
        {
          return true;
        }
      }

      return false;
    }

    private Boolean TryAddToExistingGameSession(AddPlayerMessage addPlayerMessage)
    {
      var gotBusyGameSession = false;
      do
      {
        gotBusyGameSession = false;
        foreach (var gameSession in this.gameSessions.Values)
        {
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

      public Guid GameSessionToken;

      private CancellationToken cancellationToken;
      private UInt32 currentPlayerCount;
      private IServiceProviderCallback[] clients;
      private IPlayerCardRepository playerCardRepository;
      private Dictionary<IServiceProviderCallback, PlayerData> playerCards;
      private Task gameTask;
      private ConcurrentQueue<GameSessionMessage> messageQueue;
      private ILoggerFactory loggerFactory;
      private HashSet<IServiceProviderCallback> clientsThatReceivedMessages;
      #endregion

      #region Construction
      public GameSession(IGameManager gameManager, UInt32 maxPlayerCount, IPlayerCardRepository playerCardRepository, Guid gameSessionToken, CancellationToken cancellationToken, ILoggerFactory loggerFactory)
      {
        // No parameter checking done because this is not a public interface.
        this.GameSessionToken = gameSessionToken;
        this.gameManager = gameManager;
        this.clients = new IServiceProviderCallback[maxPlayerCount];
        this.cancellationToken = cancellationToken;
        this.playerCardRepository = playerCardRepository;
        this.playerCards = new Dictionary<IServiceProviderCallback, PlayerData>();
        this.messageQueue = new ConcurrentQueue<GameSessionMessage>();
        this.loggerFactory = loggerFactory;
        this.clientsThatReceivedMessages = new HashSet<IServiceProviderCallback>();
      }
      #endregion

      #region Properties
      public GameStates GameState { get; private set; }

      public GameSessionStates GameSessionState { get; private set; }

      public States State { get; private set; }
      #endregion

      #region Methods
      public void AddPlayer(AddPlayerMessage addPlayerMessage)
      {
        this.GameSessionState = GameSessionStates.AddingPlayer;
        this.messageQueue.Enqueue(addPlayerMessage);
      }

      public void ConfirmGameInitialized(IServiceProviderCallback client)
      {
        var message = new GameSessionMessage(GameSessionMessage.Types.ConfirmGameInitialized, client);
        this.messageQueue.Enqueue(message);
      }

      public void ConfirmTownPlacement(IServiceProviderCallback client, UInt32 positionIndex)
      {
        var message = new PlaceTownMessage(client, positionIndex);
        this.messageQueue.Enqueue(message);
      }

      public void LaunchGame(IServiceProviderCallback client)
      {
        var message = new LaunchGameMessage(client);
        this.messageQueue.Enqueue(message);
      }

      public void QueuePersonalMessage(IServiceProviderCallback client, String text)
      {
        var message = new PersonalMessage(client, text);
        this.messageQueue.Enqueue(message);
      }

      public void RemoveClient(RemovePlayerMessage removePlayerMessage)
      {
        this.messageQueue.Enqueue(removePlayerMessage);
      }

      public void Start()
      {
        this.gameTask = Task.Factory.StartNew(() =>
        {
          var fileName = this.GameSessionToken + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
          using (var logger = this.loggerFactory.Create(fileName))
          {
            try
            {
              this.State = States.Running;
              this.GameState = GameStates.Lobby;

              logger.Message("Started.");
              logger.Message("State: " + this.State + ", GameState: " + this.GameState);

              while (true)
              {
                this.cancellationToken.ThrowIfCancellationRequested();

                GameSessionMessage message;
                if (this.messageQueue.IsEmpty || !this.messageQueue.TryDequeue(out message))
                {
                  Thread.Sleep(50);
                  continue;
                }

                logger.Message("Message of type " + message.Type + " received.");

                switch (message.Type)
                {
                  case GameSessionMessage.Types.AddPlayer:
                  {
                    this.ProcessAddPlayerMessage((AddPlayerMessage)message);
                    break;
                  }

                  case GameSessionMessage.Types.LaunchGame:
                  {
                    this.ProcessLaunchGameMessage((LaunchGameMessage)message);
                    break;
                  }

                  case GameSessionMessage.Types.Personal:
                  {
                    this.ProcessPersonalMessage((PersonalMessage)message);
                    break;
                  }

                  case GameSessionMessage.Types.RemovePlayer:
                  {
                    var removePlayerMessage = message as RemovePlayerMessage;
                    this.RemovePlayer(removePlayerMessage.Client);
                    break;
                  }

                  default:
                  {
                    logger.Exception("EXCEPTION: Unknown message type " + message.Type);
                    break;
                  }
                }
              }
            }
            catch (OperationCanceledException)
            {
              // Shutting down - ignore exception
              this.State = States.Stopping;
              logger.Message("Stopping.");
            }
            catch (Exception exception)
            {
              logger.Exception(exception.Message);
            }
            finally
            {
              this.State = States.Stopped;
              logger.Message("Stopped.");
            }
          }
        });
      }

      /*public void StartGame()
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
            GameSessionMessage message = null;
            while (awaitingGameInitializationConfirmation.Count > 0)
            {
              this.cancellationToken.ThrowIfCancellationRequested();

              if (this.messagePump.TryDequeue(GameSessionMessage.Types.ConfirmGameInitialized, out message))
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
      }*/

      private Boolean AddPlayer(IServiceProviderCallback client, String username)
      {
        for (var i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] != null)
          {
            continue;
          }

          this.clients[i] = client;

          client.ConfirmGameSessionJoined(this.GameSessionToken, GameStates.Lobby);

          var playerCard = this.playerCardRepository.GetPlayerData(username);

          this.currentPlayerCount++;
          if (this.currentPlayerCount > 1)
          {
            this.SendPlayerDataToPlayers(i, client, playerCard);
          }

          this.playerCards.Add(client, playerCard);

          return this.currentPlayerCount == this.clients.Length;
        }

        return true;
      }

      private void PlaceTownsInFirstPassOrder(UInt32[] playerIndexes)
      {
        GameSessionMessage message = null;
        for (var index = 0; index < this.clients.Length; index++)
        {
          var playerIndex = playerIndexes[index];

          this.clients[playerIndex].ChooseTownLocation();
          Debug.Print("First pass: Choose town message sent for Index " + playerIndex);
          while (!this.messageQueue.TryDequeue(out message))
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
        GameSessionMessage message = null;
        UInt32[] playerIndexes = this.gameManager.GetSecondSetupPassOrder();
        for (var index = 0; index < this.clients.Length; index++)
        {
          var playerIndex = playerIndexes[index];

          this.clients[playerIndex].ChooseTownLocation();
          Debug.Print("Second pass: Choose town message sent for Index " + playerIndex);
          while (!this.messageQueue.TryDequeue(out message))
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

      private void ProcessAddPlayerMessage(AddPlayerMessage addPlayerMessage)
      {
        if (this.AddPlayer(addPlayerMessage.Client, addPlayerMessage.Username))
        {
          this.GameSessionState = GameSessionStates.Full;
          this.SendConfirmGameSessionReadyToLaunchMessage();
        }
        else
        {
          this.GameSessionState = GameSessionStates.AwaitingPlayer;
        }
      }

      private void ProcessLaunchGameMessage(LaunchGameMessage message)
      {
        if (!this.clientsThatReceivedMessages.Contains(message.Client))
        {
          this.clientsThatReceivedMessages.Add(message.Client);
        }

        var haveReceivedMessagesFromAllClients = (this.clientsThatReceivedMessages.Count == this.clients.Length);
        if (haveReceivedMessagesFromAllClients)
        {
          this.clientsThatReceivedMessages.Clear();
          this.SendGameInitializationData();
        }
      }

      private void ProcessPersonalMessage(PersonalMessage message)
      {
        var playerData = this.playerCards[message.Client];
        this.SendPersonalMessageToClients(message.Client, playerData.Username, message.Text);
      }

      private void RemovePlayer(IServiceProviderCallback client)
      {
        for (Int32 i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] != client)
          {
            continue;
          }

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
            if (this.clients[j] == null)
            {
              continue;
            }

            this.clients[j].ConfirmOtherPlayerHasLeftGame(playerData.Username);
          }

          this.playerCards.Remove(client);

          return;
        }

        //TODO: Remove or make meaningful
        throw new NotImplementedException();
      }

      private void SendConfirmGameSessionReadyToLaunchMessage()
      {
        for (var i = 0; i < this.clients.Length; i++)
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

      public Boolean ContainsClient(IServiceProviderCallback client)
      {
        for (var i = 0; i < this.clients.Length; i++)
        {
          if (this.clients[i] == null)
          {
            continue;
          }

          if (this.clients[i] == client)
          {
            return true;
          }
        }

        return false;
      }
      #endregion
    }
    #endregion
  }
}
