
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;
  using System.Threading;
  using Library;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ServiceProvider : IServiceProvider
  {
    #region Fields
    private List<IServiceProviderCallback> clients;

    private Queue<IServiceProviderCallback> waitingForGame;

    private Dictionary<Guid, GameManager> games;
    #endregion

    #region Construction
    public ServiceProvider()
    {
      this.clients = new List<IServiceProviderCallback>(4);
      this.waitingForGame = new Queue<IServiceProviderCallback>();
      this.games = new Dictionary<Guid, GameManager>();
    }
    #endregion

    #region Methods
    public void TryJoinGame()
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.waitingForGame.Enqueue(client);
    }

    public void LeaveGame()
    {
      Console.WriteLine("Client left game");
    }
    #endregion

    /*public string GetData(int value)
    {
      return string.Format("You entered: {0}", value);
    }

    public CompositeType GetDataUsingDataContract(CompositeType composite)
    {
      if (composite == null)
      {
        throw new ArgumentNullException(nameof(composite));
      }
      if (composite.BoolValue)
      {
        composite.StringValue += "Suffix";
      }
      return composite;
    }*/
  }

  public class GameConnector
  {
    private Boolean working;
    private List<IServiceProviderCallback> clients;
    private Dictionary<Guid, GameRecord> games;
    private Queue<IServiceProviderCallback> waitingForGame;

    public GameConnector()
    {
      this.clients = new List<IServiceProviderCallback>();
      this.waitingForGame = new Queue<IServiceProviderCallback>();
      this.games = new Dictionary<Guid, GameRecord>();
    }

    public void StopMatching()
    {
      this.working = false;
    }

    public void StartMatching()
    {
      if (this.working)
      {
        return;
      }

      this.working = true;
      
    }

    public void MatchPlayersWithGames()
    {
      while (this.working)
      {
        while (this.waitingForGame.Count == 0)
        {
          Thread.Sleep(500);
        }

        IServiceProviderCallback client;
        GameRecord game;
        var matchMade = false;
        foreach (var kv in this.games)
        {
          game = kv.Value;
          if (game.NeedsPlayer)
          {
            client = this.waitingForGame.Dequeue();
            game.AddPlayer(client);
            matchMade = true;
            break;
          }
        }

        if (matchMade)
        {
          continue;
        }

        // Create a new game and add the player
        client = this.waitingForGame.Dequeue();
        game = new GameRecord();
        game.AddPlayer(client);
        this.games.Add(game.GameToken, game);
      }
    }

    private class GameRecord
    {
      public GameManager Game;

      public Player[] Players;

      public IServiceProviderCallback[] Clients;

      public Guid GameToken;

      public GameRecord()
      {
        this.GameToken = Guid.NewGuid();
        this.Players = new Player[4];
        var board = new Board();
        var diceRoller = new DiceRoller();
        this.Game = new GameManager(board, diceRoller, this.Players, new DevelopmentCardPile());

        this.Clients = new IServiceProviderCallback[4];
      }

      public Boolean NeedsPlayer
      {
        get { return this.Players.Length < 4; }
      }

      public void AddPlayer(IServiceProviderCallback client)
      {
        var player = new Player(null);
        this.Players[this.Players.Length] = player;
        this.Clients[this.Clients.Length] = client;
        client.ConfirmGameJoined(this.GameToken);
      }
    }
  }
}
