
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using Interfaces;

  public class DevelopmentCardHolder : IDevelopmentCardHolder
  {
    #region Fields
    private const int KnightCardCount = 14;
    private const int RoadBuildingCardCount = 2;
    private const int YearOfPlentyCardCount = 2;
    private const int MonopolyCardCount = 2;
    private const int VictoryPointCardCount = 5;

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
    public bool HasCards { get { return this.cards.Count > 0; } }
    #endregion

    #region Methods
    public bool TryGetNextCard(out DevelopmentCard card)
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

      var victoryPointCardTitles = new Queue<string>(new[] { "Chapel", "Great Hall", "Library", "Market", "University" });

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
      bool TryGetNextIndex(out int index);
    }

    private class IndexSequence : IIndexSequence
    {
      private Queue<int> numbers = new Queue<int>();

      public IndexSequence()
      {
        var random = new Random((int)DateTime.Now.Ticks);
        var numbersSelected = new HashSet<int>();
        
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

      public bool TryGetNextIndex(out int index)
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
