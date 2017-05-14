
namespace SoC.Test
{
  using System;
  using System.ServiceModel;
  using System.Threading;
  using System.Threading.Tasks;
  using Jabberwocky.SoC.Client;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Service.Console;
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
      PlayerBase[] players = null;
      remoteGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };

      //Task.Factory.StartNew(() => { Program.Main(null); }); 
      Task.Factory.StartNew(() => 
      {
        var serviceHost = new ServiceHost(typeof(Jabberwocky.SoC.Service.ServiceProvider));
        serviceHost.Open();
      });

      Task.Factory.StartNew(() =>
      {
        remoteGameController.StartJoiningGame(null);
      });

      while (players == null)
      {
        Thread.Sleep(1000);
      }

      players.ShouldNotBeNull();
    }
    #endregion 
  }
}
