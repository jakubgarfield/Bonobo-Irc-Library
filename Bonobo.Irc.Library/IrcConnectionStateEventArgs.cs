using System;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents arguments that are given to IrcConnection state changed event
    /// </summary>
    [Serializable]
    public sealed class IrcConnectionStateEventArgs : EventArgs
    {
        private readonly Exception _faultReason;
        private readonly IrcConnectionState _state;

        internal IrcConnectionStateEventArgs(IrcConnectionState state)
            : this(state, null)
        {
        }

        internal IrcConnectionStateEventArgs(Exception faultReason)
            : this(IrcConnectionState.Faulted, faultReason)
        {
        }

        private IrcConnectionStateEventArgs(IrcConnectionState state, Exception faultReason)
        {
            _state = state;
            _faultReason = faultReason;
        }

        public IrcConnectionState State
        {
            get { return _state; }
        }

        public Exception FaultReason
        {
            get { return _faultReason; }
        }
    }
}