
namespace Jabberwocky.SoC.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.Serialization;
  using System.Text;
  using System.Threading.Tasks;
  using Library;

  [DataContract]
  public class GameInitializationData
  {
    [DataMember]
    public Byte[] BoardData { get; set; }

    [DataMember]
    public Byte ColumnCount { get; set; }

    [DataMember]
    public Byte FirstColumnCount { get; set; }
  }

  public static class GameInitializationDataBuilder
  {
    public static GameInitializationData Build(Board board)
    {
      // Standard board only
      var gameData = new GameInitializationData()
      {
        ColumnCount = 5,
        FirstColumnCount = 3
      };

      return gameData;
    }
  }

  // Use a data contract as illustrated in the sample below to add composite types to service operations.
  // You can add XSD files into the project. After building the project, you can directly use the data types defined there, 
  // with the namespace "Jabberwocky.SoC.Service.ContractType".
  /*[DataContract]
  public class CompositeType
  {
    bool boolValue = true;
    string stringValue = "Hello ";

    [DataMember]
    public bool BoolValue
    {
      get { return boolValue; }
      set { boolValue = value; }
    }

    [DataMember]
    public string StringValue
    {
      get { return stringValue; }
      set { stringValue = value; }
    }
  }*/
}
