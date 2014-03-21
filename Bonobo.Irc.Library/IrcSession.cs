using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Resources;
using System.Collections.ObjectModel;

namespace Bonobo.Irc
{
    /// <summary>
    /// Class that represents main interface for client communication.
    /// Represents server connection and commands that are available
    /// and pertains to server.
    /// </summary>
    public class IrcSession : INotifyPropertyChanged, IDisposable
    {

        private IrcConnection _connection;
        private IrcMessageFactory _messageFactory = new IrcMessageFactory();
        private IrcSessionState _state;
        private PropertyChangedEventHandler _propertyChanged;

        private readonly IrcConnectionInfo _connectionInfo = new IrcConnectionInfo();
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        private readonly EventHandlerList _events = new EventHandlerList();
        private readonly ObservableCollection<IrcConversation> _conversations = new ObservableCollection<IrcConversation>();
        private readonly IrcServerConversation _serverConversation;
        private readonly Object _lock = new Object();
        private readonly ObservableCollection<IrcChannel> _channels = new ObservableCollection<IrcChannel>();
        private readonly Queue<IrcMessage> _requestQue = new Queue<IrcMessage>();

        private static readonly Object EventStateChanged = new Object();


        public IrcSession()
        {
            ChangeState(IrcSessionState.Closed);
            _serverConversation = new IrcServerConversation(this);
            _conversations.Add(_serverConversation);
        }


        public event EventHandler<IrcStateChangedEventArgs> StateChanged
        {
            add { _events.AddHandler(EventStateChanged, value); }
            remove { _events.RemoveHandler(EventStateChanged, value); }
        }

