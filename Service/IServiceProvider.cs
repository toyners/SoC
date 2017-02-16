
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.ServiceModel;

  [ServiceContract(SessionMode = SessionMode.Required,
    CallbackContract = typeof(IClientCallback))]
  public interface IServiceProvider
  {
    /// <summary>
    /// Ask to the server to join a game.
    /// </summary>
    /// <param name="username">Username of player. Can be null or empty for anonymous player.</param>
    [OperationContract(IsOneWay = true)]
    void TryJoinGame(String username);

    /// <summary>
    /// Instruct the server that the client is leaving a game.
    /// </summary>
    /// <param name="gameToken"></param>
    [OperationContract(IsOneWay = true)]
    void TryLeaveGame(Guid gameToken);

    /// <summary>
    /// Confirm to the server that the client has completed game initialization.
    /// </summary>
    /// <param name="gameToken">The game identifier.</param>
    [OperationContract(IsOneWay = true)]
    void ConfirmGameInitialized(Guid gameToken);

    /*[OperationContract]
    string GetData(int value);

    [OperationContract]
    CompositeType GetDataUsingDataContract(CompositeType composite);*/

    // TODO: Add your service operations here
  }
}
