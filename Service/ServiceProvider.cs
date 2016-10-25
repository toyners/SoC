
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.ServiceModel;

  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                   ConcurrencyMode = ConcurrencyMode.Single)]
  public class ServiceProvider : IServiceProvider
  {
    private List<IServiceProviderCallback> clients;

    public ServiceProvider()
    {
      this.clients = new List<IServiceProviderCallback>(4);
    }

    public static String Message;

    #region Methods
    public Boolean JoinGame()
    {
      if (this.clients.Count == 4)
      {
        return false; // Game is full
      }

      var client = OperationContext.Current.GetCallbackChannel<IServiceProviderCallback>();

      if (!this.clients.Contains(client))
      {
        this.clients.Add(client);
      }

      Message = "Client joined game";

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
