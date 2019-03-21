
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;

    public class PlayerSetupEvent : GameEventWithSingleArgument<IDictionary<string, Guid>>
    {
        public PlayerSetupEvent(IDictionary<string, Guid> playerIdsByName) : base(playerIdsByName)
        {
            if (playerIdsByName == null || playerIdsByName.Count == 0)
                throw new ArgumentNullException("playerIdsByName");
        }

        public IDictionary<string, Guid> PlayerIdsByName { get { return this.Item; } }

        /*public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (PlayerSetupEvent)obj;
            if (this.Item.Count != other.Item.Count)
                return false;
        
            var otherKeys = new List<string>(other.Item.Keys);
            otherKeys.Sort();

            var keys = new List<string>(this.Item.Keys);
            keys.Sort();

            for (int i = 0; i < otherKeys.Count; i++)
            {
                var key = keys[i];
                var otherKey = otherKeys[i];
                if (key != otherKey)
                    return false;
                if (this.Item[key] != other.Item[otherKey])
                    return false;
            }

            return base.Equals(obj);
        }*/
    }
}
