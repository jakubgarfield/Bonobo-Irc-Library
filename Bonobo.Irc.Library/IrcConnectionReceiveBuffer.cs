using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bonobo.Irc
{
    /// <summary>
    /// Class is used to read and parse IRC messages from network.
    /// </summary>
    internal sealed class IrcConnectionReceiveBuffer
    {
        private Byte[] _buffer;
        private Int32 _count;
        private IrcConnection _connection;

        /// <summary>
        /// Creates instance
        /// </summary>
        /// <param name="size">Defines the size of the buffer</param>
        /// <param name="connection">IrcConnection used to read message</param>
        public IrcConnectionReceiveBuffer(Int32 size, IrcConnection connection)
        {
            _buffer = new Byte[size];
            _connection = connection;
        }

        /// <summary>
        /// Reads data from stream
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="callback">Callback to call after reading is done</param>
        public void BeginReadFrom(Stream stream, AsyncCallback callback)
        {
            BeginReadFrom(stream, callback, null);
        }

        /// <summary>
        /// Begins data reading from stream
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="callback">Callback to call after reading is done</param>
        /// <param name="state">Parameter to callback</param>
        public void BeginReadFrom(Stream stream, AsyncCallback callback, Object state)
        {
            stream.BeginRead(_buffer, _count, _buffer.Length - _count, callback, state);
        }

        /// <summary>
        /// Ends reading from stream
        /// </summary>
        /// <param name="stream">Stream to stop reading from</param>
        /// <param name="ar">Result of reading</param>
        /// <returns></returns>
        public Int32 EndReadFrom(Stream stream, IAsyncResult ar)
        {
            if (_connection.State == IrcConnectionState.Opened)
            {
                var read = stream.EndRead(ar);

                if (read > 0)
                {
                    _count += read;
                }

                var str = IrcDefinition.Encoding.GetString(_buffer, 0, _count);
                return read;
            }
            else
            {
                return 0;
            }
        }

        public IEnumerable<IrcMessage> ReadMessages()
        {
            const Byte CR = 13;
            const Byte LF = 10;

            var start = 0;
            var end = 0;

            while (end < _count)
            {
                if ((_buffer[end] == CR) || (_buffer[end] == LF))
                {
                    if ((_buffer[end] == CR) && (end < _buffer.Length - 1) && (_buffer[end + 1] == LF))
                    {
                        end++;
                    }
                    IrcMessage msg;
                    var str = IrcDefinition.Encoding.GetString(_buffer, start, (end - start));
                    if (IrcMessage.TryParse(str.Trim('\r').Trim('\n'), out msg))
                    {
                        yield return msg;
                    }

                    start = end + 1;
                }

                end++;
            }

            if (start < end)
            {
                var newbuffer = new Byte[_buffer.Length];
                Array.Copy(_buffer, start, newbuffer, 0, _count - start);

                _buffer = newbuffer;
                _count -= start;
            }
            else
            {
                _count = 0;
            }
        }
    }
}
