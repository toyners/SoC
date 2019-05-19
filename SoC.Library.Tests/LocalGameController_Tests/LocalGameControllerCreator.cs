
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using Interfaces;

  [Obsolete("Deprecated. Use LocalGameControllerTestCreator instead.")]
  public class LocalGameControllerCreator
  {
    #region Fields
    private IPlayerFactory playerPool;
    private INumberGenerator dice;
    private GameBoard gameBoard;
    private IDevelopmentCardHolder developmentCardHolder;
    #endregion

    #region Contruction
    public LocalGameControllerCreator()
    {
      this.playerPool = new PlayerPool();
      this.dice = new Dice();
      this.gameBoard = new GameBoard(BoardSizes.Standard);
      this.developmentCardHolder = new DevelopmentCardHolder();
    }
    #endregion

    #region Methods
    public LocalGameControllerCreator ChangePlayerPool(IPlayerFactory playerPool)
    {
      this.playerPool = playerPool;
      return this;
    }

    public LocalGameControllerCreator ChangeDice(INumberGenerator dice)
    {
      this.dice = dice;
      return this;
    }

    public LocalGameControllerCreator ChangeGameBoard(GameBoard gameBoard)
    {
      this.gameBoard = gameBoard;
      return this;
    }

    public LocalGameControllerCreator ChangeDevelopmentCardHolder(IDevelopmentCardHolder developmentCardHolder)
    {
      this.developmentCardHolder = developmentCardHolder;
      return this;
    }

    public LocalGameController Create()
    {
      return new LocalGameController(this.dice, this.playerPool, this.gameBoard, this.developmentCardHolder);
    }
    #endregion
  }
}
