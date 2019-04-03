
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

        #region Properties
        public string TypeName { get { return this.GetType().Name; } } // Used in JSON comparison 
        [JsonIgnore]
        public string SimpleTypeName
        {
            get
            {
                var typeName = this.TypeName;
                var index = typeName.LastIndexOf('.');
                return index > -1 ? typeName.Substring(index + 1) : typeName;
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return this.GetType().ToString();
        }

        public virtual string ToJSONString() => JsonConvert.SerializeObject(this);
        #endregion
    }
}
