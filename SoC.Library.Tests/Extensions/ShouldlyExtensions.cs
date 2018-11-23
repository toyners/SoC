
namespace Jabberwocky.SoC.Library.UnitTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using Shouldly;

    public static class ShouldlyExtensions
    {
        public static void ShouldBe(this ResourceTransactionList actual, ResourceTransactionList expected)
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

        public static void ShouldContainExact<T>(this IList<T> actualCollection, IList<T> expectedCollection)
        {
            actualCollection.ShouldNotBeNull();
            expectedCollection.ShouldNotBeNull();

            actualCollection.Count.ShouldBe(expectedCollection.Count);

            for (var index = 0; index < actualCollection.Count; index++)
            {
                actualCollection[index].ShouldBe(expectedCollection[index], "Index is " + index);
            }
        }

        public static void ShouldContainExact(this Dictionary<Guid, ResourceCollection[]> actual, Dictionary<Guid, ResourceCollection[]> expected)
        {
            actual.Count.ShouldBe(expected.Count);
            var expectedKeys = new List<Guid>(expected.Keys);
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

        public static void ShouldMatch<T>(this ISet<T> actual, ISet<T> expected)
        {
            expected.ShouldNotBeNull($"ISet<{typeof(T).Name}>: Expected parameter is null");
            actual.Count.ShouldBe(expected.Count, $"ISet<{typeof(T).Name}>: Actual count {actual.Count} does not match expected count {expected.Count}");

            var expectedList = new List<T>(expected);

            foreach(var actualInstance in actual)
            {
                var matchFound = false;
                for(var i = 0; !matchFound && i < expectedList.Count; i++)
                {
                    if (actualInstance.Equals(expectedList[i]))
                    {
                        matchFound = true;
                        expectedList.RemoveAt(i);
                    }
                }

                if (!matchFound)
                {
                    throw new ShouldAssertException($"Actual instance {actualInstance} not found in expected");
                }
            }
        }
    }
}
