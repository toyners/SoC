using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioPlayer : Player
    {
        internal ScenarioPlayer(string name) : base(name)
        {
        }
    }

    internal class PlayerSnapshot
    {
        public List<DevelopmentCardTypes> HeldCards;
        public uint? VictoryPoints;

        public void Verify(IPlayer player)
        {
            if (this.HeldCards != null)
            {
                Assert.AreEqual(this.HeldCards.Count, player.HeldCards.Count, $"Player '{player.Name}' state does not match: Expected {this.HeldCards.Count} held cards, found {player.HeldCards.Count} held cards");
            }

            if (this.VictoryPoints.HasValue)
                Assert.AreEqual(this.VictoryPoints.Value, player.VictoryPoints, $"Player '{player.Name}' state does not match: Expected {this.VictoryPoints.Value} vp, found {player.VictoryPoints} vp");
        }
    }
}
