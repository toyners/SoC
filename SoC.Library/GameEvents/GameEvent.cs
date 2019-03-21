
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Diagnostics;
    using Newtonsoft.Json;

    [DebuggerDisplay("{GetType().Name}")]
    public abstract class GameEvent
    {
        #region Fields
        [JsonProperty] // [JsonIgnore] to ignore property
        public readonly Guid PlayerId;
        #endregion

        #region Construction
        public GameEvent(Guid playerId)
        {
            this.PlayerId = playerId;
        }
        #endregion

        public bool IsInformation { get; set; }

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            return this.PlayerId == ((GameEvent)obj).PlayerId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().ToString();
        }

        public virtual string ToJSONString() { return JsonConvert.SerializeObject(this); }
        #endregion
    }
}
