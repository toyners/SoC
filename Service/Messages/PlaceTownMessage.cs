
namespace Jabberwocky.SoC.Service.Messages
{
  using System;

  internal class PlaceTownMessage : GameSessionMessage
  {
    public readonly UInt32 Location;

    public PlaceTownMessage(IServiceProviderCallback client, UInt32 location) : base(Types.RequestTownPlacement, client)
    {
      this.Location = location;
    }
  }
}
