using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bonobo.Irc.Client.UI.Data
{
    public sealed partial class IrcPersonConversationTemplate : ResourceDictionary
    {
        public IrcPersonConversationTemplate()
        {
            InitializeComponent();
        }

        public DataTemplate DataTemplate
        {
            get { return base["IrcPersonConversationContentTemplate"] as DataTemplate; }
        }

        private void SendExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var grid = (Grid)sender;
            var txtMessage = (TextBox)grid.FindName("txtMessage");
            if (!String.IsNullOrEmpty(txtMessage.Text))
            {
                var conversation = ((UIPersonConversation)e.Parameter);
                conversation.Person.SendMessage(txtMessage.Text);
                txtMessage.Text = String.Empty;
            }
        }
    }
}
