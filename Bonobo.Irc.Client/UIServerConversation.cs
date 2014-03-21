using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Bonobo.Irc.Client.UI.Data;
using System.Threading;

namespace Bonobo.Irc.Client
{
    public class UIServerConversation : UIConversation
    {
        protected IrcSession _session;

        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        private IrcServerConversation _serverConversation;
        private ObservableCollection<IrcConversationMessage> _messages = new ObservableCollection<IrcConversationMessage>();
        private ObservableCollection<IrcChannel> _channels = new ObservableCollection<IrcChannel>();

        public UIServerConversation(IrcSession session, IrcConversationProvider provider)
        {
            _conversationProvider = provider;
            _session = session;
            _serverConversation = _session.ServerConversation;
            _serverConversation.Messages.CollectionChanged += new NotifyCollectionChangedEventHandler(Messages_CollectionChanged);
            _session.Channels.CollectionChanged += new NotifyCollectionChangedEventHandler(SessionChannels_CollectionChanged);
            _session.Conversations.CollectionChanged += new NotifyCollectionChangedEventHandler(Conversations_CollectionChanged);
        }     

        public IrcSession Session
        {
            get { return _session; }
        }

        public ObservableCollection<IrcConversationMessage> Messages
        {
            get { return _messages; }
        }

        public ObservableCollection<IrcChannel> Channels
        {
            get { return _channels; }
        }

        public bool ContainsChannelConversation(IrcChannel channel)
        {
            return (_session.Conversations.Where(c => c is IrcChannelConversation
                                                        && ((IrcChannelConversation)c).Name == channel.Name
                                                        && ((IrcChannelConversation)c).Session == channel.Session).Count() > 0);
        }

        public bool ContainsPersonConversation(IrcPerson person)
        {
            return (_session.Conversations.Where(c => c is IrcPersonConversation
                                                        && ((IrcPersonConversation)c).Name == person.Name
                                                        && ((IrcPersonConversation)c).Session == person.Session).Count() > 0);
        }

        public void AddChannelConversation(IrcChannel channel)
        {
            if (!ContainsChannelConversation(channel))
            {
                var conversation = new IrcChannelConversation(_session, channel);
                if (!_session.Channels.Contains(channel))
                {
                    _session.Channels.Add(channel);
                }
                _session.Conversations.Add(conversation);               
                channel.Join();
            }
        }

        public void AddPersonConversation(IrcPerson person)
        {
            if (!ContainsPersonConversation(person))
            {
                var conversation = new IrcPersonConversation(_session, person);
                _session.Conversations.Add(conversation);
            }
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    _sync.Send(() => Messages.Add((IrcConversationMessage)item));
                }
            }
        }

        private void SessionChannels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    _sync.Send(() => _channels.Add((IrcChannel)item));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _sync.Send(() => _channels.Clear());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    _sync.Send(() => _channels.Remove((IrcChannel)item));
                }
            }
        }

        void Conversations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is IrcChannelConversation)
                    {
                        var channelConversation = (IrcChannelConversation)item;
                        _syncContext.Send(() => _conversationProvider.Conversations.Add(new UIChannelConversation(channelConversation, _conversationProvider, this)
                        {
                            Header = channelConversation.Channel.Name,
                        }));
                    }
                    else if (item is IrcPersonConversation)
                    {
                        var personConversation = (IrcPersonConversation)item;

                        _syncContext.Send(() => _conversationProvider.Conversations.Add(new UIPersonConversation(personConversation, _conversationProvider)
                        {
                            Header = personConversation.Person.Name,
                        }));
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _sync.Send(() => _conversationProvider.Conversations.Clear());
            }
        }   
    }
}
