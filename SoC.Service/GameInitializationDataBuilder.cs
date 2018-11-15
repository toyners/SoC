
namespace Jabberwocky.SoC.Service
{
  using System;
  using Library;
  using Library.GameBoards;

  public static class GameInitializationDataBuilder
  {
    public static GameInitializationData Build(GameBoardManager board)
    {
      // Standard board only
      var boardData = new Byte[GameBoard.StandardBoardHexCount];
      var hexInformation = board.Data.GetHexData();
      for (Int32 index = 0; index < GameBoard.StandardBoardHexCount; index++)
      {
        boardData[index] = CreateDataForProvider(hexInformation[index]);
      }

      return new GameInitializationData()
      {
        BoardData = boardData
      };
    }

    private static Byte CreateDataForProvider(Tuple<ResourceTypes?, UInt32> resourceProducer)
    {
      return (Byte)((resourceProducer.Item2 * 10) + TranslateProviderTypeToNumber(resourceProducer.Item1));
    }

    private static Byte TranslateProviderTypeToNumber(ResourceTypes? type)
    {
      if (type != null)
      { 
        switch (type)
        {
          case ResourceTypes.Brick: return 1;
          case ResourceTypes.Grain: return 2;
          case ResourceTypes.Lumber: return 3;
          case ResourceTypes.Ore: return 4;
          case ResourceTypes.Wool: return 5;
        }
      }

      return 0;
    }
  }
}
