using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents boject representation of user client
    /// </summary>
    public class IrcPerson
    {
        private readonly IrcSession _session;

        public IrcPerson(IrcSession session, String name)
        {
            _session = session;
            Name = name;
        }

        public String Name
        {
            get;
            private set;
        }

        public IrcSession Session
        {
            get { return _session; }
        }

        public bool IsOperator
        {
            get;
            set;
        }

        public void LeaveConversation()
        {
            RemoveConversation();
        }

        /// <summary>
        /// Sends private message to client
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendMessage(String message)
        {
            var ircMessage = _session.MessageFactory.PrivateMessage(Name, message);
            _session.Send(ircMessage);
            _session.AddMessageToPersonConversation(ircMessage, Name, IrcMessageType.MyMessage);
        }

        public override bool Equals(object obj)
        {
            IrcPerson person = obj as IrcPerson;
            if (person != null)
            {
                return (String.Equals(Name, person.Name, StringComparison.OrdinalIgnoreCase) && _session == person._session);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieve username from fullname
        /// </summary>
        /// <param name="name">Full name</param>
        /// <returns>Returns username from fullname</returns>
        public static String GetUsernameFromFullname(String name)
        {
            if (name.IndexOf('!') > 0)
            {
                name = name.Substring(0, name.IndexOf('!'));
            }
            return name;
        }

        private void RemoveConversation()
        {
            IrcConversation conversation = null;
            foreach (var item in _session.Conversations)
            {
                if (item is IrcPersonConversation)
                {
                    if (((IrcPersonConversation)item).Name == Name)
                    {
                        conversation = item;
                        break;
                    }
                }
            }

            if (conversation != null)
            {
                _session.Conversations.Remove(conversation);
            }
        }
    }
}
