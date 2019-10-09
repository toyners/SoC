
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
            var boardData = new byte[GameBoard.StandardBoardHexCount];
            var hexInformation = board.Data.GetHexData();
            for (var index = 0; index < GameBoard.StandardBoardHexCount; index++)
            {
                boardData[index] = CreateDataForProvider(hexInformation[index]);
            }

            return new GameInitializationData()
            {
                BoardData = boardData
            };
        }

        private static byte CreateDataForProvider(HexInformation resourceProducer)
        {
            return (byte)((resourceProducer.ProductionFactor * 10) + TranslateProviderTypeToNumber(resourceProducer.ResourceType));
        }

        private static byte TranslateProviderTypeToNumber(ResourceTypes? type)
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
