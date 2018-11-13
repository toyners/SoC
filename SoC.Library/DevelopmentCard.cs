
namespace Jabberwocky.SoC.Library
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

    public class KnightDevelopmentCard : DevelopmentCard
    {
        public KnightDevelopmentCard() : base(DevelopmentCardTypes.Knight, "Knight", "Move the robber. Steal 1 resource from the owner of a settlement or city adjacent to the robber's new hex.")
        {
        }
    }

    public class RoadBuildingDevelopmentCard : DevelopmentCard
    {
        public RoadBuildingDevelopmentCard() : base(DevelopmentCardTypes.RoadBuilding, "Road Building", "Place 2 new roads as if you had just built them.")
        {
        }
    }

    public class YearOfPlentyDevelopmentCard : DevelopmentCard
    {
        public YearOfPlentyDevelopmentCard() : base(DevelopmentCardTypes.YearOfPlenty, "Year of Plenty", "Take any 2 resources from the bank. Add them to your hand. They can be 2 of the same resource or 2 different resources.")
        {
        }
    }

    public class MonopolyDevelopmentCard : DevelopmentCard
    {
        public MonopolyDevelopmentCard() : base(DevelopmentCardTypes.Monopoly, "Monopoly", "When you play this card, announce 1 type of resource. All other players must give you all of their resources of that type.")
        {
        }
    }

    public class VictoryPointDevelopmentCard : DevelopmentCard
    {
        public VictoryPointDevelopmentCard(string title) : base(DevelopmentCardTypes.VictoryPoint, title, "Reveal this card on your turn if, with it, you reach the number of points required for victory.")
        {
        }
    }
}
