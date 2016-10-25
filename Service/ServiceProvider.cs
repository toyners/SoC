
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Single)]
  public class ServiceProvider : IServiceProvider
  {
    #region Fields
    private List<IServiceProviderCallback> clients;

    private Guid gameId;
    #endregion

    #region Construction
    public ServiceProvider()
    {
      this.clients = new List<IServiceProviderCallback>(4);
      this.gameId = Guid.NewGuid();
    }
    #endregion

    #region Methods
    public Boolean TryJoinGame()
    {
      if (this.clients.Count == 4)
      {
        return false; // Game is full
      }

      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();

      if (!this.clients.Contains(client))
      {
        this.clients.Add(client);
        client.ConfirmGameJoined(this.gameId);
      }

      return true;
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
