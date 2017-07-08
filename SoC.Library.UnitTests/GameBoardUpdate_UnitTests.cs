
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameBoardUpdate_UnitTests
  {
    #region Methods
    [Test]
    [Category("GameBoardUpdate")]
    public void PlusOperator_OneOperandIsNull_ReturnsOtherOperand()
    {
      var operand1 = new GameBoardUpdate();
      GameBoardUpdate operand2 = null;

      var result = operand1 + operand2;

      result.ShouldBe(operand1);
    }

    [Test]
    [Category("GameBoardUpdate")]
    public void PlusOperator_BothOperandsAreNotNull_ReturnsMergeOfAllOperandComponents()
    {
      throw new NotImplementedException();
    }

    [Test]
    [Category("GameBoardUpdate")]
    public void PlusOperator_BothOperandsContainSameDataPoint_MeaningfulExceptionIsThrown()
    {
      throw new NotImplementedException();
    }
    #endregion 
  }
}
