
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public class PlayerDataModel
  {
    public Guid Id { get; set; }
    public bool IsComputer { get; set; }
    public List<DevelopmentCardTypes> DisplayedDevelopmentCards { get; set; }
    public int ResourceCards { get; set; }
    public int HiddenDevelopmentCards { get; set; }
    public string Name;
    public bool HasLongestRoad;
    public int LongestRoadSegmentCount;
  }
}
