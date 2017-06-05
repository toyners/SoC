
namespace Jabberwocky.SoC.Library
{
  using System;

  public class PlayerDataView : PlayerDataBase
  {
    public UInt32 ResourceCards { get; private set; }
    public UInt32 HiddenDevelopmentCards { get; private set; }
  }
}
