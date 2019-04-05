using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StandPoint.Reflection;

namespace StandPoint.Net
{
    public abstract class SocketController : ISocketController
    {
        private delegate Task<byte[]> AsyncFunc(byte[] data);
        private delegate byte[] SyncFunc(byte[] data);

        protected ISocketContext Context { get; private set; }

        public async Task InvokeAsync(ISocketContext context, CancellationToken token)
        {
            var methodInfo = this.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SingleOrDefault(member => member.HasAttribute(out OnRequestAttribute attribute));
            if (methodInfo == null) return;

            Context = context;

            byte[] message;
            byte[] response = null;
            while ((message = await Context.Request.ReadLineAsync()) != null)
            {
                methodInfo.HasAttributes(out List<SocketFilterAttribute> filters);
                foreach (var filter in filters)
                {
                    filter.BeforeRequestExecuted(Context, message);
                }

                Delegate @delegate;
                if ((@delegate = Delegate.CreateDelegate(typeof(AsyncFunc), this, methodInfo, false)) != null)
                {
                    response = await ((AsyncFunc)@delegate)(message);
                }
                else if ((@delegate = Delegate.CreateDelegate(typeof(SyncFunc), this, methodInfo, false)) != null)
                {
                    response = ((SyncFunc)@delegate)(message);
                }

                foreach (var filter in filters)
                {
                    filter.AfterRequestExecuted(Context);
                }

                if (response != null)
                {
                    await Context.Response.WriteLineAsync(response);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class SocketFilterAttribute : Attribute
    {
        public virtual void BeforeRequestExecuted(ISocketContext context, byte[] message)
        {

        }

        public virtual void AfterRequestExecuted(ISocketContext context)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OnRequestAttribute : Attribute
    {

    }
}
