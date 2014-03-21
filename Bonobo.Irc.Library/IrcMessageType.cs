using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Type of message in IRC conversations
    /// </summary>
    [Serializable]
    public enum IrcMessageType
    {
        UnknownMessage,
        ServerResponse, 
        ServerRequest,
        ChannelResponse,
        ChannelRequest,
        Message,
        MyMessage,
    }
}
