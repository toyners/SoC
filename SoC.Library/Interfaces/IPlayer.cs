
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  
  public interface IPlayer
  {
    #region Properties
    Guid Id { get; }
    String Name { get; }
    #endregion

    #region Methods
    PlayerDataView GetDataView();
    #endregion
  }
}
