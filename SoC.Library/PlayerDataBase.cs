
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public class PlayerDataBase
  {
    public Guid Id { get; protected set; }
    public List<DevelopmentCard> DisplayedDevelopmentCards { get; protected set; }
  }
}
