
namespace SoC.Library.ScenarioTests
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioAttribute : Attribute
    {
        public ScenarioAttribute()
        {
        }
    }
}
