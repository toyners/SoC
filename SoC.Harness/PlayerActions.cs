
namespace SoC.Harness
{
    public class PlayerActions
    {
        public string BuildCityMessages { get; private set; }
        public string BuildRoadMessages { get; private set; }
        public string BuildSettlementMessages { get; private set; }

        public bool CanBuildCity { get { return string.IsNullOrEmpty(this.BuildCityMessages); } }
        public bool CanBuildRoad { get { return string.IsNullOrEmpty(this.BuildRoadMessages); } }
        public bool CanBuildSettlement { get { return string.IsNullOrEmpty(this.BuildSettlementMessages); } }

        public bool CanBuyDevelopmentCard;
        public bool CanUseDevelopmentCard;

        public void AddBuildCityMessages(params string[] messages)
        {
            this.BuildCityMessages = string.Join("\r\n", messages);
        }

        public void AddBuildRoadMessages(params string[] messages)
        {
            this.BuildRoadMessages = string.Join("\r\n", messages);
        }

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
