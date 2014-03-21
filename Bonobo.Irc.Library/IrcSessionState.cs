using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents session state
    /// </summary>
    [Serializable]
    public enum IrcSessionState
    {
        Closed,
        Opening,
        Opened,
        Notification,
        Error,
        ServerError,
        Closing
    }
}
