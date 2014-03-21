using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bonobo.Irc.Client.UI.Data;
using System.ComponentModel;

namespace Bonobo.Irc.Client
{
    public class UIConversation : INotifyPropertyChanged
    {
        protected SynchronizationContext _sync = SynchronizationContext.Current;
        protected IrcConversationProvider _conversationProvider;

        protected String _header;
        private PropertyChangedEventHandler _propertyChanged;

        public String Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged("Header");
                }
            }
        }

        public IrcConversationProvider ConversationProvider
        {
            get { return _conversationProvider; }
        }

        private void OnPropertyChanged(String propertyName)
        {
            var handler = _propertyChanged;

            if (handler != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);

                SynchronizationContext.Current.Post(() => handler(this, args));
            }
        }
        
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
    }
}
