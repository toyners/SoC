namespace SoC.WebApplication
{
    public class ResponseBase
    {
        public string ClassName { get { return this.GetType().Name; } }
    }
}
