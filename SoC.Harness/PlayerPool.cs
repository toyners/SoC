using System;
using System.Collections.Generic;
using System.Xml;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.Store;

namespace SoC.Harness
{
  public class LocalPlayerPool : IPlayerPool
  {
    private Queue<string> names = new Queue<string>(new[] { "Barbara", "Charlie", "Dana" });

    public IPlayer CreateComputerPlayer(GameBoard gameBoard, LocalGameController localGameController, INumberGenerator numberGenerator)
    {
      return new ComputerPlayer(this.names.Dequeue(), gameBoard, null, numberGenerator);
    }

    public IPlayer CreateComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator)
    {
      throw new NotImplementedException();
    }

    public IPlayer CreatePlayer()
    {
      return new Player("Player");
    }

    public IPlayer CreatePlayer(XmlReader reader)
    {
      throw new NotImplementedException();
    }

    public IPlayer CreatePlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data)
    {
      throw new NotImplementedException();
    }

    public Guid GetBankId()
    {
      throw new NotImplementedException();
    }
  }
}
