using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bonobo.Irc.Client.UI.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Bonobo.Irc.Client
{
    public class UIPersonConversation : UIConversation
    {
        private readonly IrcPerson _person;
        private readonly ObservableCollection<IrcConversationMessage> _messages = new ObservableCollection<IrcConversationMessage>();

        public UIPersonConversation(IrcPersonConversation conversation, IrcConversationProvider provider)
        {
            _person = conversation.Person;
            _conversationProvider = provider;
            conversation.Messages.CollectionChanged += new NotifyCollectionChangedEventHandler(Messages_CollectionChanged);
        }

        public ObservableCollection<IrcConversationMessage> Messages
        {
            get { return _messages; }
        }

        public IrcPerson Person
        {
            get { return _person; }
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

    }
}
