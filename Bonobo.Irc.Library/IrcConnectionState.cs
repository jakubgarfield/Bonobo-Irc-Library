using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents IrcConnection state
    /// </summary>
    [Serializable]
    public enum IrcConnectionState
    {
        Created,        
        Opening,
        Opened,
        Closing,
        Closed,
        Faulted
    }
}
