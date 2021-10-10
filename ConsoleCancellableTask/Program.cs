using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleCancellableTask
{
    class Program
    {
        static void Main(string[] args)
        {
            DoTest();

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }

        static async void DoTest()
        {
            var tasks = new List<Task<int>>();
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            // Construct started tasks
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks.Add(DoCancellableTask(index, token));
            }

            try
            {
                Task<int> completedTask = await Task.WhenAny(tasks);
                Console.WriteLine("Hitted task {0}: {1}", completedTask.Id, completedTask.Status);

                source.Cancel();
                Thread.Sleep(5000);

                Console.WriteLine("\r\nChecking all tasks status ...");
                foreach (var t in tasks)
                {
                    Console.WriteLine("Task {0}: {1}", t.Id, t.Status);
                }
                // We should never get to this point
                Console.WriteLine("WaitAll() has not thrown exceptions. THIS WAS NOT EXPECTED.");
            }
            catch (AggregateException e)
            {
                foreach (Exception ea in e.InnerExceptions)
                {
                    if (ea is TaskCanceledException exception)
                        Console.WriteLine("Unable to compute mean: {0}",
                                          exception.Message);
                    else
                        Console.WriteLine("Exception: " + e.GetType().Name);
                }
                Console.WriteLine("\nThe following exceptions have been thrown by WaitAll(): (THIS WAS EXPECTED)");
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                }
            }
            finally
            {
                source.Dispose();
            }
        }

        // Define a delegate that prints and returns the system tick count
        private static async Task<int> DoCancellableTask(object obj, CancellationToken cancellationToken)
            => await Task.Run(() =>
                {
                    int i = (int)obj;

                    // Make each thread sleep a different time in order to return a different tick count
                    Thread.Sleep(i * 100);

                    // The tasks that receive an argument between 2 and 5 throw exceptions
                    if (2 <= i && i <= 5)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();
                    }

                    int tickCount = Environment.TickCount;
                    Console.WriteLine("Task={0}, i={1}, TickCount={2}, Thread={3}", Task.CurrentId, i, tickCount, Thread.CurrentThread.ManagedThreadId);

                    return tickCount;
                });
    }
}
