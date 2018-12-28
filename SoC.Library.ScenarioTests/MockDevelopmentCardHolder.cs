using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    public class MockDevelopmentCardHolder : IDevelopmentCardHolder
    {
        private readonly Queue<DevelopmentCard> developmentCards = new Queue<DevelopmentCard>();

        public bool HasCards => this.developmentCards.Count > 0;

        public void AddDevelopmentCard(DevelopmentCard developmentCard)
        {
            this.developmentCards.Enqueue(developmentCard);
        }

        public DevelopmentCard[] GetDevelopmentCards()
        {
            throw new NotImplementedException("Not needed");
        }

        public bool TryGetNextCard(out DevelopmentCard card)
        {
            card = null;
            if (!this.HasCards)
                return false;

            card = this.developmentCards.Dequeue();
            return true;
        }
    }
}
