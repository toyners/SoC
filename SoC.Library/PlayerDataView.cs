
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public class PlayerDataView
  {
    public Guid Id { get; set; }
    public List<DevelopmentCardTypes> DisplayedDevelopmentCards { get; set; }
    public UInt32 ResourceCards { get; set; }
    public UInt32 HiddenDevelopmentCards { get; set; }
    public String Name;
  }
}
