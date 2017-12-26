
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using Interfaces;

  public class DevelopmentCardHolder : IDevelopmentCardHolder
  {
    #region Fields
    private Queue<DevelopmentCard> cards;
    #endregion

    #region Construction
    public DevelopmentCardHolder()
    {
      this.cards = new Queue<DevelopmentCard>();

      var knightCardCount = 14;
      var roadBuildingCardCount = 2;
      var yearOfPlentyCardCount = 2;
      var monopolyCardCount = 2;
      var victoryPointCardCount = 5;
       
      var rand = new Random((Int32)DateTime.Now.Ticks);
      var victoryPointCardTitles = new Queue<String>(new[] { "Chapel", "Great Hall", "Library", "Market", "University" });

      var allCardCount = 0;
      do
      {
        allCardCount = knightCardCount + roadBuildingCardCount + yearOfPlentyCardCount + monopolyCardCount + victoryPointCardCount;
        var number = rand.Next(0, allCardCount);
        if (number < knightCardCount)
        {
          var card = new KnightDevelopmentCard();
          this.cards.Enqueue(card);
          knightCardCount--;
        }
        else if (number < (knightCardCount + roadBuildingCardCount))
        {
          var card = new RoadBuildingDevelopmentCard();
          this.cards.Enqueue(card);
          roadBuildingCardCount--;
        }
        else if (number < (knightCardCount + roadBuildingCardCount + yearOfPlentyCardCount))
        {
          var card = new YearOfPlentyDevelopmentCard();
          this.cards.Enqueue(card);
          yearOfPlentyCardCount--;
        }
        else if (number < (knightCardCount + roadBuildingCardCount + yearOfPlentyCardCount + monopolyCardCount))
        {
          var card = new MonopolyDevelopmentCard();
          this.cards.Enqueue(card);
          monopolyCardCount--;
        }
        else if (number < allCardCount)
        {
          var title = victoryPointCardTitles.Dequeue();
          var card = new VictoryPointDevelopmentCard(title);
          this.cards.Enqueue(card);
          victoryPointCardCount--;
        }

      } while (allCardCount > 0);
    }
    #endregion

    #region Properties
    public Boolean HasCards { get { return this.cards.Count > 0; } }
    #endregion

    #region Methods
    public Boolean TryGetNextCard(out DevelopmentCard card)
    {
      card = null;
      if (!this.HasCards)
      {
        return false;
      }

      card = this.cards.Dequeue();
      return true;
    }
    #endregion
  }
}
