
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;

  [ServiceContract(SessionMode = SessionMode.Required,
    CallbackContract = typeof(IServiceProviderCallback))]
  public interface IServiceProvider
  {
    [OperationContract]
    Boolean TryJoinGame();

    [OperationContract(IsOneWay = true)]
    void LeaveGame();

    /*[OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);*/

    // TODO: Add your service operations here
  }
}
