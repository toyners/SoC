using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace SoC.Library.ScenarioTests
{
    internal class PlayerSnapshot
    {
        #region Fields
        public List<DevelopmentCardTypes> HeldCards;
        public ResourceClutch? Resources;
        public uint? VictoryPoints;

        public readonly string Name;
        #endregion

        public PlayerSnapshot(string name) { this.Name = name; }

        #region Methods
        public void Verify(IPlayer player)
        {
            if (this.HeldCards != null)
            {
                Assert.AreEqual(this.HeldCards.Count, player.HeldCards.Count, $"Player '{player.Name}' state does not match: Expected {this.HeldCards.Count} held cards, found {player.HeldCards.Count} held cards");
                // TODO: Compare cards
            }

            if (this.Resources.HasValue)
                Assert.AreEqual(this.Resources.Value, player.Resources, $"Player '{player.Name}' state does not match: Expected {(this.Resources.Value != ResourceClutch.Zero ? this.Resources.Value.ToString() : "zero")} resources, found {(player.Resources != ResourceClutch.Zero ? player.Resources.ToString() : "zero")} resources");

            if (this.VictoryPoints.HasValue)
                Assert.AreEqual(this.VictoryPoints.Value, player.VictoryPoints, $"Player '{player.Name}' state does not match: Expected {this.VictoryPoints.Value} vp, found {player.VictoryPoints} vp");
        }
        #endregion
    }

    public class PlayerState
    {
        private BasePlayerTurn playerTurn;
        public readonly IPlayer Player;
        private List<DevelopmentCardTypes> heldCards;
        private ResourceClutch? resources;
        private uint? victoryPoints;

        internal PlayerState(BasePlayerTurn playerTurn, IPlayer player)
        {
            this.playerTurn = playerTurn;
            this.Player = player;
        }

        internal BasePlayerTurn End()
        {
            return this.playerTurn;
        }

        internal PlayerState HeldCards(params DevelopmentCardTypes[] cards)
        {
            this.heldCards = new List<DevelopmentCardTypes>(cards);
            return this;
        }

        internal PlayerState Resources(ResourceClutch resources)
        {
            this.resources = resources;
            return this;
        }

        public void Verify()
        {
            if (this.heldCards != null)
            {
                Assert.AreEqual(this.heldCards.Count, this.Player.HeldCards.Count, $"Player '{this.Player.Name}' state does not match: Expected {this.heldCards.Count} held cards, found {this.Player.HeldCards.Count} held cards");
                this.heldCards.Sort();
                var playerHeldCards = this.Player.HeldCards.Select(c => c.Type).ToList();
                playerHeldCards.Sort();

                for (var index = 0; index < this.heldCards.Count; index++)
                    Assert.AreEqual(this.heldCards[index], playerHeldCards[index], $"Player '{this.Player.Name}' state does not match: Expected {this.heldCards[index]} card, found {playerHeldCards[index]}. At index {index} in list");
            }

            if (this.resources.HasValue)
                Assert.AreEqual(this.resources.Value, this.Player.Resources, $"Player '{this.Player.Name}' state does not match: Expected {(this.resources.Value != ResourceClutch.Zero ? this.resources.Value.ToString() : "zero")} resources, found {(this.Player.Resources != ResourceClutch.Zero ? this.Player.Resources.ToString() : "zero")} resources");

            if (this.victoryPoints.HasValue)
                Assert.AreEqual(this.victoryPoints.Value, this.Player.VictoryPoints, $"Player '{this.Player.Name}' state does not match: Expected {this.victoryPoints.Value} vp, found {this.Player.VictoryPoints} vp");
        }

        internal PlayerState VictoryPoints(uint victoryPoints)
        {
            this.victoryPoints = victoryPoints;
            return this;
        }

    }
}
