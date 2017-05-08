
namespace Service.UnitTests
{
  using System;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.GameBoards;
  using Jabberwocky.SoC.Service;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameInitializationDataBuilder_UnitTests
  {
    #region Methods
    [Test]
    public void Build_StandardBoard_ReturnsCorrectInitializationData()
    {
      // Arrange
      var board = new GameBoard(BoardSizes.Standard);

      // Act
      var data = GameInitializationDataBuilder.Build(board);

      // Assert
      // 0 = desert, 1 = brick, 2 = grain, 3 = lumber, 4 = ore, 5 = wool
      // 20 = 2 on dice, 30 = 3 on dice, 40 = 4 on dice, .... 110 = 11 on dice, 120 = 12 on dice 

      const Byte brick = 1;
      const Byte grain = 2;
      const Byte lumber = 3;
      const Byte ore = 4;
      const Byte wool = 5;

      const Byte diceroll2 = 20;
      const Byte diceroll3 = 30;
      const Byte diceroll4 = 40;
      const Byte diceroll5 = 50;
      const Byte diceroll6 = 60;
      const Byte diceroll8 = 80;
      const Byte diceroll9 = 90;
      const Byte diceroll10 = 100;
      const Byte diceroll11 = 110;
      const Byte diceroll12 = 120;

      data.BoardData.Length.ShouldBe(19);
      data.BoardData[0].ShouldBe<Byte>(0);
      data.BoardData[1].ShouldBe<Byte>(diceroll8 + brick);
      data.BoardData[2].ShouldBe<Byte>(diceroll5 + ore);
      data.BoardData[3].ShouldBe<Byte>(diceroll4 + brick);
      data.BoardData[4].ShouldBe<Byte>(diceroll3 + lumber);
      data.BoardData[5].ShouldBe<Byte>(diceroll10 + wool);
      data.BoardData[6].ShouldBe<Byte>(diceroll2 + grain);
      data.BoardData[7].ShouldBe<Byte>(diceroll11 + lumber);
      data.BoardData[8].ShouldBe<Byte>(diceroll6 + ore);
      data.BoardData[9].ShouldBe<Byte>(diceroll11 + grain);
      data.BoardData[10].ShouldBe<Byte>(diceroll9 + wool);
      data.BoardData[11].ShouldBe<Byte>(diceroll6 + lumber);
      data.BoardData[12].ShouldBe<Byte>(diceroll12 + wool);
      data.BoardData[13].ShouldBe<Byte>(diceroll5 + brick);
      data.BoardData[14].ShouldBe<Byte>(diceroll4 + lumber);
      data.BoardData[15].ShouldBe<Byte>(diceroll3 + ore);
      data.BoardData[16].ShouldBe<Byte>(diceroll9 + grain);
      data.BoardData[17].ShouldBe<Byte>(diceroll10 + wool);
      data.BoardData[18].ShouldBe<Byte>(diceroll8 + grain);
    }
    #endregion 
  }
}
