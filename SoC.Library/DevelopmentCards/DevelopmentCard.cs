
namespace Jabberwocky.SoC.Library.DevelopmentCards
{
    using System;

    public class DevelopmentCard
    {
        public readonly Guid Id;
        public readonly string Text;
        public readonly string Title;
        public readonly DevelopmentCardTypes Type;

        public DevelopmentCard(DevelopmentCardTypes type, string title, string text)
        {
            this.Id = Guid.NewGuid();
            this.Type = type;
            this.Title = title;
            this.Text = text;
        }
    }
}
