
namespace Jabberwocky.SoC.Library
{
  using System;

  public class Market
  {
    UInt32 Buy(ResourceTypes wantedType, ResourceTypes count) { throw new NotImplementedException(); }
  }

  public class Port
  {
    ResourceTypes ReceivingType;

    UInt32 NumberPerUnitReturned;

    UInt32 Trade(UInt32 count) { throw new NotImplementedException(); }
  }
}
