
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;

  public interface IServiceProviderCallback
  {
    [OperationContract(IsOneWay = true)]
    void StartTurn(Guid token);

    [OperationContract(IsOneWay = true)]
    void ConfirmGameJoined(Guid gameToken);

    [OperationContract(IsOneWay = true)]
    void ConfirmGameLeft();

    [OperationContract(IsOneWay = true)]
    void InitializeGame(GameInitializationData gameData);

    [OperationContract(IsOneWay = true)]
    void PlaceTown();
  }
}
