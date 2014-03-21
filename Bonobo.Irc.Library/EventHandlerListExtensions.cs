using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Bonobo.Irc
{
    /// <summary>
    /// Extension class for simplier event invoking
    /// </summary>
    internal static class EventHandlerListExtensions
    {
        public static void InvokeEvent(this EventHandlerList list, Object key, Object sender)
        {
            var handler = (EventHandler)(list[key]);

            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        public static void InvokeEvent<TArgs>(this EventHandlerList list, Object key, Object sender, TArgs args) where TArgs : EventArgs
        {
            var handler = (EventHandler<TArgs>)(list[key]);

            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
