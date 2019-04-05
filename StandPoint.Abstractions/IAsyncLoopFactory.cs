using System;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Utilities;

namespace StandPoint.Abstractions
{
    /// <summary>Factory for creating and also possibly starting application defined tasks inside async loop.</summary>
    public interface IAsyncLoopFactory
    {
        /// <summary>
        /// Creates a new async loop.
        /// </summary>
        /// <param name="name">Name of the loop.</param>
        /// <param name="loop">Application defined task that will be called and awaited in the async loop.</param>
        /// <returns>Newly created async loop that can be started on demand.</returns>
        IAsyncLoop Create(string name, Func<CancellationToken, Task> loop);

        /// <summary>
        /// Starts an application defined task inside a newly created async loop.
        /// </summary>
        /// <param name="name">Name of the loop.</param>
        /// <param name="loop">Application defined task that will be called and awaited in the async loop.</param>
        /// <param name="repeatEvery">Interval between each execution of the task. 
        /// If this is <see cref="TimeSpans.RunOnce"/>, the task is only run once and there is no loop. 
        /// If this is null, the task is repeated every 1 second by default.</param>
        /// <param name="startAfter">Delay before the first run of the task, or null if no startup delay is required.</param>
        IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null);

        /// <summary>
        /// Starts an application defined task inside a newly created async loop.
        /// </summary>
        /// <param name="name">Name of the loop.</param>
        /// <param name="loop">Application defined task that will be called and awaited in the async loop.</param>
        /// <param name="cancellation">Cancellation token that triggers when the task and the loop should be cancelled.</param>
        /// <param name="repeatEvery">Interval between each execution of the task. 
        /// If this is <see cref="TimeSpans.RunOnce"/>, the task is only run once and there is no loop. 
        /// If this is null, the task is repeated every 1 second by default.</param>
        /// <param name="startAfter">Delay before the first run of the task, or null if no startup delay is required.</param>
        IAsyncLoop Run(string name, Func<CancellationToken, Task> loop, CancellationToken cancellation, TimeSpan? repeatEvery = null, TimeSpan? startAfter = null);

        /// <summary>
        /// Waits until a condition is met, then executes the action and completes.
        /// <para>
        /// Waiting is implemented using the async loop for which the task is defined as execution of the condition method 
        /// followed by execution of the action if the condition is satisfied - i.e. returns true. If the condition is 
        /// not satisfied, the loop waits as per <see cref="repeatEvery"/> setting and then it can be attempted again.
        /// </para>
        /// </summary>
        /// <param name="name">Name of the loop.</param>
        /// <param name="nodeCancellationToken">Cancellation token that triggers when the task and the loop should be cancelled.</param>
        /// <param name="condition">Condition to be tested.</param>
        /// <param name="action">Method to execute once the condition is met.</param>
        /// <param name="onException">Method to execute if an exception occurs during evaluation of the condition or during execution of the <see cref="action"/>.</param>
        /// <param name="repeatEvery">Interval between each execution of the task. 
        /// If this is <see cref="TimeSpans.RunOnce"/>, the task is only run once and there is no loop. 
        /// If this is null, the task is repeated every 1 second by default.</param>
        /// <returns></returns>
        IAsyncLoop RunUntil(string name, CancellationToken nodeCancellationToken, Func<bool> condition, Action action, Action<Exception> onException, TimeSpan repeatEvery);
    }
}
