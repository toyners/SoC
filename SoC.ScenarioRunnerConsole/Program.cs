using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SoC.Library.ScenarioTests;

namespace SoC.ScenarioRunnerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Exception exception = null;
            var isFinished = false;
            var assembly = Assembly.GetAssembly(typeof(ScenarioTests));
            var task = Task.Factory.StartNew(() =>
            {
                var methods = assembly.GetTypes()
                                .SelectMany(t => t.GetMethods())
                                .Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Length > 0)
                                .ToArray();
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
