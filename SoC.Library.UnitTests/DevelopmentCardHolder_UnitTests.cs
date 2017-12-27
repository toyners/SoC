
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("Äll")]
  [Category("DevelopmentCardHolder")]
  public class DevelopmentCardHolder_UnitTests
  {
    #region Methods
    [Test]
    public void HasCards_OnConstruction_ReturnsTrue()
    {
      var developmentCardHolder = new DevelopmentCardHolder();

      developmentCardHolder.HasCards.ShouldBeTrue();
    }

    [Test]
    public void TryGetNextCard_OnConstruction_ReturnsTrue()
    {
      var developmentCardHolder = new DevelopmentCardHolder();

      DevelopmentCard developmentCard;
      var result = developmentCardHolder.TryGetNextCard(out developmentCard);
      
      result.ShouldBeTrue();
      developmentCard.ShouldNotBeNull();
    }

    [Test]
    public void Cstr_TakeFirstCardEachTime_ReturnsDefaultOrder()
    {
      var index = -1;
      var numberSequencer = Substitute.For<DevelopmentCardHolder.IIndexSequence>();
      var indexes = new System.Collections.Generic.Queue<Int32>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 });
      numberSequencer
        .TryGetNextIndex(out index)
        .Returns(x => {
          if (indexes.Count > 0)
          {
            x[0] = indexes.Dequeue();
            return true;
          }

          x[0] = -1;
          return false;
        });

      var developmentCardHolder = new DevelopmentCardHolder(numberSequencer);

      for (var i = 0; i < 15; i++)
      {
        DevelopmentCard developmentCard;
        developmentCardHolder.TryGetNextCard(out developmentCard).ShouldBeTrue();
        developmentCard.Type.ShouldBe(DevelopmentCardTypes.Knight);
      }

      for (var i = 0; i < 2; i++)
      {
        DevelopmentCard developmentCard;
        developmentCardHolder.TryGetNextCard(out developmentCard).ShouldBeTrue();
        developmentCard.Type.ShouldBe(DevelopmentCardTypes.Monopoly);
      }

      for (var i = 0; i < 2; i++)
      {
        DevelopmentCard developmentCard;
        developmentCardHolder.TryGetNextCard(out developmentCard).ShouldBeTrue();
        developmentCard.Type.ShouldBe(DevelopmentCardTypes.RoadBuilding);
      }

      for (var i = 0; i < 2; i++)
      {
        DevelopmentCard developmentCard;
        developmentCardHolder.TryGetNextCard(out developmentCard).ShouldBeTrue();
        developmentCard.Type.ShouldBe(DevelopmentCardTypes.YearOfPlenty);
      }

      for (var i = 0; i < 5; i++)
      {
        DevelopmentCard developmentCard;
        developmentCardHolder.TryGetNextCard(out developmentCard).ShouldBeTrue();
        developmentCard.Type.ShouldBe(DevelopmentCardTypes.YearOfPlenty);
      }

      developmentCardHolder.HasCards.ShouldBeFalse();
    }
    #endregion 
  }
}
