using System;
using System.Threading.Tasks;
using SoC.Library.ScenarioTests;

namespace SoC.ScenarioRunnerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                new ScenarioTests().ATest();
            });

            Console.WriteLine("Hit any key to exit");
            Console.ReadKey();
        }
    }
}
