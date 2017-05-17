
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Interfaces;
  using GameBoards;

  public class LocalGameController : IGameController
  {
    private Guid curentPlayerTurnToken;
    private IDiceRoller diceRoller;
    private GameBoardManager gameBoardManager;
    private IGameSession gameSession;

    public LocalGameController(IDiceRoller diceRoller)
    {
      this.diceRoller = diceRoller;
    }

    public Guid GameId { get; private set; }

    #region Events
    public Action<PlayerBase[]> GameJoinedEvent { get; set; }

    public Action<GameBoardData> InitialBoardSetupEvent { get; set; }

    public Action<ClientAccount> LoggedInEvent { get; set; }

    public Action<Guid> StartInitialTurnEvent { get; set; }
    #endregion

    public void AcceptOffer(Offer offer)
    {
      throw new NotImplementedException();
    }

    public void BuildRoad(Location startingLocation, Location finishingLocation)
    {
      throw new NotImplementedException();
    }

    public DevelopmentCard BuyDevelopmentCard()
    {
      throw new NotImplementedException();
    }

    public ICollection<Offer> MakeOffer(Offer offer)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(Location location)
    {
      throw new NotImplementedException();
    }

    public void Quit()
    {
      throw new NotImplementedException();
    }

    public void StartJoiningGame(GameOptions gameOptions)
    {
      if (gameOptions == null)
      {
        gameOptions = new GameOptions { MaxPlayers = 1, MaxAIPlayers = 3 };
      }

      var players = new PlayerBase[gameOptions.MaxPlayers + gameOptions.MaxAIPlayers];

      var index = 0;
      while ((gameOptions.MaxPlayers--) > 0)
      {
        players[index++] = new PlayerData();
      }

      while ((gameOptions.MaxAIPlayers--) > 0)
      {
        players[index++] = new PlayerView();
      }

      this.GameJoinedEvent?.Invoke(players);

      this.gameBoardManager = new GameBoardManager(BoardSizes.Standard);
      this.InitialBoardSetupEvent?.Invoke(this.gameBoardManager.Data);
      this.gameSession = new GameSession();

      this.curentPlayerTurnToken = this.GetTurnToken();
      this.StartInitialTurnEvent?.Invoke(this.curentPlayerTurnToken);
    }

    public void StartJoiningGame(GameOptions gameOptions, Guid accountToken)
    {
      throw new NotImplementedException();
    }

    public void StartLogIntoAccount(String username, String password)
    {
      throw new NotImplementedException();
    }

    public ResourceTypes TradeResourcesAtPort(Location location)
    {
      throw new NotImplementedException();
    }

    public ResourceTypes TradeResourcesWithBank()
    {
      throw new NotImplementedException();
    }

    public void UpgradeToCity(Location location)
    {
      throw new NotImplementedException();
    }

    private Guid GetTurnToken()
    {
      return Guid.NewGuid();
    }
  }
}
