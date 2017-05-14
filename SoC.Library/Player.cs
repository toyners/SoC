
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Interfaces;

  public class PlayerData : PlayerBase
  {
    DevelopmentCard[] HiddenDevelopmentCards;

    UInt32 OreCount;
    UInt32 WheatCount;
    UInt32 SheepCount;
    UInt32 LumberCount;
    UInt32 BrickCount;
  }
}
