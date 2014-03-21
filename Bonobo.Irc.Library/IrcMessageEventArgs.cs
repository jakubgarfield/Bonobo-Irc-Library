using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents arguments on messsage received by IrcConnection
    /// </summary>
    [Serializable]
    public sealed class IrcMessageEventArgs : EventArgs
    {
        private readonly IrcMessage _message;

        public IrcMessageEventArgs(IrcMessage message)
        {
            _message = message;
        }

        public IrcMessage Message
        {
            get { return _message; }
        }
    }
}
