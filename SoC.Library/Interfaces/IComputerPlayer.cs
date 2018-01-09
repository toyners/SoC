
using System;
using Jabberwocky.SoC.Library.GameBoards;

namespace Jabberwocky.SoC.Library.Interfaces
{
  public enum PlayerAction
  {
    EndTurn,
    BuildCity,
    BuildRoad,
    BuyDevelopmentCard,
    PlayKnightCard,
  }

  public interface IComputerPlayer : IPlayer
  {
    #region Methods
    UInt32 ChooseCityLocation(GameBoardData gameBoardData);
    UInt32 ChooseSettlementLocation(GameBoardData gameBoardData);
    void ChooseRoad(GameBoardData gameBoardData, out UInt32 startRoadLocation, out UInt32 endRoadLocation);
    void ChooseInitialInfrastructure(GameBoardData gameBoardData, out UInt32 settlementLocation, out UInt32 roadEndLocation);
    KnightDevelopmentCard ChooseKnightCard();
    ResourceClutch ChooseResourcesToDrop();
    UInt32 ChooseRobberLocation();
    Guid ChoosePlayerToRob();
    PlayerAction GetPlayerAction();
    void AddDevelopmentCard(DevelopmentCard developmentCard);
    #endregion
  }
}
