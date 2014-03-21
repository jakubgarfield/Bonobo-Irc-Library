using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using Bonobo.Irc.Client.UI.Data;
using System.Threading;

namespace Bonobo.Irc.Client.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public MainWindow()
        {
            InitializeComponent();
            AddDefaultServerConversation();
        }

        private void AddDefaultServerConversation()
        {
            _syncContext.Send(() => ConversationProvider.Conversations.Add(new UIServerConversation(new IrcSession(), ConversationProvider) { Header = "New Server" }));

        }

        private IrcConversationProvider ConversationProvider
        {
            get { return (IrcConversationProvider)Resources["ConversationProvider"]; }
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CloseServerProviders();
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            CloseServerProviders();
            base.OnClosed(e);
        }

        private void CloseServerProviders()
        {
            UIConversation[] temp = new UIConversation[ConversationProvider.Conversations.Count];
            ConversationProvider.Conversations.CopyTo(temp, 0);

            foreach (var item in temp)
            {
                if (item is UIServerConversation)
                {
                    RemoveServerConversation(((UIServerConversation)item));
                }
            }
        }

        private void DisconnectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (tabConversations.SelectedItem != null)
            {
                if (tabConversations.SelectedItem is UIServerConversation)
                {
                    var session = ((UIServerConversation)tabConversations.SelectedItem).Session;
                    if (session.State != IrcSessionState.Closed && session.State != IrcSessionState.Closing)
                    {
                        session.Close();
                    }
                }
            }
        }

        private void CloseAllConversationsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CloseServerProviders();
            _syncContext.Send(() => ConversationProvider.Conversations.Clear());
            AddDefaultServerConversation();
            tabConversations.SelectedItem = tabConversations.Items[0];
        }

        private void CloseConversationExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (tabConversations.SelectedItem != null)
            {
                if (tabConversations.SelectedItem is UIServerConversation)
                {
                    RemoveServerConversation((UIServerConversation)tabConversations.SelectedItem);
                }
                else if (tabConversations.SelectedItem is UIChannelConversation)
                {
                    RemoveChannelConversation((UIChannelConversation)tabConversations.SelectedItem);
                }
                else if (tabConversations.SelectedItem is UIPersonConversation)
                {
                    RemovePersonConversation((UIPersonConversation)tabConversations.SelectedItem);
                }
            }
        }

        private void RemoveServerConversation(UIServerConversation conversation)
        {
            if (conversation.Session.State != IrcSessionState.Closed && conversation.Session.State != IrcSessionState.Closing)
            {
                UIConversation[] temp = new UIConversation[ConversationProvider.Conversations.Count];
                ConversationProvider.Conversations.CopyTo(temp, 0);
                foreach (UIConversation c in temp)
                {
                    if (c is UIChannelConversation && conversation.ContainsChannelConversation(((UIChannelConversation)c).Channel))
                    {
                        RemoveChannelConversation((UIChannelConversation)c);
                    }
                    else if (c is UIPersonConversation && conversation.ContainsPersonConversation(((UIPersonConversation)c).Person))
                    {
                        RemovePersonConversation((UIPersonConversation)c);
                    }
                }
                conversation.Session.Close();
            }
            _syncContext.Send(() => ConversationProvider.Conversations.Remove(conversation));
        }

        private void RemovePersonConversation(UIPersonConversation conversation)
        {
            conversation.Person.LeaveConversation();
            _syncContext.Send(() => ConversationProvider.Conversations.Remove(conversation));
        }

        private void RemoveChannelConversation(UIChannelConversation conversation)
        {
            conversation.Channel.Part();
            _syncContext.Send(() => ConversationProvider.Conversations.Remove(conversation));
        }

        private void ConnectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _syncContext.Send(() => ConversationProvider.Conversations.Add(new UIServerConversation(new IrcSession(), ConversationProvider) { Header = "New Server" }));
            tabConversations.SelectedItem = tabConversations.Items[tabConversations.Items.Count - 1];
        }

    }

}
