using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bonobo.Irc.Test
{
    /// <summary>
    /// Summary description for IrcLibrary
    /// </summary>
    [TestClass]
    public class IrcLibrary
    {
        public IrcLibrary()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IrcConnection_CtorNullParameter()
        {
            var connection = new IrcConnection(null);
        }

        [TestMethod]
        public void IrcChannel_Name()
        {
            var session = new IrcSession();
            var channel = new IrcChannel(session, "Test", "");
            Assert.AreEqual("Test", channel.Name);
        }

        [TestMethod]
        public void IrcChannel_NameWithSpace()
        {
            var session = new IrcSession();
            var channel = new IrcChannel(session, "Test space", "");
            Assert.AreEqual("Test", channel.Name);
        }

        [TestMethod]
        public void IrcChannel_Topic()
        {
            var session = new IrcSession();
            var channel = new IrcChannel(session, String.Empty, ":Test topic");
            Assert.AreEqual("Test topic", channel.Topic);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void IrcChannel_TopicNull()
        {
            var session = new IrcSession();
            var channel = new IrcChannel(session, String.Empty, null);
        }

        [TestMethod]
        public void IrcChannel_TopicEmpty()
        {
            var session = new IrcSession();
            var channel = new IrcChannel(session, String.Empty, String.Empty);
            Assert.AreEqual(String.Empty, channel.Topic);
        }
    }
}
