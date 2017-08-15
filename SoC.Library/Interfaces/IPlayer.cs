
namespace Jabberwocky.SoC.Library.Interfaces
{
  using System;
  
  public interface IPlayer
  {
    #region Properties
    Guid Id { get; }
    String Name { get; }
    Int32 ResourcesCount { get; }
    #endregion

    #region Methods
    PlayerDataView GetDataView();
    void RemoveResources(ResourceClutch resourceClutch);
    #endregion
  }
}
