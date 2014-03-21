using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bonobo.Irc.Test
{
    [TestClass]
    public class IrcMessageTest
    {
        [TestMethod]
        public void IrcMsg_CtorEmptyCommand()
        {
            new IrcMessage("", "");
        }

        [TestMethod]
        public void IrcMsg_CtorNullCommand()
        {
            new IrcMessage(null, null);
        }

        [TestMethod]
        public void IrcMsg_ToStringNoPrefixNoArgs()
        {
            var expected = "PING";

            var actual = new IrcMessage("PING", "");
            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void IrcMsg_ToStringWithPrefixNoArgs()
        {
            var expected = ":jakub PING";

            var actual = new IrcMessage("PING", "jakub");
            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void IrcMsg_ToStringArgWithSpace()
        {
            var expected = ":prefix PRIVMSG blabla :string with a lot of spaces";
            var actual = new IrcMessage("PRIVMSG", "prefix", "blabla", "string with a lot of spaces");

            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void IrcMsg_ParseNullString()
        {
            IrcMessage msg;
            Assert.IsFalse(IrcMessage.TryParse(null, out msg));
        }

        [TestMethod]
        public void IrcMsg_ParseEmptyString()
        {
            IrcMessage msg;
            Assert.IsFalse(IrcMessage.TryParse("", out msg));
        }

        [TestMethod]
        public void IrcMsg_ParseNoPrefixNoArgs()
        {
            IrcMessage msg;
            Assert.IsTrue(IrcMessage.TryParse("PING\r\n", out msg));
            Assert.AreEqual("PING\r\n", msg.Type);
            Assert.AreEqual(String.Empty, msg.Prefix);
        }

        [TestMethod]
        public void IrcMsg_ParseWithPrefixNoArgs()
        {
            IrcMessage msg;
            Assert.IsTrue(IrcMessage.TryParse(":nick!user@hostname PONG \r\n", out msg));
            Assert.AreEqual("PONG", msg.Type);
            Assert.AreEqual("nick!user@hostname ", msg.Prefix);
        }

        [TestMethod]
        public void IrcMsg_ParseWithPrefixSpaceArgs()
        {
            IrcMessage msg;
            Assert.IsTrue(IrcMessage.TryParse(":nick!user@hostname PRIVMSG arg1 :arg2 arg2 arg2 arg2\r\n", out msg));
            Assert.AreEqual("PRIVMSG", msg.Type);
            Assert.AreEqual("nick!user@hostname ", msg.Prefix);

            Assert.AreEqual(2, msg.Arguments.Count);
            Assert.AreEqual("arg1", msg.Arguments[0]);
            Assert.AreEqual(":arg2 arg2 arg2 arg2\r\n", msg.Arguments[1]);
        }
    }
}