        public IrcSessionState State
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged("State");
                }
            }
        }

        public IrcConnectionInfo ConnectionInfo
        {
            get { return _connectionInfo; }
        }

        public ObservableCollection<IrcConversation> Conversations
        {
            get { return _conversations; }
        }

        public IrcServerConversation ServerConversation
        {
            get { return _serverConversation; }
        }

        public ObservableCollection<IrcChannel> Channels
        {
            get { return _channels; }
        }

        internal IrcMessageFactory MessageFactory
        {
            get { return _messageFactory; }
        }

        /// <summary>
        /// Starts connection
        /// </summary>
        public void Open()
        {
            if (ConnectionInfo.Address == null || ConnectionInfo.Port <= 0)
            {
                throw new InvalidOperationException(Resources.AddressAndPortException);
            }

            _messageFactory.Username = ConnectionInfo.Username;
            _connection = new IrcConnection(new IPEndPoint(ConnectionInfo.Address, ConnectionInfo.Port));
            _connection.StateChanged += OnStateChanged;
            _connection.MessageReceived += OnMessageReceived;
            _connection.MessageSent += OnMessageSent;
            _connection.OpenAsync();
            StartConnection();
        }


        /// <summary>
        /// Send IRC message
        /// </summary>
        /// <param name="request">Request to send to server</param>
        public void Send(IrcMessage request)
        {
            if (_connection != null && _connection.State == IrcConnectionState.Opened)
            {
                _connection.SendAsync(request);
            }
            else
            {
                lock (_requestQue)
                {
                    _requestQue.Enqueue(request);
                }
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }

        /// <summary>
        /// Close session
        /// </summary>
        public void Close()
        {
            Close(null);
        }

        /// <summary>
        /// Close session
        /// </summary>
        /// <param name="message">QUIT message</param>
        public void Close(String message)
        {
            ChangeState(IrcSessionState.Closing);
            Send(_messageFactory.Quit(message));
        }

        /// <summary>
        /// Start channel listing
        /// </summary>
        public void ListChannels()
        {
            _channels.Clear();
            Send(_messageFactory.List());
        }

        private void StartConnection()
        {
            ChangeState(IrcSessionState.Opening);
            lock (_requestQue)
            {
                if (!String.IsNullOrEmpty(ConnectionInfo.ConnectionPassword))
                {
                    _requestQue.Enqueue(_messageFactory.SetConnectionPassword(ConnectionInfo.ConnectionPassword));
                    _requestQue.Enqueue(_messageFactory.SetNick(ConnectionInfo.Username));
                    Send(_messageFactory.SetUser(ConnectionInfo.Username, ConnectionInfo.Realname));
                }
                else
                {
                    _requestQue.Enqueue(_messageFactory.SetNick(ConnectionInfo.Username));
                    _requestQue.Enqueue(_messageFactory.SetUser(ConnectionInfo.Username, ConnectionInfo.Realname));
                }

                foreach (var item in _conversations)
                {
                    if (item is IrcChannelConversation)
                    {
                        _requestQue.Enqueue(_messageFactory.JoinChannel(((IrcChannelConversation)item).Name));
                    }
                }

                Pong("Start");
            }
        }

        private void OnMessageReceived(Object sender, IrcMessageEventArgs e)
        {
            var message = e.Message;
            var type = e.Message.Type;
            switch (type)
            {
                case IrcDefinition.Request.Ping:
                    Pong(message.Arguments[0]);
                    return;

                case IrcDefinition.Response.ErrorNotRegistred:
                    ChangeState(IrcSessionState.Opening, new IrcException(String.Format(Resources.NotRegistred, message.Arguments[0])));
                    break;

                case IrcDefinition.Response.ErrorNeedMoreParams:
                    NotifyChangeState(IrcSessionState.Error, new IrcException(String.Format(Resources.NeedMoreParams, message.Arguments[0])));
                    break;

                case IrcDefinition.Response.ErrorAlreadyRegistred:
                    NotifyChangeState(IrcSessionState.Error, new IrcException(String.Format(Resources.AlreadyRegistred, message.Arguments[0])));
                    break;

                case IrcDefinition.Response.ErrorNoNicknameGiven:
                    ChangeState(IrcSessionState.Opening, new IrcException(String.Format(Resources.NoNickname, message.Arguments[0])));
                    break;

                case IrcDefinition.Response.ErrorErrorneusNickname:
                    ChangeState(IrcSessionState.Opening, new IrcException(String.Format(String.Format(Resources.ErrorneusNickname, message.Arguments[0], message.Arguments[1]))));
                    break;

                case IrcDefinition.Response.ErrorNicknameInUse:
                    ChangeState(IrcSessionState.Opening, new IrcException(String.Format(Resources.NicknameInUse, message.Arguments[0], message.Arguments[1])));
                    break;

                case IrcDefinition.Response.ErrorNicknameCollision:
                    ChangeState(IrcSessionState.Opening, new IrcException(String.Format(Resources.NicknameCollision, message.Arguments[0], message.Arguments[1])));
                    break;

                case IrcDefinition.Request.Nick:
                case IrcDefinition.Response.ListStart:
                case IrcDefinition.Response.ListEnd:
                case IrcDefinition.Response.EndOfNames:

                    return;

                case IrcDefinition.Response.List:
                    _channels.Add(new IrcChannel(this, message.Arguments[1], (message.Arguments.Count > 3) ? message.Arguments[3] : String.Empty));
                    return;

                case IrcDefinition.Response.NoTopic:
                    {
                        AddMessageToChannelConversation(message, message.Arguments[1], IrcMessageType.ChannelResponse);
                        return;
                    }

                case IrcDefinition.Response.Topic:
                    {
                        if (Channels.Where(c => c.Name == message.Arguments[1]).Count() > 0)
                        {
                            Channels.Where(c => c.Name == message.Arguments[1]).First().Topic = message.Arguments[2];
                        }
                        AddMessageToChannelConversation(message, message.Arguments[1], IrcMessageType.ChannelResponse);
                        return;
                    }

                case IrcDefinition.Response.NameReply:
                    {
                        if (Channels.Where(c => c.Name == message.Arguments[1]).Count() > 0)
                        {
                            Channels.Where(c => c.Name == message.Arguments[1]).First().RetrieveUsers(message);
                        }
                        return;
                    }

                case IrcDefinition.Request.Join:
                    {
                        var name = IrcPerson.GetUsernameFromFullname(message.Prefix);
                        if (Channels.Where(c => c.Name == message.Arguments[0]).Count() > 0)
                        {
                            Channels.Where(c => c.Name == message.Arguments[0]).First().AddUser(new IrcPerson(this, name));
                        }
                        AddMessageToChannelConversation(message, message.Arguments[0], IrcMessageType.ChannelRequest);
                        return;
                    }

                case IrcDefinition.Request.Part:
                    {
                        var name = message.Prefix;
                        if (message.Prefix.IndexOf('!') > 0)
                        {
                            name = message.Prefix.Substring(0, message.Prefix.IndexOf('!'));
                        }
                        if (Channels.Where(c => c.Name == message.Arguments[0]).Count() > 0)
                        {
                            Channels.Where(c => c.Name == message.Arguments[0]).First().RemoveUser(new IrcPerson(this, name));
                        }
                        AddMessageToChannelConversation(message, message.Arguments[0], IrcMessageType.ChannelRequest);
                        return;
                    }

                case IrcDefinition.Response.ChannelCreation:
                    {
                        AddMessageToChannelConversation(message, message.Arguments[1], IrcMessageType.ChannelResponse);
                        return;
                    }

                case IrcDefinition.Response.ErrorNoSuchChannel:
                    {
                        var channels = _channels.Where(c => c.Name == message.Arguments[1]);
                        if (channels.Count() > 0)
                        {
                            _channels.Remove(channels.First());
                        }
                        AddMessageToChannelConversation(message, message.Arguments[1], IrcMessageType.ChannelResponse);
                        return;
                    }

                case IrcDefinition.Response.ErrorTooManyChannels:
                    {
                        var channels = _channels.Where(c => c.Name == message.Arguments[1]);
                        if (channels.Count() > 0)
                        {
                            _channels.Remove(channels.First());
                        }
                        AddMessageToChannelConversation(message, message.Arguments[1], IrcMessageType.ChannelResponse);
                        return;
                    }

                case IrcDefinition.Response.ErrorBannedFromChannel:
                case IrcDefinition.Response.ErrorInviteOnlyChannel:
                case IrcDefinition.Response.ErrorChannelIsFull:
                case IrcDefinition.Response.ErrorBadChannelKey:
                    {
                        AddMessageToChannelConversation(message, message.Arguments[1], IrcMessageType.ChannelResponse);
                        return;
                    }

                case IrcDefinition.Request.Topic:
                    {
                        if (Channels.Where(c => c.Name == message.Arguments[0]).Count() > 0)
                        {
                            Channels.Where(c => c.Name == message.Arguments[0]).First().Topic = message.Arguments[1];
                        }
                        AddMessageToChannelConversation(message, message.Arguments[0], IrcMessageType.ChannelRequest);
                        return;
                    }

                case IrcDefinition.Request.Mode:
                    {
                        if (IrcChannel.IsChannelName(message.Arguments[0]))
                        {
                            AddMessageToChannelConversation(message, message.Arguments[0], IrcMessageType.ChannelRequest);
                            return;
                        }
                        break;
                    }

                case IrcDefinition.Request.PrivateMessage:
                    {
                        if (IrcChannel.IsChannelName(message.Arguments[0]))
                        {
                            AddMessageToChannelConversation(message, message.Arguments[0], IrcMessageType.Message);
                            return;
                        }
                        else
                        {
                            AddMessageToPersonConversation(message, IrcPerson.GetUsernameFromFullname(message.Prefix), IrcMessageType.Message);
                            return;
                        }
                    }

                default:
                    {
                        var msg = message.Arguments.Count > 0 ? message.Arguments.Last() : message.ToString();
                        NotifyChangeState(IrcSessionState.Notification, msg);
                        break;
                    }
            }

            AddMessageToServerConversation(message);
        }

        internal void AddMessageToChannelConversation(IrcMessage message, String channelName, IrcMessageType type)
        {
            foreach (var c in _conversations)
            {
                if (c is IrcChannelConversation)
                {
                    if (((IrcChannelConversation)c).Name == channelName)
                    {
                        c.Messages.Add(new IrcConversationMessage()
                        {
                            TimeStamp = DateTime.Now,
                            Message = message.ToString(),
                            Type = type,
                            Username = _connectionInfo.Username,
                            MessageType = message.Type,
                        });
                        return;
                    }
                }
            }
        }

        internal void AddMessageToPersonConversation(IrcMessage message, String username, IrcMessageType type)
        {
            foreach (var c in _conversations)
            {
                if (c is IrcPersonConversation)
                {
                    if (((IrcPersonConversation)c).Name == username)
                    {
                        c.Messages.Add(new IrcConversationMessage()
                        {
                            TimeStamp = DateTime.Now,
                            Message = message.ToString(),
                            Type = type,
                            Username = _connectionInfo.Username,
                            MessageType = message.Type,
                        });
                        return;
                    }
                }
            }

            var newConversation = CreatePersonConversation(username);
            newConversation.Messages.Add(new IrcConversationMessage()
            {
                TimeStamp = DateTime.Now,
                Message = message.ToString(),
                Type = type,
                Username = _connectionInfo.Username,
                MessageType = message.Type,
            });
        }

        private IrcPersonConversation CreatePersonConversation(String username)
        {
            var c = new IrcPersonConversation(this, new IrcPerson(this, username));
            _conversations.Add(c);
            return c;
        }

        internal void AddMessageToServerConversation(IrcMessage message)
        {
            int x;
            var type = (Int32.TryParse(message.Type, out x)) ? IrcMessageType.ServerResponse : IrcMessageType.ServerRequest;
            _serverConversation.Messages.Add(new IrcConversationServerMessage()
            {
                TimeStamp = DateTime.Now,
                Message = message.ToString(),
                IsLoginError = message.IsLoginError(),
                Type = type,
                Username = _connectionInfo.Username,
                MessageType = message.Type,
            });
        }

        internal void AddMessageToConversation(String unknownCommand)
        {
            _serverConversation.Messages.Add(new IrcConversationMessage()
            {
                TimeStamp = DateTime.Now,
                Message = unknownCommand,
                Type = IrcMessageType.UnknownMessage
            });
        }

        private void OnStateChanged(Object sender, EventArgs e)
        {
            if (((IrcConnection)sender).State == IrcConnectionState.Opened)
            {
                lock (_requestQue)
                {
                    if (_requestQue.Count > 0)
                    {
                        _connection.SendAsync(_requestQue.Dequeue());
                    }
                }
            }
            else if (((IrcConnection)sender).State == IrcConnectionState.Faulted)
            {
                ChangeState(IrcSessionState.ServerError, new IrcException(Resources.ConnectionNotEstablished));
            }
        }

        private void OnMessageSent(Object sender, IrcMessageEventArgs e)
        {
            lock (_requestQue)
            {
                if (_requestQue.Count > 0)
                {
                    // End of registering and sending LIST command
                    if (_requestQue.Count == 1)
                    {
                        ChangeState(IrcSessionState.Opened);
                    }
                    _connection.SendAsync(_requestQue.Dequeue());
                }
            }
            if (State == IrcSessionState.Closing)
            {
                ChangeState(IrcSessionState.Closed);
                _connection.Dispose();
            }
        }

        private void Pong(String deamon)
        {
            Send(_messageFactory.Pong(deamon));
        }

        private void NotifyChangeState(IrcSessionState state, IrcException exception)
        {
            var previousState = _state;
            ChangeState(state, exception);
            ChangeState(previousState);
        }

        private void NotifyChangeState(IrcSessionState state, String message)
        {
            var previousState = _state;
            ChangeState(state, message);
            ChangeState(previousState);
        }

        private void ChangeState(IrcSessionState state, IrcException exception)
        {
            lock (_lock)
            {
                State = state;
                IrcStateChangedEventArgs arg;
                arg = new IrcStateChangedEventArgs(exception);
                _events.InvokeEvent(EventStateChanged, this, arg);
            }
        }

        private void ChangeState(IrcSessionState state)
        {
            ChangeState(state, String.Empty);
        }

        private void ChangeState(IrcSessionState state, String message)
        {
            lock (_lock)
            {
                State = state;
                IrcStateChangedEventArgs arg;
                arg = new IrcStateChangedEventArgs(message);
                _events.InvokeEvent(EventStateChanged, this, arg);
            }
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
