
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public class PlayerDataView
  {
    public Guid Id { get; set; }
    public List<DevelopmentCard> DisplayedDevelopmentCards { get; set; }
    public UInt32 ResourceCards { get; set; }
    public Int32 HiddenDevelopmentCards { get; set; }
  }
}
