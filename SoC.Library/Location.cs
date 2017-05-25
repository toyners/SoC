
namespace Jabberwocky.SoC.Library
{
  using System.Collections.Generic;

  public class Location
  {
    public Trail[] Trails;

    public readonly HashSet<ResourceProvider> Providers; // 1, 2 or 3

    #region Construction
    public Location()
    {
      this.Providers = new HashSet<ResourceProvider>();
    }
    #endregion

    #region Methods
    public void AddTrail(Trail trail)
    {
      if (this.Trails == null)
      {
        this.Trails = new Trail[1];
        this.Trails[0] = trail;
      }
      else
      {
        var trails = new Trail[this.Trails.Length + 1];
        this.Trails.CopyTo(trails, 0);
        trails[trails.Length - 1] = trail;
        this.Trails = trails;
      }
    }
    #endregion
  }
}
