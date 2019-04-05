using System;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Abstractions.Builder;

namespace StandPoint.Abstractions
{
    /// <summary>
    /// Extension methods for IApplication interface.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Installs handlers for graceful shutdown in the console, starts a application and waits until it terminates. 
        /// </summary>
        /// <param name="app">Applicartion to run.</param>
        public static void Run(this IApplication app)
        {
            var done = new ManualResetEventSlim(false);
            using (var cts = new CancellationTokenSource())
            {
                Action shutdown = () =>
                {
                    if (!cts.IsCancellationRequested)
                    {
                        Console.WriteLine("Application is shutting down...");
                        try
                        {
                            cts.Cancel();
                        }
                        catch (ObjectDisposedException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    done.Wait();
                };

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    shutdown();
                    // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                    eventArgs.Cancel = true;
                };

                app.Run(cts.Token, "Application started. Press Ctrl+C to shut down.", "Application stopped.");
                done.Set();
            }
        }

        /// <summary>
        /// Starts a application, sets up cancellation tokens for its shutdown, and waits until it terminates. 
        /// </summary>
        /// <param name="app">Application to run.</param>
        /// <param name="cancellationToken">Cancellation token that triggers when the application should be shut down.</param>
        /// <param name="shutdownMessage">Message to display on the console to instruct the user on how to invoke the shutdown.</param>
        /// <param name="shutdownCompleteMessage">Message to display on the console when the shutdown is complete.</param>
        public static void Run(this IApplication app, CancellationToken cancellationToken, string shutdownMessage,
            string shutdownCompleteMessage)
        {
            using (app)
            {
                app.Start();

                if (!string.IsNullOrEmpty(shutdownMessage))
                {
                    Console.WriteLine();
                    Console.WriteLine(shutdownMessage);
                    Console.WriteLine();
                }

                cancellationToken.Register(state =>
                {
                    ((IApplicationLifeTime)state).StopApplication();
                },
                app.ApplicationLifeTime);

                var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                app.ApplicationLifeTime.ApplicationStopping.Register(obj =>
                {
                    var tcs = (TaskCompletionSource<object>)obj;
                    tcs.TrySetResult(null);
                }, waitForStop);

                //await waitForStop.Task;
                waitForStop.Task.GetAwaiter().GetResult();

                app.Stop();

                if (!string.IsNullOrEmpty(shutdownCompleteMessage))
                {
                    Console.WriteLine();
                    Console.WriteLine(shutdownCompleteMessage);
                    Console.WriteLine();
                }
            }
        }
    }
}
