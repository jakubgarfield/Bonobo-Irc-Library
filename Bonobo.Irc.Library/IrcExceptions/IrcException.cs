using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    [global::System.Serializable]
    public class IrcException : Exception
    {
        public IrcException() { }
        public IrcException(string message) : base(message) { }
        public IrcException(string message, Exception inner) : base(message, inner) { }
        protected IrcException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
