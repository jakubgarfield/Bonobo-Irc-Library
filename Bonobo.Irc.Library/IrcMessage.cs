using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bonobo.Irc
{
    /// <summary>
    /// Class represents IRC message. It is used to parse messages
    /// received from server and it's object representation of message
    /// </summary>
    public class IrcMessage
    {
        protected const char Space = ' ';
        protected const char Colon = ':';

        protected String _prefix;
        protected String[] _arguments;
        protected String _command;
        protected String _type;

        private IrcMessage()
        {
        }

        public IrcMessage(String type, String prefix, params String[] arguments)
        {
            _prefix = prefix;
            _type = type;
            _arguments = arguments;
            _command = String.Empty;
            CreateCommand();
        }

        public IList<String> Arguments
        {
            get { return _arguments; }
        }

        public String Prefix
        {
            get { return _prefix; }
        }

        public String Message
        {
            get { return GetMessageWithoutPrefix(_prefix, _command); }
        }

        public String Type
        {
            get { return _type; }
        }

        public override string ToString()
        {
            return _command;
        }

        public bool IsLoginError()
        {
            switch (_type)
            {
                case IrcDefinition.Response.ErrorErrorneusNickname:
                case IrcDefinition.Response.ErrorNicknameCollision:
                case IrcDefinition.Response.ErrorNicknameInUse:
                case IrcDefinition.Response.ErrorNoNicknameGiven:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts message to byte array and writes to stream
        /// </summary>
        public void WriteTo(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var buffer = ToByteArray();
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Converts message to byte array
        /// </summary>
        /// <returns>Message as byte array</returns>
        public Byte[] ToByteArray()
        {
            var str = ToString();
            return IrcDefinition.Encoding.GetBytes(str);
        }


        protected void HandleSpaces()
        {
            for (int i = 0; i < _arguments.Length; i++)
            {
                if (_arguments[i].Contains(Space))
                {
                    _arguments[i] = Colon + _arguments[i];
                }
            }
        }

        protected void CreateCommand()
        {
            HandleSpaces();

            if (String.IsNullOrEmpty(_prefix))
            {
                _command = _type;
            }
            else
            {
                _command = Colon + _prefix + Space + _type;
            }

            foreach (var item in _arguments)
            {
                _command += Space + item;
            }
        }


        protected static string GetMessageArguments(String messageWithoutPrefix, String command)
        {
            if (messageWithoutPrefix.Length < command.Length + 1)
            {
                return String.Empty;
            }
            messageWithoutPrefix = messageWithoutPrefix.Remove(0, command.Length + 1);
            if (!String.IsNullOrEmpty(messageWithoutPrefix) && messageWithoutPrefix[0] == ' ')
            {
                messageWithoutPrefix = messageWithoutPrefix.Remove(0, 1);
            }
            return messageWithoutPrefix;
        }

        protected static string GetMessageWithoutPrefix(String prefix, String message)
        {
            if (String.IsNullOrEmpty(prefix))
            {
                if (message.StartsWith(": "))
                {
                    message.Substring(2);
                }
                return message;
            }
            else
            {
                return message.Remove(0, prefix.Length + 1);
            }
        }

        protected static String[] ParseArguments(String arguments)
        {
            var list = new List<String>();
            var arr = arguments.ToCharArray();

            var temp = String.Empty;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == ':' && i + 1 < arr.Length)
                {
                    list.Add(arguments.Substring(i + 1));
                    break;
                }
                else
                {
                    if (arr[i] != ' ')
                    {
                        temp += arr[i];
                    }
                    else
                    {
                        list.Add(temp);
                        temp = String.Empty;
                    }

                }
            }

            return list.ToArray();
        }

        protected static String GetMessageType(String msgWithoutPrefix)
        {
            return (msgWithoutPrefix.IndexOf(' ') <= 0) ? msgWithoutPrefix : msgWithoutPrefix.Substring(0, msgWithoutPrefix.IndexOf(' '));
        }

        protected static String GetMessagePrefix(string message)
        {
            if (message.StartsWith(":") && message.Length > 1 && message.IndexOf(' ') > 0)
            {
                return message.Substring(1, message.IndexOf(' '));
            }

            return String.Empty;
        }

        public static bool TryParse(String message, out IrcMessage output)
        {
            if (!String.IsNullOrEmpty(message))
            {
                var factory = new IrcMessageFactory();

                String prefix = GetMessagePrefix(message);
                String msgWithoutPrefix = GetMessageWithoutPrefix(prefix, message);

                String type = GetMessageType(msgWithoutPrefix);
                String msgArguments = GetMessageArguments(msgWithoutPrefix, type);
                String[] arguments = ParseArguments(msgArguments);

                if (type.Equals(IrcDefinition.Request.Ping, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]))
                    {
                        output = factory.Ping(arguments[0]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorNotRegistred, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.NotRegistred(prefix, arguments[0], arguments[1]);
                        return true;
                    }
                    else if (!String.IsNullOrEmpty(arguments[0]))
                    {
                        output = factory.NotRegistred(prefix, arguments[0]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorAlreadyRegistred, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.AlreadyRegistred(prefix, arguments[1]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorNeedMoreParams, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.NeedMoreParams(prefix, arguments[0], arguments[1]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorNoNicknameGiven, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]))
                    {
                        output = factory.NoNickNameGiven(prefix, arguments[0]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorErrorneusNickname, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.ErrorneusNickname(prefix, arguments[0], arguments[1]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorNicknameInUse, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.NicknameInUse(prefix, arguments[0], arguments[1]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorNicknameCollision, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.NicknameCollision(prefix, arguments[0], arguments[1]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ListStart, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]))
                    {
                        output = factory.ListStart(prefix, arguments[0]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ListEnd, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]))
                    {
                        output = factory.ListEnd(prefix, arguments[0]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.List, StringComparison.Ordinal))
                {
                    if (arguments.Length > 1)
                    {
                        var nick = arguments[0];
                        var channel = (arguments.Length > 1 && !String.IsNullOrEmpty(arguments[1])) ? arguments[1] : String.Empty;
                        var visibility = (arguments.Length > 2 && !String.IsNullOrEmpty(arguments[2])) ? arguments[2] : String.Empty;
                        var topic = (arguments.Length > 3 && !String.IsNullOrEmpty(arguments[3])) ? arguments[3] : String.Empty;
                        output = factory.List(prefix, nick, channel, visibility, topic);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.EndOfNames, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]) && !String.IsNullOrEmpty(arguments[2]))
                    {
                        output = factory.EndOfNames(prefix, arguments[0], arguments[1], arguments[2]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.NoTopic, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.NoTopic(prefix, arguments[0], arguments[1]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.Topic, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]) && !String.IsNullOrEmpty(arguments[2]))
                    {
                        output = factory.Topic(prefix, arguments[0], arguments[1], arguments[2]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.NameReply, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]) && !String.IsNullOrEmpty(arguments[2]))
                    {
                        output = factory.NameReply(prefix, arguments[0], arguments[2], arguments[3].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ChannelCreation, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]) && !String.IsNullOrEmpty(arguments[2]))
                    {
                        output = factory.ChannelCreation(prefix, arguments[0], arguments[1], arguments[2], (arguments.Length > 3) ? arguments[3] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorNoSuchChannel, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.NoSuchChannel(prefix, arguments[0], arguments[1], (arguments.Length > 2) ? arguments[2] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorTooManyChannels, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.TooManyChannels(prefix, arguments[0], arguments[1], (arguments.Length > 2) ? arguments[2] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorBannedFromChannel, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.BannedFromChannel(prefix, arguments[0], arguments[1], (arguments.Length > 2) ? arguments[2] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorInviteOnlyChannel, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.InviteOnlyChannel(prefix, arguments[0], arguments[1], (arguments.Length > 2) ? arguments[2] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorChannelIsFull, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.ChannelIsFull(prefix, arguments[0], arguments[1], (arguments.Length > 2) ? arguments[2] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Response.ErrorBadChannelKey, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.BadChannelKey(prefix, arguments[0], arguments[1], (arguments.Length > 2) ? arguments[2] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Request.Topic, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]))
                    {
                        output = factory.Topic(prefix, arguments[0], (arguments.Length > 1) ? arguments[1] : String.Empty);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Request.Mode, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        if (arguments.Length == 2)
                        {
                            output = factory.Mode(prefix, arguments[0], arguments[1]);
                            return true;
                        }
                        else if (arguments.Length > 2)
                        {
                            output = factory.Mode(prefix, arguments[0], arguments[1], arguments[2]);
                            return true;
                        }
                    }
                }
                else if (type.Equals(IrcDefinition.Request.PrivateMessage, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]) && !String.IsNullOrEmpty(arguments[1]))
                    {
                        output = factory.PrivateMessage(prefix, arguments[0], arguments[1]);
                        return true;
                    }
                }
                else if (type.Equals(IrcDefinition.Request.Nick, StringComparison.Ordinal))
                {
                    if (!String.IsNullOrEmpty(arguments[0]))
                    {
                        output = factory.SetNick(arguments[0]);
                        return true;
                    }
                }
                else
                {
                    output = new IrcMessage(type, prefix, arguments);
                    return true;
                }
            }

            output = new IrcMessage();
            return false;
        }
    }
}
