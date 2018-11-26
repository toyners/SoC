
namespace Jabberwocky.SoC.Library.PlayerData
{
  using System;

  public class PlayerDataOld : PlayerDataBase
  {
    public DevelopmentCard[] DevelopmentCards { get; private set; }

    public UInt32 BrickCount { get; private set; }
    public UInt32 GrainCount { get; private set; }
    public UInt32 LumberCount { get; private set; }
    public UInt32 OreCount { get; private set; }
    public UInt32 WoolCount { get; private set; }
  }
}
