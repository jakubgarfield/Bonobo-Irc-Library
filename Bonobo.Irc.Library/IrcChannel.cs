using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;

namespace Bonobo.Irc
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class INotifyPropertyChangedExtensions
    {
        public static SendOrPostCallback OnPropertyChanged(this INotifyPropertyChanged self, PropertyChangedEventHandler handler, String propertyName)
        {
            return unused => handler(self, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Represents model of IRC channel object. Keeps messages channel information
    /// and has usefull methods
    /// </summary>
    public class IrcChannel : INotifyPropertyChanged
    {
        private readonly IrcSession _session;
        private readonly ObservableCollection<IrcPerson> _people = new ObservableCollection<IrcPerson>();

        private PropertyChangedEventHandler _onPropChanged;
        private IrcMessageFactory _factory;
        private object _lock = new Object();

        public IrcChannel(IrcSession session, String name, String topic)
        {
            _session = session;
            _factory = _session.MessageFactory;
            Name = name.Split(' ')[0];
            if (topic.Length > 0 && topic[0] == ':')
            {
                topic = topic.Substring(1);
            }
            Topic = topic;
        }

        /// <summary>
        /// Clients connected to channel
        /// </summary>
        public ObservableCollection<IrcPerson> People
        {
            get
            {
                return _people;
            }
        }

        /// <summary>
        /// Session to what channel belongs
        /// </summary>
        public IrcSession Session
        {
            get { return _session; }
        }

        public String Name
        {
            get;
            private set;
        }

        public String Topic
        {
            get;
            set;
        }

        /// <summary>
        /// Join to this channel
        /// </summary>
        public void Join()
        {
            _session.Send(_factory.JoinChannel(Name));
        }
        
        /// <summary>
        /// Safely leave channel
        /// </summary>
        public void Part()
        {
            _session.Send(_factory.Part(Name));
            RemoveConversation();
        }

        /// <summary>
        /// Send message to all channel users
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendMessage(String message)
        {
            var ircMessage = _factory.PrivateMessage(Name, message);
            _session.Send(ircMessage);
            _session.AddMessageToChannelConversation(ircMessage, Name, IrcMessageType.MyMessage);
        }

        /// <summary>
        /// Send command to get clients in channel
        /// </summary>
        public void ListUsers()
        {
            _session.Send(_factory.Names(Name));
        }

        /// <summary>
        /// Add user to People collection. Use if new client
        /// connects to channel
        /// </summary>
        /// <param name="person">New person</param>
        public void AddUser(IrcPerson person)
        {
            lock (_lock)
            {
                if (!_people.Contains(person))
                {
                    _people.Add(person);
                }
            }
        }

        /// <summary>
        /// Remove user to People collection. Use if client
        /// parts.
        /// </summary>
        /// <param name="person">Person to remove</param>
        public void RemoveUser(IrcPerson person)
        {
            lock (_lock)
            {
                _people.Remove(person);
            }
        }

        /// <summary>
        /// Retrive users from NAME reply
        /// </summary>
        /// <param name="nameReply">IRC NAME reply</param>
        public void RetrieveUsers(IrcMessage nameReply)
        {
            lock (_lock)
            {
                _people.Clear();
                if (nameReply.Type == IrcDefinition.Response.NameReply)
                {
                    for (int i = 2; i < nameReply.Arguments.Count; i++)
                    {
                        bool isOperator = false;
                        if (nameReply.Arguments[i][0] == '@')
                        {
                            isOperator = true;
                        }
                        _people.Add(new IrcPerson(_session ,nameReply.Arguments[i].Trim(':').Trim('@')) { IsOperator = isOperator});
                    }
                }
            }
        }

        /// <summary>
        /// Validates channel name
        /// </summary>
        /// <param name="name">Name to validate</param>
        /// <returns>Returns true if name is valid channel name</returns>
        public static bool IsChannelName(String name)
        {            
            if (!String.IsNullOrEmpty(name))
            {
                var startChars = new char[] { '&', '#', '!', '+' };
                return (startChars.Contains(name[0]));
            }

            return false;
        }

        private void RemoveConversation()
        {
            IrcConversation conversation = null;
            foreach (var item in _session.Conversations)
            {
                if (item is IrcChannelConversation)
                {
                    if (((IrcChannelConversation)item).Name == Name)
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

        public override bool Equals(object obj)
        {
            IrcChannel channel = obj as IrcChannel;
            if (channel != null)
            {
                return (String.Equals(Name, channel.Name, StringComparison.OrdinalIgnoreCase) && _session == channel.Session);
            }
            else
            {
                return false;
            }
        }

        #region INotifyPropertyChanged Members

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _onPropChanged += value; }
            remove { _onPropChanged -= value; }
        }

        #endregion
    }
}
