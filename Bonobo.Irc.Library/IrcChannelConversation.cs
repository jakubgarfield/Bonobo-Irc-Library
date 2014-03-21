using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents channel IRC conversation
    /// </summary>
    public class IrcChannelConversation : IrcConversation
    {
        private IrcChannel _channel;

        public IrcChannelConversation(IrcSession session, IrcChannel channel)
            : base(session)
        {
            _channel = channel;
        }

        public String Name
        {
            get { return _channel.Name; }
        }

        public IrcChannel Channel
        {
            get { return _channel; }
        }
    }
}
