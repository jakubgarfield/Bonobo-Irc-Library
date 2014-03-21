using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents server IRC conversation
    /// </summary>
    public class IrcServerConversation : IrcConversation
    {
        public IrcServerConversation(IrcSession session)
            : base(session)
        {
        }
    }
}
