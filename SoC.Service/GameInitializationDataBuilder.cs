
namespace Jabberwocky.SoC.Service
{
  using System;
  using Library;

  public static class GameInitializationDataBuilder
  {
    public static GameInitializationData Build(Board board)
    {
      // Standard board only
      var boardData = new Byte[Board.StandardBoardResourceProviderCount];

      for (Int32 index = 0; index < Board.StandardBoardResourceProviderCount; index++)
      {
        boardData[index] = CreateDataForProvider(board.Providers[index]);
      }

      return new GameInitializationData()
      {
        BoardData = boardData
      };
    }

    private static Byte CreateDataForProvider(ResourceProvider provider)
    {
      return (Byte)((provider.ProductionNumber * 10) + TranslateProviderTypeToNumber(provider.Type));
    }

    private static Byte TranslateProviderTypeToNumber(ResourceTypes type)
    {
      switch (type)
      {
        case ResourceTypes.Brick: return 1;
        case ResourceTypes.Grain: return 2;
        case ResourceTypes.Lumber: return 3;
        case ResourceTypes.Ore: return 4;
        case ResourceTypes.Wool: return 5;
      }

      return 0;
    }
  }
}
