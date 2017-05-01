
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class Location
  {
    public readonly HashSet<Trail> Trails;

    public readonly HashSet<ResourceProvider> Providers; // Either 2 or 3

    #region Construction
    public Location()
    {
      this.Trails = new HashSet<Trail>();
      this.Providers = new HashSet<ResourceProvider>();
    }
    #endregion

    #region Methods
    public void AddTrail(Trail trail)
    {
      this.Trails.Add(trail);
    }
    #endregion
  }
}
