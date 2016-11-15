
namespace Jabberwocky.SoC.Service
{
  using Library;

  public static class GameInitializationDataBuilder
  {
    public static GameInitializationData Build(Board board)
    {
      // Standard board only
      var gameData = new GameInitializationData()
      {
        ColumnCount = 5,
        FirstColumnCount = 3
      };

      //board.Locations[0].Providers

      return gameData;
    }
  }
}
