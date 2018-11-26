﻿
namespace SoC.Test
{
  using System;
  using System.ServiceModel;
  using System.Threading;
  using System.Threading.Tasks;
  using Jabberwocky.SoC.Client;
  using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.PlayerData;
    using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class TestClass
  {
    #region Methods
    [Test]
    public void Test()
    {
      var remoteGameController = new RemoteGameController();
      PlayerDataModel[] players = null;
      remoteGameController.GameJoinedEvent = (PlayerDataModel[] p) => { players = p; };

      var cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = cancellationTokenSource.Token;
      var serviceTask = Task.Factory.StartNew(() => 
      {
        var serviceHost = new ServiceHost(typeof(Jabberwocky.SoC.Service.ServiceProvider));
        serviceHost.Open();

        while (!cancellationToken.IsCancellationRequested)
        {
          Thread.Sleep(500);
        }
      }, cancellationToken);

      Task.Factory.StartNew(() =>
      {
        remoteGameController.JoinGame(null);
      });

      while (players == null)
      {
        Thread.Sleep(1000);
      }

      cancellationTokenSource.Cancel();

      players.ShouldNotBeNull();
    }
    #endregion 
  }
}
