
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;

  public class MockPlayer : Player
  {
    public ResourceBag Resources;

    public MockPlayer(String name) : base(name)
    {
    }

    public override Int32 ResourcesCount
    {
      get
      {
        return this.Resources.Count;
      }
    }
  }
}
