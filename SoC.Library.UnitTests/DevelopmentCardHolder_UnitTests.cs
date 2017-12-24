
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
    #endregion 
  }
}
