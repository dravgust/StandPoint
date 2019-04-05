using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StandPoint.Threading
{
    public static class Extensions
    {
        public static Task HandleExceptions(this Task task, ILogger logger = null)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception == null) return;
                foreach (var e in t.Exception.Flatten().InnerExceptions)
                {
                    var message = $"{e.GetType().Name} {e.Message}\r\n{e.StackTrace}";
                    if (logger != null)
                    {
                        logger.LogError(message);
                    }
                    else
                    {
                        Trace.TraceError(message);
                    }
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }
    }
}
