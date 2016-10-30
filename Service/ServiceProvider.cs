
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;
  using Logging;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class ServiceProvider : IServiceProvider
  {
    #region Fields
    private GameConnector gameConnector;

    private ILog log;
    #endregion

    #region Construction
    public ServiceProvider()
    {
      //TODO: null reference check
      this.log = log;

      //this.log.Message("Initiating Service Provider");
      this.gameConnector = new GameConnector();
      this.gameConnector.StartMatching();
    }
    #endregion

    #region Methods
    public void TryJoinGame()
    {
      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();
      this.gameConnector.AddClient(client);
    }

    public void LeaveGame()
    {
      Console.WriteLine("Client left game");
    }
    #endregion

    /*public string GetData(int value)
    {
      return string.Format("You entered: {0}", value);
    }

    public CompositeType GetDataUsingDataContract(CompositeType composite)
    {
      if (composite == null)
      {
        throw new ArgumentNullException(nameof(composite));
      }
      if (composite.BoolValue)
      {
        composite.StringValue += "Suffix";
      }
      return composite;
    }*/
  }
}
