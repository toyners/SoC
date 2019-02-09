using System;
using System.Threading;
using System.Threading.Tasks;
using SoC.Library.ScenarioTests;

namespace SoC.ScenarioRunnerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Exception exception = null;
            var isFinished = false;
            var task = Task.Factory.StartNew(() =>
            {
                new ScenarioTests().ATest();
            });

            task.ContinueWith(t => {
                isFinished = true;
                if (t.IsFaulted)
                {
                    exception = t.Exception.Flatten().InnerException;
                }
            });

            Console.Write("Running... ");
            while (!isFinished)
            {
                Thread.Sleep(50);
            }

            if (exception != null)
            {
                Console.WriteLine("FAILED");
                Console.WriteLine(exception.Message);
            }
            else
            {
                Console.WriteLine("Done");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
