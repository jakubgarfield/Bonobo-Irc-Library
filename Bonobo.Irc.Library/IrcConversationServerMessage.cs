using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Message received from server that belongs to server conversation
    /// </summary>
    public class IrcConversationServerMessage : IrcConversationMessage
    {
        public bool IsLoginError
        {
            get;
            set;
        }

        public IrcConversationServerMessage()
        {

        }
    }
}
