using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Bonobo.Irc.Client.UI
{
    public class IrcCommands
    {
        public static readonly RoutedUICommand Disconnect = new RoutedUICommand("Disconnect", "Disconnect", typeof(IrcCommands));
        public static readonly RoutedUICommand Connect = new RoutedUICommand("Connect", "Connect", typeof(IrcCommands));
        public static readonly RoutedUICommand Login = new RoutedUICommand("Login", "Login", typeof(IrcCommands));
        public static readonly RoutedUICommand List = new RoutedUICommand("List", "List", typeof(IrcCommands));
        public static readonly RoutedUICommand JoinChannel = new RoutedUICommand("JoinChannel", "JoinChannel", typeof(IrcCommands));
        public static readonly RoutedUICommand AddChannel = new RoutedUICommand("AddChannel", "AddChannel", typeof(IrcCommands));
        public static readonly RoutedUICommand Chat = new RoutedUICommand("Chat", "Chat", typeof(IrcCommands));
        public static readonly RoutedUICommand CloseAllConversations = new RoutedUICommand("CloseAllConversations", "CloseAllConversations", typeof(IrcCommands));
        public static readonly RoutedUICommand CloseConversation = new RoutedUICommand("CloseConversation", "CloseConversation", typeof(IrcCommands));
        public static readonly RoutedUICommand Send = new RoutedUICommand("Send", "Send", typeof(IrcCommands));
    }
}
