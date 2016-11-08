
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;

  [ServiceContract(SessionMode = SessionMode.Required,
    CallbackContract = typeof(IServiceProviderCallback))]
  public interface IServiceProvider
  {
    [OperationContract(IsOneWay = true)]
    void TryJoinGame();

    [OperationContract(IsOneWay = true)]
    void TryLeaveGame(Guid gameToken);

    /*[OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);*/

    // TODO: Add your service operations here
  }
}
