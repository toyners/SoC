
namespace Jabberwocky.SoC.Client.Console
{
  using System;
  using Jabberwocky.SoC.Client.Console.ServiceReference;

  public class Client : IServiceProviderCallback
  {
    public void ConfirmGameJoined(Guid gameId)
    {
      throw new NotImplementedException();
    }

    public void StartTurn(Guid token)
    {
      Console.WriteLine(String.Format("Received token {0} for turn.", token));
    }
  }
}
