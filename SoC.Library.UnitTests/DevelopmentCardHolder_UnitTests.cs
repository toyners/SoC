
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
    public void Cstr_InAscendingOrder_ReturnsExpectedOrder()
    {
      var index = -1;
      var indexes = new System.Collections.Generic.Queue<Int32>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 });
      var numberSequencer = Substitute.For<DevelopmentCardHolder.IIndexSequence>();
      numberSequencer
        .TryGetNextIndex(out index)
        .ReturnsForAnyArgs(x => {
          if (indexes.Count > 0)
          {
            x[0] = indexes.Dequeue();
            return true;
          }

          x[0] = -1;
          return false;
        });

      var developmentCardHolder = new DevelopmentCardHolder(numberSequencer);

      for (var i = 0; i < 14; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Knight);
      }

      for (var i = 0; i < 2; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Monopoly);
      }

      for (var i = 0; i < 2; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.RoadBuilding);
      }

      for (var i = 0; i < 2; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.YearOfPlenty);
      }

      for (var i = 0; i < 5; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.VictoryPoint);
      }

      developmentCardHolder.HasCards.ShouldBeFalse();
    }

    [Test]
    public void Cstr_InDescendingOrder_ReturnsExpectedOrder()
    {
      var index = -1;
      var indexes = new System.Collections.Generic.Queue<Int32>(new[] { 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 });
      var numberSequencer = Substitute.For<DevelopmentCardHolder.IIndexSequence>();
      numberSequencer
        .TryGetNextIndex(out index)
        .ReturnsForAnyArgs(x => {
          if (indexes.Count > 0)
          {
            x[0] = indexes.Dequeue();
            return true;
          }

          x[0] = -1;
          return false;
        });

      var developmentCardHolder = new DevelopmentCardHolder(numberSequencer);

      for (var i = 0; i < 5; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.VictoryPoint);
      }

      for (var i = 0; i < 2; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.YearOfPlenty);
      }

      for (var i = 0; i < 2; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.RoadBuilding);
      }

      for (var i = 0; i < 2; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Monopoly);
      }

      for (var i = 0; i < 14; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Knight);
      }

      developmentCardHolder.HasCards.ShouldBeFalse();
    }

    [Test]
    public void Cstr_TakeOneOfEachCardUntilExhausted_ReturnsExpectedOrder()
    {
      var index = -1;
      var indexes = new System.Collections.Generic.Queue<Int32>(new[] { 0, 14, 16, 18, 20, 1, 15, 17, 19, 21, 2, 22, 3, 23, 4, 24, 5, 6, 7, 8, 9, 10, 11, 12, 13 });
      var numberSequencer = Substitute.For<DevelopmentCardHolder.IIndexSequence>();
      numberSequencer
        .TryGetNextIndex(out index)
        .ReturnsForAnyArgs(x => {
          if (indexes.Count > 0)
          {
            x[0] = indexes.Dequeue();
            return true;
          }

          x[0] = -1;
          return false;
        });

      var developmentCardHolder = new DevelopmentCardHolder(numberSequencer);

      for (var i = 0; i < 2; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Knight);
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Monopoly);
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.RoadBuilding);
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.YearOfPlenty);
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.VictoryPoint);
      }

      for (var i = 0; i < 3; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Knight);
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.VictoryPoint);
      }

      for (var i = 0; i < 9; i++)
      {
        this.AssertNextDevelopmentCardIsCorrect(developmentCardHolder, DevelopmentCardTypes.Knight);
      }

      developmentCardHolder.HasCards.ShouldBeFalse();
    }

    private void AssertNextDevelopmentCardIsCorrect(DevelopmentCardHolder developmentCardHolder, DevelopmentCardTypes expectedType)
    {
      DevelopmentCard developmentCard;
      developmentCardHolder.TryGetNextCard(out developmentCard).ShouldBeTrue();
      developmentCard.Type.ShouldBe(expectedType);
    }
    #endregion 
  }
}
