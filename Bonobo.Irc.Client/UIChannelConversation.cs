using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Bonobo.Irc.Client.UI.Data;

namespace Bonobo.Irc.Client
{
    public class UIChannelConversation : UIConversation
    {
        private IrcChannel _channel;
        private readonly UIServerConversation _serverConversation;

        private ObservableCollection<IrcConversationMessage> _messages = new ObservableCollection<IrcConversationMessage>();
        private ObservableCollection<IrcPerson> _people = new ObservableCollection<IrcPerson>();

        public UIChannelConversation(IrcChannelConversation conversation, IrcConversationProvider provider, UIServerConversation serverConversation)
        {
            _channel = conversation.Channel;
            _conversationProvider = provider;
            _serverConversation = serverConversation;
            conversation.Messages.CollectionChanged += new NotifyCollectionChangedEventHandler(Messages_CollectionChanged);
            _channel.People.CollectionChanged += new NotifyCollectionChangedEventHandler(People_CollectionChanged);
        }

        public IrcChannel Channel
        {
            get { return _channel; }
        }

        public ObservableCollection<IrcConversationMessage> Messages
        {
            get { return _messages; }
        }

        public ObservableCollection<IrcPerson> People
        {
            get { return _people; }
        }

        public UIServerConversation ServerConversation
        {
            get { return _serverConversation; }
        }

        void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    _sync.Send(() => Messages.Add((IrcConversationMessage)item));
                }
            }
        }

        void People_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    _sync.Send(() => People.Add((IrcPerson)item));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    _sync.Send(() => People.Remove((IrcPerson)item));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _sync.Send(() => People.Clear());
            }
        }


    }
}
