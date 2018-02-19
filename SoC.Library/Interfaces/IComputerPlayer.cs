
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using GameActions;
  using GameBoards;

  public interface IComputerPlayer : IPlayer
  {
    #region Methods
    void BuildInitialPlayerActions(GameBoard gameBoard, PlayerDataView[] playerData);
    UInt32 ChooseCityLocation(GameBoard gameBoardData);
    UInt32 ChooseSettlementLocation(GameBoard gameBoardData);
    void ChooseRoad(GameBoard gameBoardData, out UInt32 startRoadLocation, out UInt32 endRoadLocation);
    void ChooseInitialInfrastructure(GameBoard gameBoardData, out UInt32 settlementLocation, out UInt32 roadEndLocation);
    KnightDevelopmentCard ChooseKnightCard();
    MonopolyDevelopmentCard ChooseMonopolyCard();
    YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard();
    ResourceClutch ChooseResouresToCollectFromBank();
    ResourceClutch ChooseResourcesToDrop();
    ResourceTypes ChooseResourceTypeToRob();
    UInt32 ChooseRobberLocation();
    IPlayer ChoosePlayerToRob(IEnumerable<IPlayer> otherPlayers);
    Boolean TryGetPlayerAction(out ComputerPlayerAction playerAction);
    void AddDevelopmentCard(DevelopmentCard developmentCard);
    #endregion
  }
}
