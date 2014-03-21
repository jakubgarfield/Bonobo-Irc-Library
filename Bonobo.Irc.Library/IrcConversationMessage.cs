using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents message in IrcConversation
    /// </summary>
    [Serializable]
    public class IrcConversationMessage
    {
        public IrcConversationMessage()
        {
        }

        public DateTime TimeStamp 
        { 
            get; 
            set; 
        }

        public String Message
        { 
            get; 
            set;
        }

        public IrcMessageType Type
        {
            get;
            set;
        }

        public String Username
        {
            get;
            set;
        }

        public String MessageType
        {
            get;
            set;
        }
    }
}
