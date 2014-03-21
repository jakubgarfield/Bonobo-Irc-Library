using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents arguments on session state change
    /// </summary>
    [Serializable]
    public sealed class IrcStateChangedEventArgs : EventArgs
    {
        private Exception _reason;
        private String _message;

        public IrcStateChangedEventArgs(String message)
        {
            _message = message;
        }

        public IrcStateChangedEventArgs(Exception reason)
        {
            _reason = reason;
        }

        // FIXME: remove
        public String Message
        {
            get { return _message; }
        }

        public Exception Reason
        {
            get { return _reason; }
        }
    }
}
