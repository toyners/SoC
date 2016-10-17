﻿
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.Serialization;
  using System.ServiceModel;
  using System.Text;

  public class ServiceProvider : IServiceProvider
  {
    private List<IClient> clients;

    public ServiceProvider()
    {
      this.clients = new List<IClient>(4);
    }

    #region Methods
    public Boolean JoinGame()
    {
      if (this.clients.Count == 4)
      {
        return false; // Game is full
      }

      var client = OperationContext.Current.GetCallbackChannel<IClient>();

      if (!this.clients.Contains(client))
      {
        this.clients.Add(client);
      }

      return true;
    }

    public void LeaveGame()
    {
      throw new NotImplementedException();
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
