
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.IO;
  using System.ServiceModel;
  using Logging;
  //using NLog;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Single)]
  public class ServiceProvider : IServiceProvider
  {
    #region Fields
    private GameConnector gameConnector;

    //private Logger logger = LogManager.GetCurrentClassLogger();

    private Guid id = Guid.NewGuid();

    private StreamWriter writer;
    #endregion

    #region Construction
    public ServiceProvider()
    {
      this.writer = new StreamWriter(@"C:\projects\soc_" + id.ToString() + ".log");
      this.writer.WriteLine("Initiating Service Provider: " + id.ToString());
      this.writer.AutoFlush = true;
      this.gameConnector = new GameConnector();
      this.gameConnector.StartMatching();
    }
    #endregion

    #region Methods
    public void TryJoinGame()
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameConnector.AddClient(client);
      this.writer.WriteLine("Client joined game: " + id.ToString());
    }

    public void LeaveGame(Guid gameToken)
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameConnector.RemoveClient(gameToken, client);
      //this.logger.Info("Client left game");
    }
    #endregion
  }
}
