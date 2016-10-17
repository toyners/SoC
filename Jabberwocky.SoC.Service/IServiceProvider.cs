
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.Serialization;
  using System.ServiceModel;
  using System.Text;

  [ServiceContract(SessionMode = SessionMode.Required,
    CallbackContract = typeof(IClient))]
  public interface IServiceProvider
  {
    [OperationContract]
    Boolean JoinGame();

    [OperationContract(IsOneWay = true)]
    void LeaveGame();

    /*[OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);*/

    // TODO: Add your service operations here
  }
}
