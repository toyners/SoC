
namespace SoC.Harness
{
    using System.Collections.Generic;

    public class PlayerActions
    {
        private List<string> buildMessages = new List<string>(6);
        public bool CanBuildSettlement { get { return string.IsNullOrEmpty(this.BuildSettlementMessages); } }
        public string BuildSettlementMessages { get; private set; }
        public bool CanBuildRoad;
        public bool CanBuildCity;
        public bool CanBuyDevelopmentCard;
        public bool CanUseDevelopmentCard;

        public void AddBuildSettlementMessages(params string[] messages)
        {
            this.BuildSettlementMessages = string.Join("\r\n", messages);
        }

        public void Clear()
        {
            this.BuildSettlementMessages = null;
        }
    }
}
