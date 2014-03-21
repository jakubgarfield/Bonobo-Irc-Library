using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Bonobo.Irc.Client
{
    internal static class SynchronizationContextExtensions
    {
        public static void Post(this SynchronizationContext context, Action action)
        {
            context.Post(s => action(), null);
        }

        public static void Send(this SynchronizationContext context, Action action)
        {
            context.Send(s => action(), null);
        }

        public static SynchronizationContextScope Push(this SynchronizationContext context)
        {
            return new SynchronizationContextScope(context);
        }

        public struct SynchronizationContextScope : IDisposable
        {
            private readonly SynchronizationContext _prev;

            public SynchronizationContextScope(SynchronizationContext newContext)
            {
                _prev = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(newContext);
            }

            public void Dispose()
            {
                SynchronizationContext.SetSynchronizationContext(_prev);
            }
        }
    }
}
