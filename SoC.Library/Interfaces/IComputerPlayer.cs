
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  using System.Collections.Generic;
  using GameActions;
    using Jabberwocky.SoC.Library.PlayerData;

    public interface IComputerPlayer : IPlayer
  {
    #region Methods
    void AddDevelopmentCard(DevelopmentCard developmentCard);
    void BuildInitialPlayerActions(PlayerDataModel[] playerData);
    UInt32 ChooseCityLocation();
    UInt32 ChooseSettlementLocation();
    void ChooseInitialInfrastructure(out UInt32 settlementLocation, out UInt32 roadEndLocation);
    KnightDevelopmentCard ChooseKnightCard();
    MonopolyDevelopmentCard ChooseMonopolyCard();
    ResourceClutch ChooseResourcesToCollectFromBank();
    ResourceClutch ChooseResourcesToDrop();
    ResourceTypes ChooseResourceTypeToRob();
    UInt32 ChooseRobberLocation();
    IPlayer ChoosePlayerToRob(IEnumerable<IPlayer> otherPlayers);
    ComputerPlayerAction GetPlayerAction();
    YearOfPlentyDevelopmentCard ChooseYearOfPlentyCard();
    #endregion
  }
}
