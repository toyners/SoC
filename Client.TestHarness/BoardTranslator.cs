
namespace Client.TestHarness
{
  using System;
  using Jabberwocky.SoC.Library;

  public class BoardTranslator
  {
    public String Content
    {
      get
      {
        return "<html><header></header><body>Hello!<body></html>";
      }
    }

    public BoardTranslator(Board board)
    {
    }
  }
}
