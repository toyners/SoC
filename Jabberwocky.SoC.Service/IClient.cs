
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.ServiceModel;
  using System.Text;
  using System.Threading.Tasks;

  public interface IClient
  {
    [OperationContract(IsOneWay = true)]
    void StartTurn(Guid token);
  }
}
