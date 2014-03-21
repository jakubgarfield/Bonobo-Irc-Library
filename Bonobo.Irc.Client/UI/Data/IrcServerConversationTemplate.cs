using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;

namespace Bonobo.Irc.Client.UI.Data
{
    public sealed partial class IrcServerConversationTemplate : ResourceDictionary
    {
        private ListBox _lvItems;

        private UIServerConversation ServerConversation
        {
            get;
            set;
        }

        public IrcServerConversationTemplate()
        {
            InitializeComponent();
        }

        public DataTemplate DataTemplate
        {
            get { return base["IrcServerConversationContentTemplate"] as DataTemplate; }
        }

        private void LoginCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var session = ((UIServerConversation)((Grid)sender).DataContext).Session;
            e.CanExecute = (session.State == IrcSessionState.Closed || session.State == IrcSessionState.Opening || session.State == IrcSessionState.ServerError);
        }

        private void LoginExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _lvItems = (ListBox)((Grid)sender).FindName("lvItems");

            ServerConversation = ((UIServerConversation)e.Parameter);
            var session = ((UIServerConversation)e.Parameter).Session;
            session.StateChanged += new EventHandler<IrcStateChangedEventArgs>(session_StateChanged);

            if (IsConnectionInfoValid((Grid)sender))
            {
                session.Open();
            }
        }

        void session_StateChanged(object sender, IrcStateChangedEventArgs e)
        {
            if (((IrcSession)sender).State == IrcSessionState.Opened)
            {
                ServerConversation.Header = ((IrcSession)sender).ConnectionInfo.Address.ToString();
            }
        }

        private void ListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var session = ((UIServerConversation)e.Parameter).Session;
            session.ListChannels();
        }

        private bool IsConnectionInfoValid(Grid sender)
        {
            var address = UpdateBinding("txtServerAddress", sender, TextBox.TextProperty);
            var port = UpdateBinding("txtPort", sender, TextBox.TextProperty);
            var username = UpdateBinding("txtUsername", sender, TextBox.TextProperty);
            var realname = UpdateBinding("txtFullName", sender, TextBox.TextProperty);

            return !address.HasError && !port.HasError && !username.HasError && !realname.HasError;
        }

        private BindingExpression UpdateBinding(String controlName, Grid sender, DependencyProperty dp)
        {
            BindingExpression expression = ((TextBox)(sender.FindName(controlName))).GetBindingExpression(dp);
            expression.UpdateSource();
            return expression;
        }

        private void JoinChannelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var listChannels = (ListBox)((Grid)sender).FindName("lvChannels");
            var serverCoversation = ((UIServerConversation)e.Parameter);
            foreach (IrcChannel item in listChannels.SelectedItems)
            {
                serverCoversation.AddChannelConversation(item);
            }
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(((ListBox)textBox.FindName("lvChannels")).ItemsSource);
            collectionView.Filter = new Predicate<object>(i => NameFilter(i, textBox.Text));
        }

        public bool NameFilter(object item, String text)
        {
            IrcChannel channel = item as IrcChannel;
            return (channel.Name.ToLower().Contains(text.ToLower()));
        }

        private void AddChannelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var txtChannel = (TextBox)((Grid)sender).FindName("txtChannel");
            if (!String.IsNullOrEmpty(txtChannel.Text))
            {
                var startChars = new char[] { '&', '#', '!', '+' };
                var serverCoversation = ((UIServerConversation)e.Parameter);
                var name = txtChannel.Text;
                if (!startChars.Contains(name[0]))
                {
                    name = '#' + name;
                }
                var channel = new IrcChannel(serverCoversation.Session, name, String.Empty);
                if (serverCoversation.Session.Channels.Where(c => c.Name == name).Count() > 0)
                {
                    channel = serverCoversation.Session.Channels.Where(c => c.Name == name).First();
                }                
                serverCoversation.AddChannelConversation(channel);
            }
        }
    }
}
