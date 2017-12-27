
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using Interfaces;

  public class DevelopmentCardHolder : IDevelopmentCardHolder
  {
    #region Fields
    private const Int32 KnightCardCount = 14;
    private const Int32 RoadBuildingCardCount = 2;
    private const Int32 YearOfPlentyCardCount = 2;
    private const Int32 MonopolyCardCount = 2;
    private const Int32 VictoryPointCardCount = 5;

    private Queue<DevelopmentCard> cards;
    #endregion

    #region Construction
    public DevelopmentCardHolder()
    {
      this.Initialise(new IndexSequence());
    }

    public DevelopmentCardHolder(IIndexSequence random)
    {
      this.Initialise(random);
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

    private void Initialise(IIndexSequence random)
    {
      this.cards = new Queue<DevelopmentCard>();

      var victoryPointCardTitles = new Queue<String>(new[] { "Chapel", "Great Hall", "Library", "Market", "University" });

      var index = -1;
      while (random.TryGetNextIndex(out index))
      {
        if (index < KnightCardCount)
        {
          var card = new KnightDevelopmentCard();
          this.cards.Enqueue(card);
        }
        else if (index < (KnightCardCount + MonopolyCardCount))
        {
          var card = new MonopolyDevelopmentCard();
          this.cards.Enqueue(card);
        }
        else if (index < (KnightCardCount + MonopolyCardCount + RoadBuildingCardCount))
        {
          var card = new RoadBuildingDevelopmentCard();
          this.cards.Enqueue(card);
        }
        else if (index < (KnightCardCount + MonopolyCardCount + RoadBuildingCardCount + YearOfPlentyCardCount))
        {
          var card = new YearOfPlentyDevelopmentCard();
          this.cards.Enqueue(card);
        }
        else if (index < (KnightCardCount + MonopolyCardCount + RoadBuildingCardCount + YearOfPlentyCardCount + VictoryPointCardCount))
        {
          var title = victoryPointCardTitles.Dequeue();
          var card = new VictoryPointDevelopmentCard(title);
          this.cards.Enqueue(card);
        }
      }
    }
    #endregion

    #region Structures
    public interface IIndexSequence
    {
      Boolean TryGetNextIndex(out Int32 index);
    }

    private class IndexSequence : IIndexSequence
    {
      private Queue<Int32> numbers = new Queue<Int32>();

      public IndexSequence()
      {
        Random random = new Random((Int32)DateTime.Now.Ticks);
        var numbersSelected = new HashSet<Int32>();
        
        while (numbersSelected.Count < 25)
        {
          var index = random.Next(0, 25);
          if (!numbersSelected.Contains(index))
          {
            this.numbers.Enqueue(index);
            numbersSelected.Add(index);
          }
        }
      }

      public Boolean TryGetNextIndex(out Int32 index)
      {
        index = -1;
        if (this.numbers.Count == 0)
        {
          return false;
        }

        index = this.numbers.Dequeue();
        return true;
      }
    }
    #endregion
  }
}
