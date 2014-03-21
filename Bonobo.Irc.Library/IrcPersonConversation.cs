using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Represents person IRC conversation
    /// </summary>
    public class IrcPersonConversation : IrcConversation
    {
        private IrcPerson _person;

        public IrcPersonConversation(IrcSession session, IrcPerson person) : base(session)
        {
            _person = person;
        }

        public String Name
        {
            get { return _person.Name; }
        }

        public IrcPerson Person
        {
            get { return _person; }
        }
    }
}
