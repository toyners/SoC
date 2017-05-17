﻿
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using GameBoards;

  public interface IGameController
  {
    Action<PlayerBase[]> GameJoinedEvent { get; set; }

    Action<GameBoardData> InitialBoardSetupEvent { get; set; }

    Action<ClientAccount> LoggedInEvent { get; set; }

    Action<Guid> StartInitialTurnEvent { get; set; }

    Guid GameId { get; }

    void StartLogIntoAccount(String username, String password);

    void StartJoiningGame(GameOptions gameFilter);

    void StartJoiningGame(GameOptions gameFilter, Guid accountToken);

    void PlaceTown(Location location);

    void UpgradeToCity(Location location);

    void BuildRoad(Location startingLocation, Location finishingLocation);

    DevelopmentCard BuyDevelopmentCard();

    ResourceTypes TradeResourcesWithBank();

    ResourceTypes TradeResourcesAtPort(Location location);

    ICollection<Offer> MakeOffer(Offer offer);

    void AcceptOffer(Offer offer);

    void Quit();
  }

  public struct Offer
  {
    PlayerData player;

    Int32 OfferedOre;
    Int32 OfferedWheat;
    Int32 OfferedSheep;
    Int32 OfferedLumber;
    Int32 OfferedBrick;

    Int32 WantedOreCount;
    Int32 WantedWheatCount;
    Int32 WantedSheepCount;
    Int32 WantedLumberCount;
    Int32 WantedBrickCount;
  }
}
