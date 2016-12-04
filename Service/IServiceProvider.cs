
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;

  [ServiceContract(SessionMode = SessionMode.Required,
    CallbackContract = typeof(IServiceProviderCallback))]
  public interface IServiceProvider
  {
    /// <summary>
    /// Ask to the server to join a game.
    /// </summary>
    [OperationContract(IsOneWay = true)]
    void TryJoinGame();

    /// <summary>
    /// Instruct the server that the client is leaving a game.
    /// </summary>
    /// <param name="gameToken"></param>
    [OperationContract(IsOneWay = true)]
    void TryLeaveGame(Guid gameToken);

    /// <summary>
    /// Confirm to the server that the client has completed game initialization.
    /// </summary>
    /// <param name="gameToken"></param>
    [OperationContract(IsOneWay = true)]
    void ConfirmGameInitialized(Guid gameToken);

    /*[OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);*/

    // TODO: Add your service operations here
  }
}
