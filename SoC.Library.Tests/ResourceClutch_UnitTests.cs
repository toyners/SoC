
namespace Jabberwocky.SoC.Library.UnitTests
{
    using System;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("All")]
    [Category("ResourceClutch")]
    public class ResourceClutch_UnitTests
    {
        #region Methods
        [Test]
        public void EqualsOperator_BothParametersAreZero_ReturnsTrue()
        {
            (ResourceClutch.Zero == ResourceClutch.Zero).ShouldBeTrue();
        }

        [Test]
        public void EqualsOperator_ResourceCountsAreDifferent_ReturnsFalse()
        {
            var r1 = new ResourceClutch(1, 2, 3, 4, 5);
            var r2 = new ResourceClutch(1, 2, 3, 4, 0);

            (r1 == r2).ShouldBeFalse();
        }

        [Test]
        public void NotEqualsOperator_ResourceCountsAreDifferent_ReturnsTrue()
        {
            var r1 = new ResourceClutch(1, 2, 3, 4, 5);
            var r2 = new ResourceClutch(1, 2, 3, 4, 0);

            (r1 != r2).ShouldBeTrue();
        }

        [Test]
        public void EqualsOperator_ResourceCountsAreSame_ReturnsTrue()
        {
            var r1 = new ResourceClutch(1, 2, 3, 4, 5);
            var r2 = new ResourceClutch(1, 2, 3, 4, 5);

            (r1 == r2).ShouldBeTrue();
        }

        [Test]
        public void NotEqualsOperator_ResourceCountsAreSame_ReturnsFalse()
        {
            var r1 = new ResourceClutch(1, 2, 3, 4, 5);
            var r2 = new ResourceClutch(1, 2, 3, 4, 5);

            (r1 != r2).ShouldBeFalse();
        }

        [Test]
        public void MultiplyByNaturalNumberOperator_FirstOperandIsZero_ResultIsZero()
        {
            var op1 = new ResourceClutch(1, 1, 1, 1, 1);
            var op2 = 0;

            (op1 * op2).ShouldBe(ResourceClutch.Zero);
        }

        [Test]
        public void MultiplyByNaturalNumberOperator_SecondOperandIsZero_ResultIsZero()
        {
            var op1 = new ResourceClutch(1, 1, 1, 1, 1);
            var op2 = 0;

            (op2 * op1).ShouldBe(ResourceClutch.Zero);
        }

        [Test]
        public void MultiplyByNaturalNumberOperator_FirstOperandIsPositive_ResultHasExpectedCounts()
        {
            var op1 = new ResourceClutch(1, 1, 1, 1, 1);
            var op2 = 2;
            var expectedResult = new ResourceClutch(2, 2, 2, 2, 2);

            (op2 * op1).ShouldBe(expectedResult);
        }

        [Test]
        public void MultiplyByNaturalNumberOperator_SecondOperandIsPositive_ResultHasExpectedCounts()
        {
            var op1 = new ResourceClutch(1, 1, 1, 1, 1);
            var op2 = 2;
            var expectedResult = new ResourceClutch(2, 2, 2, 2, 2);

            (op1 * op2).ShouldBe(expectedResult);
        }

        [Test]
        public void MultiplyByNaturalNumberOperator_FirstOperandIsNegative_ThrowsMeaningFulException()
        {
            var op1 = new ResourceClutch(1, 1, 1, 1, 1);
            var op2 = -2;

            Should.Throw<ArgumentOutOfRangeException>(() => { var r = op1 * op2; })
              .Message.ShouldBe("Must be a natural number\r\nParameter name: operand2\r\nActual value was -2.");
        }

        [Test]
        public void MultiplyByNaturalNumberOperator_SecondOperandIsNegative_ThrowsMeaningFulException()
        {
            var op1 = new ResourceClutch(1, 1, 1, 1, 1);
            var op2 = -2;

            Should.Throw<ArgumentOutOfRangeException>(() => { var r = op2 * op1; })
              .Message.ShouldBe("Must be a natural number\r\nParameter name: operand1\r\nActual value was -2.");
        }

        [Test]
        [TestCase(ResourceTypes.Brick, 1, 0, 0, 0, 0)]
        [TestCase(ResourceTypes.Grain, 0, 1, 0, 0, 0)]
        [TestCase(ResourceTypes.Lumber, 0, 0, 1, 0, 0)]
        [TestCase(ResourceTypes.Ore, 0, 0, 0, 1, 0)]
        [TestCase(ResourceTypes.Wool, 0, 0, 0, 0, 1)]
        public void CreateFromResourceType_SetToResourceType_CreatesOneOfResourceType(ResourceTypes resourceType, Int32 expectedBrickCount, Int32 expectedGrainCount, Int32 expectedLumberCount, Int32 expectedOreCount, Int32 expectedWoolCount)
        {
            var result = ResourceClutch.CreateFromResourceType(resourceType);

            result.BrickCount.ShouldBe(expectedBrickCount);
            result.GrainCount.ShouldBe(expectedGrainCount);
            result.LumberCount.ShouldBe(expectedLumberCount);
            result.OreCount.ShouldBe(expectedOreCount);
            result.WoolCount.ShouldBe(expectedWoolCount);
        }

        [Test]
        public void Subtraction_GreaterMinusLesser_CreatesCorrectResourceClutch()
        {
            var result = ResourceClutch.OneBrick - ResourceClutch.Zero;

            result.ShouldBe(ResourceClutch.OneBrick);
        }
        #endregion
    }
}
