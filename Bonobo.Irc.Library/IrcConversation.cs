using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents basic IRC conversation
    /// </summary>
    public abstract class IrcConversation
    {
        private readonly SynchronizedObservableCollection<IrcConversationMessage> _messages = new SynchronizedObservableCollection<IrcConversationMessage>();
        private readonly IrcSession _session;


        /// <summary>
        /// Messages in conversation
        /// </summary>
        public SynchronizedObservableCollection<IrcConversationMessage> Messages 
        { 
            get { return _messages; } 
        }

        public IrcSession Session
        {
            get { return _session; }
        }

        protected IrcConversation(IrcSession session)
        {
            _session = session;
        }
    }
}
