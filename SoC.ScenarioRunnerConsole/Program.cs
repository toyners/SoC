using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
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
            var assembly = Assembly.GetAssembly(typeof(ScenarioTests));
            var methods = assembly.GetTypes()
                                .SelectMany(t => t.GetMethods())
                                .Where(m => m.GetCustomAttributes(typeof(ScenarioAttribute), false).Length > 0)
                                .ToArray();

            while (true)
            {
                var isFinished = false;
                var methodNumber = 0;
                Console.WriteLine($"[{methodNumber++}] - ALL");
                foreach (var method in methods)
                    Console.WriteLine($"[{methodNumber++}] - {method.Name}");

                Console.WriteLine("Select number of scenario to run. 0 to run all. X to exit");
                var key = Console.ReadLine();
                if (key == "x" || key == "X")
                    break;
                methodNumber = int.Parse(key);
                Console.WriteLine();

                var task = Task.Factory.StartNew(() =>
                {
                    var runningMethods = new List<MethodInfo>();
                    if (methodNumber != 0)
                        runningMethods.Add(methods[methodNumber - 1]);
                    else
                        runningMethods.AddRange(methods);

                    foreach (MethodInfo method in runningMethods)
                    {
                        try
                        {
                            Console.Write($"Running '{method.Name}' ...");
                            var instance = Activator.CreateInstance(method.DeclaringType);
                            method.Invoke(instance, null);
                            Console.WriteLine("Completed");
                        }
                        catch (Exception e)
                        {
                            var exception = e;
                            while (exception.InnerException != null)
                                exception = exception.InnerException;
                            Console.WriteLine($"FAILED - {exception.Message}");
                        }
                    }

                    isFinished = true;
                });

                while (!isFinished)
                {
                    Thread.Sleep(50);
                }

                Console.WriteLine();
            }
        }
    }
}
