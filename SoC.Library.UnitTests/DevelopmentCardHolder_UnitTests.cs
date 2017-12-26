
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
    public void Cstr_KnightCardsFirst_ReturnsValid()
    {
      var numberSequencer = Substitute.For<DevelopmentCardHolder.IRandom>();
      numberSequencer.Next(Arg.Any<Int32>(), Arg.Any<Int32>()).Returns(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

      var developmentCardHolder = new DevelopmentCardHolder(numberSequencer);

      var i = 0;
      for (; i < 15; i++)
      {
        DevelopmentCard developmentCard;
        developmentCardHolder.TryGetNextCard(out developmentCard).ShouldBeTrue();
        developmentCard.Type.ShouldBe(DevelopmentCardTypes.Knight);
      }

    }
    #endregion 
  }
}
