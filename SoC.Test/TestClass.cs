
namespace SoC.Test
{
  using System;
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

      Task.Factory.StartNew(() => { Program.Main(null); });

      remoteGameController.StartJoiningGame(null);

      players.ShouldNotBeNull();
    }
    #endregion 
  }
}
