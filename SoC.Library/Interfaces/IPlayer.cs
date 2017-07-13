
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  
  public interface IPlayer
  {
    Guid Id { get; }

    PlayerDataView GetDataView();
  }
}
