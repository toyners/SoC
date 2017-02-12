
namespace Service.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Messages;

  public class TestScript
  {
    #region Enums
    public enum RunPoints
    {
      RunUntilClientsReceiveGameSessionReadyToLaunchMessage,
      RunUntilClientsReceiveGameInitializationMessage,
      RunUntilEnd
    }
    #endregion

    #region Fields
    private List<TestClient> clients;
    #endregion

    #region Construction
    public TestScript(TestClient testClient, params TestClient[] testClients)
    {
      this.clients = this.MergeToList(testClient, testClients);
    }
    #endregion

    #region Methods
    public void RunUntil(RunPoints runPoint)
    {
      this.AllClientsJoinGame();

      this.WaitUntilClientsReceiveMessageOfType(typeof(GameSessionReadyToLaunchMessage), this.clients);

      if (runPoint == RunPoints.RunUntilClientsReceiveGameSessionReadyToLaunchMessage)
      {
        return;
      }

      this.SendLaunchMessageFromClients(this.clients);
      this.WaitUntilClientsReceiveMessageOfType(typeof(InitializeGameMessage), this.clients);

      if (runPoint == RunPoints.RunUntilClientsReceiveGameInitializationMessage)
      {
        return;
      }

      this.SendGameInitializationConfirmationFromClients(this.clients);
    }

    private void AllClientsJoinGame()
    {
      foreach (var client in this.clients)
      {
        client.JoinGame();
      }
    }

    public void SendGameInitializationConfirmationFromClients(TestClient client, params TestClient[] clients)
    {
      client.ConfirmGameInitialized();
      for (int i = 0; i < clients.Length; i++)
      {
        clients[i].ConfirmGameInitialized();
      }
    }

    public void SendLaunchMessageFromClients(TestClient client, params TestClient[] clients)
    {
      this.SendLaunchMessageFromClients(this.MergeToList(client, clients));
    }

    public void SendTownPlacementFromClient(TestClient client, UInt32 positionIndex)
    {
      client.SendTownLocation(positionIndex);
    }

    public void WaitUntilClientsReceiveMessageOfType(Type expectedMessageType, TestClient testClient, params TestClient[] testClients)
    {
      var allClients = this.MergeToList(testClient, testClients); 
      this.WaitUntilClientsReceiveMessageOfType(expectedMessageType, allClients);
    }

    private List<TestClient> MergeToList(TestClient testClient, TestClient[] testClients)
    {
      var allClients = new List<TestClient>();
      allClients.Add(testClient);
      if (testClients != null)
      {
        allClients.AddRange(testClients);
      }

      return allClients;
    }

    private void SendGameInitializationConfirmationFromClients(List<TestClient> testClients)
    {
      foreach (var testClient in testClients)
      {
        testClient.ConfirmGameInitialized();
      }
    }

    private void SendLaunchMessageFromClients(List<TestClient> testClients)
    {
      foreach (var testClient in testClients)
      {
        testClient.SendLaunchGameMessage();
      }
    }

    private void WaitUntilClientsReceiveMessageOfType(Type expectedMessageType, List<TestClient> testClients)
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      var clientsWaitingForMessage = new List<TestClient>(testClients);

      while (clientsWaitingForMessage.Count > 0
        && stopWatch.ElapsedMilliseconds <= 1000
        )
      {
        for (var index = 0; index < clientsWaitingForMessage.Count; index++)
        {
          var message = clientsWaitingForMessage[index].GetLastMessage();
          if (message != null && message.GetType() == expectedMessageType)
          {
            clientsWaitingForMessage.RemoveAt(index);
            index--;
          }
        }

        Thread.Sleep(50);
      }

      stopWatch.Stop();

      if (clientsWaitingForMessage.Count > 0)
      {
        var exceptionMessage = String.Format("Timed out waiting for clients to receive message of type '{0}'", expectedMessageType);
        throw new TimeoutException(exceptionMessage);
      }
    }
    #endregion
  }
}
