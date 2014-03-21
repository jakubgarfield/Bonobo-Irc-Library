using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Bonobo.Irc.Client.UI.Data
{
    public class IrcConversationProvider
    {
        private ObservableCollection<UIConversation> _conversations = new ObservableCollection<UIConversation>();

        public ObservableCollection<UIConversation> Conversations
        {
            get { return _conversations; }
        }

        public IrcConversationProvider()
        {

        }
    }
}
