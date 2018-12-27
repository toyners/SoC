
using Jabberwocky.SoC.Library.DevelopmentCards;

namespace Jabberwocky.SoC.Library.Interfaces
{
    public interface IDevelopmentCardHolder
    {
        bool HasCards { get; }

        bool TryGetNextCard(out DevelopmentCard card);

        DevelopmentCard[] GetDevelopmentCards();    
    }
}
