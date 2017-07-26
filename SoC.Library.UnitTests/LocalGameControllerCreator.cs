
namespace Jabberwocky.SoC.Library.UnitTests
{
  using GameBoards;
  using Interfaces;

  public class LocalGameControllerCreator
  {
    #region Fields
    private IComputerPlayerFactory computerPlayerFactory;
    private IDice dice;
    private GameBoardManager gameBoardManager;
    #endregion

    #region Contruction
    public LocalGameControllerCreator()
    {
      this.computerPlayerFactory = new ComputerPlayerFactory();
      this.dice = new Dice();
      this.gameBoardManager = new GameBoardManager(BoardSizes.Standard);
    }
    #endregion

    #region Methods
    public LocalGameControllerCreator ChangeComputerPlayerFactory(IComputerPlayerFactory computerPlayerFactory)
    {
      this.computerPlayerFactory = computerPlayerFactory;
      return this;
    }

    public LocalGameControllerCreator ChangeDice(IDice dice)
    {
      this.dice = dice;
      return this;
    }

    public LocalGameControllerCreator ChangeGameBoardManager(GameBoardManager gameBoardManager)
    {
      this.gameBoardManager = gameBoardManager;
      return this;
    }

    public LocalGameController Create()
    {
      return new LocalGameController(this.dice, this.computerPlayerFactory, this.gameBoardManager);
    }
    #endregion
  }
}
