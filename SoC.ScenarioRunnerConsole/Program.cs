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
            //Exception exception = null;
            var isFinished = false;
            var assembly = Assembly.GetAssembly(typeof(ScenarioTests));
            var methods = assembly.GetTypes()
                                .SelectMany(t => t.GetMethods())
                                .Where(m => m.GetCustomAttributes(typeof(ScenarioAttribute), false).Length > 0)
                                .ToArray();

            var task = Task.Factory.StartNew(() =>
            {
                foreach (MethodInfo method in methods)
                {
                    try
                    {
                        Console.Write($"Running '{method.Name}' ...");
                        var instance = Activator.CreateInstance(method.DeclaringType);
                        method.Invoke(instance, null);
                        Console.WriteLine($"Done");
                    }
                    catch (Exception e)
                    {
                        var exception = e;
                        while (exception.InnerException != null)
                            exception = exception.InnerException;
                        Console.WriteLine("FAILED");
                        Console.WriteLine($" {exception.Message}");
                    }
                }

                isFinished = true;
            });

            /*task.ContinueWith(t => {
                isFinished = true;
                if (t.IsFaulted)
                {
                    exception = t.Exception.Flatten();
                    while (exception.InnerException != null)
                        exception = exception.InnerException;
                }
            });*/

            //Console.Write("Running... ");
            while (!isFinished)
            {
                Thread.Sleep(50);
            }

            /*if (exception != null)
            {
                Console.WriteLine("FAILED");
                Console.WriteLine($" {exception.Message}");
            }
            else
            {
                Console.WriteLine("Done");
            }*/

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
