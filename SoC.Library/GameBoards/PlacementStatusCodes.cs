
namespace Jabberwocky.SoC.Library.GameBoards
{
    public enum PlacementStatusCodes
    {
        Success,
        LocationForCityIsInvalid,
        LocationForSettlementIsInvalid,
        LocationIsAlreadyCity,
        LocationIsOccupied,
        LocationIsNotSettled,
        LocationIsNotOwned,
        NoDirectConnection,
        RoadIsOccupied,
        RoadIsOffBoard,
        RoadNotConnectedToExistingRoad,
        SettlementNotConnectedToExistingRoad,
        StartingInfrastructureNotPresentWhenPlacingCity,
        StartingInfrastructureNotCompleteWhenPlacingCity,
        StartingInfrastructureNotPresentWhenPlacingRoad,
        StartingInfrastructureNotCompleteWhenPlacingRoad,
        StartingInfrastructureNotPresentWhenPlacingSettlement,
        StartingInfrastructureNotCompleteWhenPlacingSettlement,
        StartingInfrastructureAlreadyPresent,
        TooCloseToSettlement,
    }
}
