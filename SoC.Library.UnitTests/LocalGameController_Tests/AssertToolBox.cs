
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using System.Collections.Generic;
  using GameEvents;
  using Shouldly;

  public static class AssertToolBox
  {
    public static void AssertThatPlayerActionsForTurnAreCorrect(List<GameEvent> actualEvents, params GameEvent[] expectedEvents)
    {
      actualEvents.Count.ShouldBe(expectedEvents.Length);
      for (var index = 0; index < actualEvents.Count; index++)
      {
        actualEvents[index].ShouldBe(expectedEvents[index], "Index is " + index);
      }
    }

    public static void AssertThatResourceCollectionsAreTheSame(Dictionary<Guid, ResourceCollection[]> actual, Dictionary<Guid, ResourceCollection[]> expected)
    {
      actual.Count.ShouldBe(expected.Count);
      List<Guid> expectedKeys = new List<Guid>(expected.Keys);
      expectedKeys.Sort();

      foreach (var guid in expectedKeys)
      {
        actual.ShouldContainKey(guid);
        var actualList = new List<ResourceCollection>(actual[guid]);
        var expectedList = new List<ResourceCollection>(expected[guid]);

        actualList.Count.ShouldBe(expectedList.Count);
        actualList.Sort();
        expectedList.Sort();

        for (var i = 0; i < expectedList.Count; i++)
        {
          actualList[i].Location.ShouldBe(expectedList[i].Location);
          actualList[i].Resources.ShouldBe(expectedList[i].Resources);
        }

        actual.Remove(guid);
      }

      actual.Count.ShouldBe(0);
    }

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
