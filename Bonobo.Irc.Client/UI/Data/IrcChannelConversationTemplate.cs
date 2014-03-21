using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Bonobo.Irc.Client.UI.Data
{
    public sealed partial class IrcChannelConversationTemplate : ResourceDictionary
    {
        public IrcChannelConversationTemplate()
        {
            InitializeComponent();
        }

        public DataTemplate DataTemplate
        {
            get { return base["IrcChannelConversationTemplate"] as DataTemplate; }
        }

        private void ListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var conversation = ((UIChannelConversation)e.Parameter);
            conversation.Channel.ListUsers();
        }

        private void ChatExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var listPersons = (ListBox)((Grid)sender).FindName("lvPeople");
            var conversation = ((UIChannelConversation)e.Parameter);
            foreach (IrcPerson item in listPersons.SelectedItems)
            {
                if (item.Name != conversation.ServerConversation.Session.ConnectionInfo.Username)
                {
                    conversation.ServerConversation.AddPersonConversation(item);
                }
            }
        }

        private void SendExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var grid = (Grid)sender;
            var txtMessage = (TextBox)grid.FindName("txtMessage");
            if (!String.IsNullOrEmpty(txtMessage.Text))
            {
                var conversation = ((UIChannelConversation)e.Parameter);
                conversation.Channel.SendMessage(txtMessage.Text);
                txtMessage.Text = String.Empty;
            }
        }
    }
}
