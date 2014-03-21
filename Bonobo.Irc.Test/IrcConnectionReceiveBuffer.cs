using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bonobo.Irc;
using System.IO;
using System.Threading;
using System.Net;

namespace Bonobo.Irc.Test
{
    namespace Bonobo.Irc.Tests
    {
        [TestClass]
        public class IrcConnectionReceiveBufferTest
        {
            private static IrcConnectionReceiveBuffer _buffer;

            public IrcConnectionReceiveBufferTest()
            {
                var connection = new IrcConnection(new IPEndPoint(IPAddress.Parse("147.32.80.79"), 6667));
                connection.State = IrcConnectionState.Opened;
                _buffer = new IrcConnectionReceiveBuffer(100, connection);
            }

            [TestMethod]
            public void RecvBuf_SingleMessage()
            {
                var msg = ParseMessages(":user USER arg1 arg2 arg3 \r\n", 100).SingleOrDefault();
                Assert.IsNotNull(msg);

                Assert.AreEqual("USER", msg.Type);
                Assert.AreEqual("user ", msg.Prefix);
                Assert.AreEqual(3, msg.Arguments.Count);
                Assert.AreEqual("arg1", msg.Arguments[0]);
                Assert.AreEqual("arg2", msg.Arguments[1]);
                Assert.AreEqual("arg3", msg.Arguments[2]);
            }

            [TestMethod]
            public void RecvBuf_HalfMessage()
            {
                Read(_buffer, ":prefix JO");
                Read(_buffer, "IN arg1 \r\n");

                var msg = _buffer.ReadMessages().SingleOrDefault();
                Assert.IsNotNull(msg);
                Assert.AreEqual("prefix ", msg.Prefix);
                Assert.AreEqual("JOIN", msg.Type);
                Assert.AreEqual(1, msg.Arguments.Count);
                Assert.AreEqual("arg1", msg.Arguments[0]);
            }

            private static IrcMessage[] ParseMessages(String str, Int32 bufferSize)
            {
                Read(_buffer, str);

                return _buffer.ReadMessages().ToArray();
            }

            private static void Read(IrcConnectionReceiveBuffer buffer, String str)
            {
                var syncRoot = new Object();
                var stream = CreateStream(str);

                lock (syncRoot)
                {
                    buffer.BeginReadFrom(stream, ar =>
                    {
                        buffer.EndReadFrom(stream, ar);

                        lock (syncRoot)
                        {
                            Monitor.Pulse(syncRoot);
                        }
                    });

                    Monitor.Wait(syncRoot);
                }
            }

            private static Stream CreateStream(String msg)
            {
                return new MemoryStream(IrcDefinition.Encoding.GetBytes(msg));
            }
        }
    }
}
