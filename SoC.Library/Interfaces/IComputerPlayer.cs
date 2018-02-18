
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using GameActions;
  using GameBoards;

  public enum PlayerActionTypes
  {
    EndTurn,
    BuildCity,
    BuildRoadSegment,
    BuildSettlement,
    BuyDevelopmentCard,
    PlayKnightCard,
    PlayMonopolyCard,
    PlayYearOfPlentyCard,
    TradeWithBank,
  }

  public interface IComputerPlayer : IPlayer
  {
    #region Methods
    void BuildInitialPlayerActions(GameBoardData gameBoard, PlayerDataView[] playerData);
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
    Boolean TryGetPlayerAction(out PlayerAction playerAction);
    void AddDevelopmentCard(DevelopmentCard developmentCard);
    #endregion
  }
}
