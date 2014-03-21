using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Bonobo.Irc.Client.UI.Data
{
    [ValueConversion(typeof(object), typeof(String))]
    public class MessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var message = (IrcConversationMessage)value;

            switch (message.Type)
            {
                case IrcMessageType.ServerResponse:
                    return FormatServerResponse(message);

                case IrcMessageType.ServerRequest:
                    return FormatServerRequest(message);

                case IrcMessageType.ChannelRequest:
                    return FormatChannelRequest(message);

                case IrcMessageType.Message:
                case IrcMessageType.MyMessage:
                    return FormatMessage(message);

                case IrcMessageType.ChannelResponse:
                    return FormatChannelResponse(message);

                default:
                    return message.Message.Trim('\r').Trim('\n');
            }
        }

        private String FormatChannelResponse(IrcConversationMessage conversation)
        {
            var message = RemovePrefix(conversation.Message);
            message = RemoveType(message);
            message = RemoveParameter(message);
            return message;
        }

        private String FormatMessage(IrcConversationMessage conversation)
        {
            var str = String.Empty;
            str = "<" +RetrieveUsername(conversation.Message) +"> ";
            
            var message = RemovePrefix(conversation.Message);
            message = RemoveType(message);
            message = RemoveParameter(message);
            message = RemoveColon(message);
            str += message;

            return str;
        }

        private String FormatChannelRequest(IrcConversationMessage conversation)
        {
            var str = String.Empty;
            str = RetrieveUsername(conversation.Message);

            if (String.Equals(conversation.MessageType, IrcDefinition.Request.Join))
            {
                str += " joined channel";
            }
            else if (String.Equals(conversation.MessageType, IrcDefinition.Request.Mode))
            {
                var message = RemovePrefix(conversation.Message);
                message = RemoveType(message);
                message = RemoveParameter(message);
                str += " sets mode ";
                str += message;
            }
            else if (String.Equals(conversation.MessageType, IrcDefinition.Request.Part))
            {
                str += " has left channel";
            }
            else if (String.Equals(conversation.MessageType, IrcDefinition.Request.Topic))
            {
                var message = RemovePrefix(conversation.Message);
                message = RemoveType(message);
                message = RemoveParameter(message);
                str += " sets topic to ";
                str += message;
            }
            else
            {
                return conversation.Message;
            }

            return str;
        }

        private String FormatServerRequest(IrcConversationMessage conversation)
        {
            return RemovePrefix(conversation.Message);
        }

        private String FormatServerResponse(IrcConversationMessage conversation)
        {
            var message = RemovePrefix(conversation.Message);
            message = RemoveType(message);
            message = RemoveColon(message);

            if (message.Contains(' '))
            {
                if ((message.Substring(0, message.IndexOf(' ')) == conversation.Username))
                {
                    message = message.Remove(0, message.IndexOf(' ') + 1);
                }
            }
            message = RemoveColon(message);

            return RemoveLineEnd(message);
        }

        private String RetrieveUsername(String message)
        {
            message = RemoveColon(message);
            return IrcPerson.GetUsernameFromFullname(message.Substring(0, message.IndexOf(' ')));
        }

        private String RemovePrefix(String message)
        {
            var str = message;
            if (message[0] == ':')
            {
                if (message.IndexOf(' ') >= 0)
                {
                    str = message.Remove(0, message.IndexOf(' ') + 1);
                }
            }
            if (str[0] == ' ')
            {
                return str.Substring(1);
            }
            return str;
        }

        private String RemoveParameter(String message)
        {
            var str = message;
            if (message[0] == ':')
            {
                if (message.IndexOf(' ') >= 0)
                {
                    str = message.Remove(0, message.IndexOf(' ') + 1);
                }
            }
            if (str.IndexOf(' ') >= 0)
            {
                return str.Remove(0, str.IndexOf(' ') + 1);
            }
            return str;
        }

        private String RemoveType(String message)
        {
            var str = message;
            if (message.IndexOf(' ') >= 0)
            {
                str = message.Remove(0, message.IndexOf(' ') + 1);
            }
            return str;
        }

        private String RemoveColon(String message)
        {
            if (message[0] == ':')
            {
                message = message.Remove(0, 1);
            }
            return message;
        }

        private String RemoveLineEnd(String message)
        {
            return message.Trim('\r').Trim('\n');
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
