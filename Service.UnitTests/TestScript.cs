
namespace Service.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Messages;

  public class TestScript
  {
    public enum RunPoints
    {
      RunUntilClientsReceiveGameSessionReadyToLaunchMessage,
      RunUntilClientsReceiveGameInitializationMessage,
      RunUntilEnd
    }

    #region Fields
    private TestClient testPlayer1;
    private TestClient testPlayer2;
    private TestClient testPlayer3;
    private TestClient testPlayer4;
    #endregion

    #region Construction
    public TestScript(TestClient testPlayer1, TestClient testPlayer2, TestClient testPlayer3, TestClient testPlayer4)
    {
      this.testPlayer1 = testPlayer1;
      this.testPlayer2 = testPlayer2;
      this.testPlayer3 = testPlayer3;
      this.testPlayer4 = testPlayer4;
    }
    #endregion

    #region Methods
    public void RunUntil(RunPoints runPoint)
    {
      this.testPlayer1.JoinGame();
      this.testPlayer2.JoinGame();
      this.testPlayer3.JoinGame();
      this.testPlayer4.JoinGame();

      this.WaitUntilClientsReceiveMessageOfType(typeof(GameSessionReadyToLaunchMessage), this.testPlayer1, this.testPlayer2, this.testPlayer3, this.testPlayer4);

      if (runPoint == RunPoints.RunUntilClientsReceiveGameSessionReadyToLaunchMessage)
      {
        return;
      }

      this.SendLaunchMessageFromClients(this.testPlayer1, this.testPlayer2, this.testPlayer3, this.testPlayer4);
      this.WaitUntilClientsReceiveMessageOfType(typeof(InitializeGameMessage), this.testPlayer1, this.testPlayer2, this.testPlayer3, this.testPlayer4);

      if (runPoint == RunPoints.RunUntilClientsReceiveGameInitializationMessage)
      {
        return;
      }

      this.SendGameInitializationConfirmationFromClients(this.testPlayer1, this.testPlayer2, this.testPlayer3, this.testPlayer4);
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
      client.SendLaunchGameMessage();
      for (int i = 0; i < clients.Length; i++)
      {
        clients[i].SendLaunchGameMessage();
      }
    }

    public void SendTownPlacementFromClient(TestClient client, UInt32 positionIndex)
    {
      client.SendTownLocation(positionIndex);
    }

    public void WaitUntilClientsReceiveMessageOfType(Type expectedMessageType, params TestClient[] testClients)
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
