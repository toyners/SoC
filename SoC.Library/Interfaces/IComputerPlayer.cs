﻿
using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public enum PlayerActionTypes
  {
    EndTurn,
    BuildCity,
    BuildRoad,
    BuyDevelopmentCard,
    PlayKnightCard,
    PlayMonopolyCard,
    PlayYearOfPlentyCard,
    TradeWithBank,
  }

  public interface IComputerPlayer : IPlayer
  {
    #region Methods
    UInt32 ChooseCityLocation(GameBoardData gameBoardData);
    UInt32 ChooseSettlementLocation(GameBoardData gameBoardData);
    void ChooseRoad(GameBoardData gameBoardData, out UInt32 startRoadLocation, out UInt32 endRoadLocation);
    void ChooseInitialInfrastructure(GameBoardData gameBoardData, out UInt32 settlementLocation, out UInt32 roadEndLocation);
    KnightDevelopmentCard ChooseKnightCard();
    MonopolyDevelopmentCard ChooseMonopolyCard();
    YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard();
    ResourceClutch ChooseResouresToCollectFromBank();
    ResourceClutch ChooseResourcesToDrop();
    ResourceTypes ChooseResourceTypeToRob();
    UInt32 ChooseRobberLocation();
    IPlayer ChoosePlayerToRob(IEnumerable<IPlayer> otherPlayers);
    Boolean TryGetPlayerAction(out PlayerMove playerMove);
    void AddDevelopmentCard(DevelopmentCard developmentCard);
    #endregion
  }

  public class PlayerMove
  {
    public readonly PlayerActionTypes Action;

    public PlayerMove(PlayerActionTypes action)
    {
      this.Action = action;
    }
  }

  public class TradeWithBankMove : PlayerMove
  {
    public TradeWithBankMove(PlayerActionTypes action) : base(action)
    {
    }
  }
}
