
namespace Jabberwocky.SoC.Library
{
  using System;

  public class Market
  {
    UInt32 Buy(ResourceTypes wantedType, ResourceTypes count) { throw new NotImplementedException(); }
  }

  public class DevelopmentCard { DevelopmentCardTypes Type; }

  public class DevelopmentCardPile
  {
    void LoadCards() { }

    Boolean HasCards;

    DevelopmentCard GetCard() { throw new NotImplementedException(); }
  }

  public class Settlement
  {
    PlayerData Owner;
  }

  public class City
  { }

  public class Port
  {
    ResourceTypes ReceivingType;

    UInt32 NumberPerUnitReturned;

    UInt32 Trade(UInt32 count) { throw new NotImplementedException(); }
  }

  public class Road
  {
    Location Location1;
    Location Location2;
  }
}
