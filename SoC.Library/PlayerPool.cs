
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.IO;
  using System.Xml;
  using Interfaces;

  public class PlayerPool : IPlayerPool
  {
    public IPlayer Create()
    {
      throw new NotImplementedException();
      //return new ComputerPlayer(Guid.NewGuid());
    }

    /// <summary>
    /// Create a player instance from stream data.
    /// </summary>
    /// <param name="stream">Stream containing player data.</param>
    /// <returns>Player instance</returns>
    public IPlayer CreatePlayer(Stream stream)
    {
      throw new NotImplementedException();
      /*using (var reader = XmlReader.Create(stream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, CloseInput = false, IgnoreComments = true, IgnoreWhitespace = true }))
      {
        while (!reader.EOF)
        {
          if (reader.Name == "player" && reader.NodeType == XmlNodeType.Element)
          {
            return this.CreatePlayer(reader);
          }

          reader.Read();
        }
      }*/
    }

    public IPlayer GetPlayer()
    {
      throw new NotImplementedException();
    }

    internal IPlayer CreatePlayer(XmlReader reader)
    {
      throw new NotImplementedException();
    }
  }
}
