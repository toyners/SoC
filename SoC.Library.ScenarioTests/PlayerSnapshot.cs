using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;

namespace SoC.Library.ScenarioTests
{
    internal class PlayerSnapshot
    {
        #region Fields
        public List<DevelopmentCardTypes> HeldCards;
        public ResourceClutch? Resources;
        public uint? VictoryPoints;
        #endregion

        #region Methods
        public void Verify(IPlayer player)
        {
            if (this.HeldCards != null)
            {
                Assert.AreEqual(this.HeldCards.Count, player.HeldCards.Count, $"Player '{player.Name}' state does not match: Expected {this.HeldCards.Count} held cards, found {player.HeldCards.Count} held cards");
                // TODO: Compare cards
            }

            if (this.Resources.HasValue)
                Assert.AreEqual(this.Resources.Value, player.Resources, $"Player '{player.Name}' state does not match: Expected {this.Resources.Value} resources, found {player.Resources} resources");

            if (this.VictoryPoints.HasValue)
                Assert.AreEqual(this.VictoryPoints.Value, player.VictoryPoints, $"Player '{player.Name}' state does not match: Expected {this.VictoryPoints.Value} vp, found {player.VictoryPoints} vp");
        }
        #endregion
    }
}
