using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioNumberGenerator : INumberGenerator
    {
        private readonly Queue<Tuple<uint, uint>> diceRolls = new Queue<Tuple<uint, uint>>();
        private readonly Queue<PlayerResource> scenarios = new Queue<PlayerResource>();

        public void AddRandomNumber(int number)
        {
        }

        public void AddTwoDiceRoll(uint dice1, uint dice2)
        {
            this.diceRolls.Enqueue(new Tuple<uint, uint>(dice1, dice2));
        }

        public int GetRandomNumberBetweenZeroAndMaximum(int exclusiveMaximum)
        {
            var playerResource = this.scenarios.Dequeue();
            var playerAgent = playerResource.playerAgent;

            switch (playerResource.resourceType)
            {
                case ResourceTypes.Brick:
                {
                    if (playerAgent.Resources.BrickCount == 0)
                        throw new Exception("Brick count is 0");
                    return 0;
                }
                case ResourceTypes.Grain:
                {
                    if (playerAgent.Resources.GrainCount == 0)
                        throw new Exception("Grain count is 0");
                    return playerAgent.Resources.BrickCount;
                }
                case ResourceTypes.Lumber:
                {
                    if (playerAgent.Resources.LumberCount == 0)
                        throw new Exception("Lumber count is 0");
                    return playerAgent.Resources.BrickCount + playerAgent.Resources.GrainCount;
                }
                case ResourceTypes.Ore:
                {
                    if (playerAgent.Resources.OreCount == 0)
                        throw new Exception("Ore count is 0");
                    return playerAgent.Resources.BrickCount + playerAgent.Resources.GrainCount +
                        playerAgent.Resources.LumberCount;
                }
                case ResourceTypes.Wool:
                {
                    if (playerAgent.Resources.WoolCount == 0)
                        throw new Exception("Wool count is 0");
                    return playerAgent.Resources.BrickCount + playerAgent.Resources.GrainCount +
                        playerAgent.Resources.LumberCount + playerAgent.Resources.OreCount;
                }
            }

            throw new Exception("Should not get here");
        }

        public void RollTwoDice(out uint dice1, out uint dice2)
        {
            if (this.diceRolls.Count == 0)
                throw new Exception("No dice rolls");

            var tuple = this.diceRolls.Dequeue();
            dice1 = tuple.Item1;
            dice2 = tuple.Item2;
        }

        public void PlayerLosesResource(ScenarioPlayerAgent playerAgent, ResourceTypes resourceType)
        {
            this.scenarios.Enqueue(new PlayerResource { playerAgent = playerAgent, resourceType = resourceType });
        }

        private class PlayerResource
        {
            public ScenarioPlayerAgent playerAgent;
            public ResourceTypes resourceType;
        }
    }
}