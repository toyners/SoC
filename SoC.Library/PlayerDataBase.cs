
namespace Jabberwocky.SoC.Library
{
  using System;

  public class PlayerDataBase
  {
    public Guid Id { get; private set; }
    public DevelopmentCard[] DisplayedDevelopmentCards { get; private set; }
  }
}
