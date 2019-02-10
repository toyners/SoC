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
            var methods = assembly.GetTypes()
                                .SelectMany(t => t.GetMethods())
                                .Where(
                                    m => m.GetCustomAttributes(typeof(TestAttribute), false).Length > 0 &&
                                    m.Name == "ATest")
                                .ToArray();

            var task = Task.Factory.StartNew(() =>
            {
                foreach (MethodInfo method in methods)
                {
                    var instance = Activator.CreateInstance(method.DeclaringType);
                    method.Invoke(instance, null);
                }
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
