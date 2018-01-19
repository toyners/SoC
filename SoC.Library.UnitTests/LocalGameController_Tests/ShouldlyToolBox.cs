
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using Shouldly;

  public static class ShouldlyToolBox
  {
    public static void AssertThatTheResourceTransactionListIsAsExpected(ResourceTransactionList actual, ResourceTransactionList expected)
    {
      actual.ShouldNotBeNull();
      expected.ShouldNotBeNull();

      actual.Count.ShouldBe(expected.Count);

      for (var i = 0; i < actual.Count; i++)
      {
        actual[i].ReceivingPlayerId.ShouldBe(expected[i].ReceivingPlayerId);
        actual[i].GivingPlayerId.ShouldBe(expected[i].GivingPlayerId);
        actual[i].Resources.ShouldBe(expected[i].Resources);
      }
    }
  }
}
