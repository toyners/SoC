
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public class PlayerDataView
  {
    public Guid Id { get; set; }
    public Boolean IsComputer { get; set; }
    public List<DevelopmentCardTypes> DisplayedDevelopmentCards { get; set; }
    public Int32 ResourceCards { get; set; }
    public Int32 HiddenDevelopmentCards { get; set; }
    public String Name;
  }
}
