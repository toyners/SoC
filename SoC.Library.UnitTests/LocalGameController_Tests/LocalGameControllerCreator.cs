
namespace Jabberwocky.SoC.Library.UnitTests
{
  using GameBoards;
  using Interfaces;

  public class LocalGameControllerCreator
  {
    #region Fields
    private IPlayerPool playerPool;
    private INumberGenerator dice;
    private GameBoardManager gameBoardManager;
    private IDevelopmentCardHolder developmentCardHolder;
    #endregion

    #region Contruction
    public LocalGameControllerCreator()
    {
      this.playerPool = new PlayerPool();
      this.dice = new Dice();
      this.gameBoardManager = new GameBoardManager(BoardSizes.Standard);
      this.developmentCardHolder = new DevelopmentCardHolder();
    }
    #endregion

    #region Methods
    public LocalGameControllerCreator ChangePlayerPool(IPlayerPool playerPool)
    {
      this.playerPool = playerPool;
      return this;
    }

    public LocalGameControllerCreator ChangeDice(INumberGenerator dice)
    {
      this.dice = dice;
      return this;
    }

    public LocalGameControllerCreator ChangeGameBoardManager(GameBoardManager gameBoardManager)
    {
      this.gameBoardManager = gameBoardManager;
      return this;
    }

    public LocalGameControllerCreator ChangeDevelopmentCardHolder(IDevelopmentCardHolder developmentCardHolder)
    {
      this.developmentCardHolder = developmentCardHolder;
      return this;
    }

    public LocalGameController Create()
    {
      return new LocalGameController(this.dice, this.playerPool, this.gameBoardManager, this.developmentCardHolder);
    }
    #endregion
  }
}
