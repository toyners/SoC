
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class PlayerBase
  {
    DevelopmentCard[] DisplayedDevelopmentCards;
  }

  public class PlayerView : PlayerBase
  {
    UInt32 ResourceCards;
    UInt32 HiddenDevelopmentCards;
  }
}
