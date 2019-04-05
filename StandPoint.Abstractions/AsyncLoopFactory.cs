using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StandPoint.Utilities;

namespace StandPoint.Abstractions
{
    public class AsyncLoopFactory : IAsyncLoopFactory
    {
        /// <summary>Instance logger.</summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the object.
        /// </summary>
        /// <param name="loggerFactory">Factory to create logger for the object and for the async loops it creates.</param>
        /// <remarks>TODO: It might be a better idea to pass factory to the newly created loops so that new loggers can be created for each loop.</remarks>
        public AsyncLoopFactory(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger(typeof(Application).FullName);
        }

        /// <inheritdoc />
        public IAsyncLoop Create(string name, Func<CancellationToken, Task> loop)
        {
            return new AsyncLoop(name, this._logger, loop);
        }

        /// <inheritdoc />
        public IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null)
        {
            return new AsyncLoop(name, this._logger, loop).Run(repeatEvery, startAfter);
        }

        /// <inheritdoc />
        public IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, CancellationToken cancellation, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null)
        {
            Guard.NotNull(cancellation, nameof(cancellation));
            Guard.NotEmpty(name, nameof(name));
            Guard.NotNull(loop, nameof(loop));

            return new AsyncLoop(name, this._logger, loop).Run(cancellation, repeatEvery ?? TimeSpan.FromMilliseconds(1000), startAfter);
        }

        /// <inheritdoc />
        public IAsyncLoop RunUntil(string name, CancellationToken nodeCancellationToken, Func<bool> condition, Action action,
            Action<Exception> onException, TimeSpan repeatEvery)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(nodeCancellationToken);
            return this.Run(name, token =>
            {
                try
                {
                    // loop until the condition is met, then execute the action and finish.
                    if (condition())
                    {
                        action();

                        linkedTokenSource.Cancel();
                    }
                }
                catch (Exception e)
                {
                    onException(e);
                    linkedTokenSource.Cancel();
                }
                return Task.CompletedTask;
            },
            linkedTokenSource.Token,
            repeatEvery: repeatEvery);
        }
    }
}
