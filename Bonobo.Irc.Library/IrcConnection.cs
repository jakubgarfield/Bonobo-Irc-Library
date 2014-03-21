using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Bonobo.Irc
{
    /// <summary>
    /// Class that handles network connection for IRC library. 
    /// It receives messages through TCP protocol and parses 
    /// them into IrcMessage and send IrcMessage via TCP.
    /// </summary>
    public class IrcConnection : IDisposable
    {
        private sealed class AsyncState
        {
            public readonly TcpClient Client;
            public readonly Stream Stream;
            public readonly Byte[] Buffer;

            public AsyncState(TcpClient client, Stream stream, Byte[] buffer)
            {
                Client = client;
                Stream = stream;
                Buffer = buffer;
            }
        }

        private static readonly Object EventMessageReceived = new Object();
        private static readonly Object EventMessageSent = new Object();
        private static readonly Object EventStateChanged = new Object();

        private readonly IrcConnectionReceiveBuffer _receiveBuffer;
        private readonly EventHandlerList _events = new EventHandlerList();

        private volatile IrcConnectionState _state;
        private volatile Exception _faultReason;

        private TcpClient _client;
        private Stream _stream;
        private IPEndPoint _endPoint;

        /// <summary>
        /// Creates IrcConnection instance
        /// </summary>
        /// <param name="endPoint">Information about IRC server</param>
        public IrcConnection(IPEndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }
            _receiveBuffer = new IrcConnectionReceiveBuffer(32768, this);
            _endPoint = endPoint;
        }

        /// <summary>
        /// Event triggered when message is received from server
        /// </summary>
        public event EventHandler<IrcMessageEventArgs> MessageReceived
        {
            add { _events.AddHandler(EventMessageReceived, value); }
            remove { _events.RemoveHandler(EventMessageReceived, value); }
        }

        /// <summary>
        /// Event triggered when message is sent to server
        /// </summary>
        public event EventHandler<IrcMessageEventArgs> MessageSent
        {
            add { _events.AddHandler(EventMessageSent, value); }
            remove { _events.RemoveHandler(EventMessageSent, value); }
        }

        /// <summary>
        /// Event triggered when IrcConnection state is changed
        /// </summary>
        public event EventHandler<IrcConnectionStateEventArgs> StateChanged
        {
            add { _events.AddHandler(EventStateChanged, value); }
            remove { _events.RemoveHandler(EventStateChanged, value); }
        }

        public IrcConnectionState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged(new IrcConnectionStateEventArgs(value));
                }
            }
        }

        public Exception FaultReason
        {
            get { return _faultReason; }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (_events != null)
            {
                _events.Dispose();
            }

            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            if (State != IrcConnectionState.Faulted)
            {
                State = IrcConnectionState.Closed;
            }
        }

        /// <summary>
        /// Asynchronnous closing of connection
        /// </summary>
        public void CloseAsync()
        {
            if (State != IrcConnectionState.Faulted)
            {
                State = IrcConnectionState.Closing;
                Dispose();
            }
        }

        /// <summary>
        /// Asynchronnous opening of connection
        /// </summary>
        public void OpenAsync()
        {
            EnsureCreated();
            BeginConnect(null);
        }

        /// <summary>
        /// This method sends message to IRC server where IrcConnection is connected
        /// </summary>
        /// <param name="message">Message to send to IRC server</param>
        public void SendAsync(IrcMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("messages");
            }
            if (message.ToString().Length > 512)
            {
                throw new ArgumentException("message length");
            }
            var sendString = message.ToString() + IrcDefinition.NewLine;
            BeginWrite(IrcDefinition.Encoding.GetBytes(sendString));
        }

        protected virtual void OnMessageReceived(IrcMessageEventArgs e)
        {
            _events.InvokeEvent(EventMessageReceived, this, e);
        }

        protected virtual void OnStateChanged(IrcConnectionStateEventArgs e)
        {
            _events.InvokeEvent(EventStateChanged, this, e);
        }

        protected void Fault(Exception reason)
        {
            if (State == IrcConnectionState.Faulted)
            {
                throw new InvalidOperationException();
            }

            _state = IrcConnectionState.Faulted;
            _faultReason = reason;

            OnStateChanged(new IrcConnectionStateEventArgs(reason));
        }

        private void BeginConnect(Byte[] bufferToSend)
        {
            EnsureCreated();
            State = IrcConnectionState.Opening;

            if ((_client != null) && (_client.Connected))
            {
                _stream = _client.GetStream();
                _state = IrcConnectionState.Opened;

                BeginRead();
            }
            else
            {
                _client = new TcpClient();
                _client.BeginConnect(_endPoint.Address, _endPoint.Port, OnConnectCompleted, CreateAsyncState(bufferToSend));
            }
        }

        private void BeginRead()
        {
            if (State == IrcConnectionState.Created)
            {
                BeginConnect(null);
            }
            else if (State == IrcConnectionState.Opened)
            {
                _receiveBuffer.BeginReadFrom(_stream, OnReadCompleted, CreateAsyncState(null));
            }
        }

        private void BeginWrite(Byte[] buffer)
        {
            if (State == IrcConnectionState.Created)
            {
                BeginConnect(buffer);
            }
            else if (State == IrcConnectionState.Opened)
            {
                _stream.BeginWrite(buffer, 0, buffer.Length, OnWriteCompleted, CreateAsyncState(null));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void OnConnectCompleted(IAsyncResult ar)
        {
            var asyncState = (AsyncState)(ar.AsyncState);

            try
            {
                asyncState.Client.EndConnect(ar);

                _client = asyncState.Client;
                _stream = asyncState.Client.GetStream();

                State = IrcConnectionState.Opened;                
                BeginRead();

                if (asyncState.Buffer != null)
                {
                    BeginWrite(asyncState.Buffer);
                }
            }
            catch (Exception exc)
            {
                Fault(exc);
            }
        }

        private void OnReadCompleted(IAsyncResult ar)
        {
            var asyncState = (AsyncState)(ar.AsyncState);

            try
            {
                var read = _receiveBuffer.EndReadFrom(asyncState.Stream, ar);

                if ((State == IrcConnectionState.Opened) && (read > 0))
                {
                    foreach (var msg in _receiveBuffer.ReadMessages())
                    {
                        OnMessageReceived(new IrcMessageEventArgs(msg));
                    }
                    BeginRead();
                }                
            }
            catch (Exception exc)
            {
                Fault(exc);
            }
        }

        private void OnWriteCompleted(IAsyncResult ar)
        {
            var asyncState = (AsyncState)(ar.AsyncState);

            try
            {
                asyncState.Stream.EndWrite(ar);
                _events.InvokeEvent(EventMessageSent, this, new IrcMessageEventArgs(null));
                
            }
            catch (Exception exc)
            {
                Fault(exc);
            }
        }

        private AsyncState CreateAsyncState(Byte[] buffer)
        {
            return new AsyncState(_client, _stream, buffer);
        }

        private void EnsureCreated()
        {
            EnsureNotClosed();

            if (State != IrcConnectionState.Created)
            {
                throw new InvalidOperationException();
            }
        }

        private void EnsureNotClosed()
        {
            if (State == IrcConnectionState.Closed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
