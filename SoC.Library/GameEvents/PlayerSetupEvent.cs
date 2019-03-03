
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;

    public class PlayerSetupEvent : GameEventArg<IDictionary<string, Guid>>
    {
        public readonly IDictionary<string, Guid> PlayerIdsByName;

        public PlayerSetupEvent(IDictionary<string, Guid> playerIdsByName) : base(playerIdsByName)
        {
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (PlayerSetupEvent)obj;
            if (this.PlayerIdsByName.Count != other.PlayerIdsByName.Count)
                return false;

            var otherKeys = new List<string>(other.PlayerIdsByName.Keys);
            otherKeys.Sort();

            var keys = new List<string>(this.PlayerIdsByName.Keys);
            keys.Sort();

            for (int i = 0; i < otherKeys.Count; i++)
            {
                var key = keys[i];
                var otherKey = otherKeys[i];
                if (key != otherKey)
                    return false;
                if (this.PlayerIdsByName[key] != other.PlayerIdsByName[otherKey])
                    return false;
            }

            return base.Equals(obj);
        }
    }
}
