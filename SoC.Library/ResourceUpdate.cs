
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public class ResourceUpdate
  {
    public UInt32 BrickCount;
    public UInt32 GrainCount;
    public UInt32 LumberCount;
    public UInt32 OreCount;
    public UInt32 WoolCount;

    public Dictionary<Guid, ResourceUpdate> OtherPlayers;
  }
}
