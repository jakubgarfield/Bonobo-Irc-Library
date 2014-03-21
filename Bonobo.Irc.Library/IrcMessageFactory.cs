using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bonobo.Irc
{
    /// <summary>
    /// Protocol class that returns object representation of IRC message
    /// </summary>
    internal class IrcMessageFactory
    {
        internal IrcMessageFactory()
        {

        }

        internal IrcMessageFactory(String username)
        {
            Username = username;
        }

        internal String Username
        {
            get;
            set;
        }

        // Requests

        internal IrcMessage Pong(String daemon)
        {
            return new IrcMessage(IrcDefinition.Request.Pong, Username, daemon);
        }

        internal IrcMessage Ping(String daemon)
        {
            return new IrcMessage(IrcDefinition.Request.Ping, Username, daemon);
        }

        internal IrcMessage Quit(String quitMessage)
        {
            if (String.IsNullOrEmpty(quitMessage))
            {
                return new IrcMessage(IrcDefinition.Request.Quit, Username);
            }
            return new IrcMessage(IrcDefinition.Request.Quit, Username, quitMessage);
        }

        internal IrcMessage List()
        {
            return new IrcMessage(IrcDefinition.Request.List, Username);
        }

        internal IrcMessage SetConnectionPassword(String password)
        {
            return new IrcMessage(IrcDefinition.Request.Pass, Username, password);
        }

        internal IrcMessage SetNick(String nickname)
        {
            return new IrcMessage(IrcDefinition.Request.Nick, Username, nickname);
        }

        internal IrcMessage SetUser(String username, String realname)
        {
            String hostname = "hostname";
            String servername = "servername";
            return new IrcMessage(IrcDefinition.Request.User, Username, username, hostname, servername, realname);
        }

        internal IrcMessage JoinChannel(String name)
        {
            return new IrcMessage(IrcDefinition.Request.Join, Username, name);
        }

        internal IrcMessage JoinChannel(String name, String password)
        {
            return new IrcMessage(IrcDefinition.Request.Join, Username, name, password);
        }

        internal IrcMessage Part(String channelName)
        {
            return new IrcMessage(IrcDefinition.Request.Part, Username, channelName);
        }


        internal IrcMessage Names(String channelName)
        {
            return new IrcMessage(IrcDefinition.Request.Names, Username, channelName);
        }

        internal IrcMessage Topic(String user, String channelName, String topic)
        {
            return new IrcMessage(IrcDefinition.Request.Topic, user, channelName, topic);
        }

        internal IrcMessage PrivateMessage(String from, String to, String message)
        {
            return new IrcMessage(IrcDefinition.Request.PrivateMessage, from, to, message);
        }

        internal IrcMessage PrivateMessage(String to, String message)
        {
            return new IrcMessage(IrcDefinition.Request.PrivateMessage, Username, to, message);
        }

        internal IrcMessage Mode(String user, String target, String type, String parameter)
        {
            return new IrcMessage(IrcDefinition.Request.Mode, user, target, type, parameter);
        }

        internal IrcMessage Mode(String user, String target, String type)
        {
            return Mode(user, target, type, String.Empty);
        }

        internal IrcMessage Mode(String target, String type)
        {
            return Mode(Username, target, type, String.Empty);
        }

        // Responses

        internal IrcMessage NotRegistred(String prefix, String parameter, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorNotRegistred, prefix, parameter, errorMessage);
        }

        internal IrcMessage NotRegistred(String prefix, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorNotRegistred, prefix, errorMessage);
        }

        internal IrcMessage AlreadyRegistred(String prefix, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorAlreadyRegistred, prefix, errorMessage);
        }

        internal IrcMessage NeedMoreParams(String prefix, String command, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorNeedMoreParams, prefix, command, errorMessage);
        }

        internal IrcMessage NoNickNameGiven(String prefix, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorNoNicknameGiven, prefix, errorMessage);
        }

        internal IrcMessage ErrorneusNickname(String prefix, String nick, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorErrorneusNickname, prefix, errorMessage);
        }

        internal IrcMessage NicknameInUse(String prefix, String nick, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorNicknameInUse, prefix, nick, errorMessage);
        }

        internal IrcMessage NicknameCollision(String prefix, String nick, String errorMessage)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorNicknameCollision, prefix, nick, errorMessage);
        }

        internal IrcMessage List(String prefix, String nick, String channel, String visible, String topic)
        {
            return new IrcMessage(IrcDefinition.Response.List, prefix, nick, channel, visible, topic);
        }

        internal IrcMessage ListStart(String prefix, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ListStart, prefix, message);
        }

        internal IrcMessage ListEnd(String prefix, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ListEnd, prefix, message);
        }

        internal IrcMessage EndOfNames(String prefix, String postfix, String channel, String message)
        {
            return new IrcMessage(IrcDefinition.Response.EndOfNames, prefix, postfix, channel, message);
        }

        internal IrcMessage NoTopic(String prefix, String postfix, String channel)
        {
            return new IrcMessage(IrcDefinition.Response.NoTopic, prefix, postfix, channel);
        }

        internal IrcMessage Topic(String prefix, String postfix, String channel, String topic)
        {
            return new IrcMessage(IrcDefinition.Response.Topic, prefix, postfix, channel, topic);
        }

        internal IrcMessage NameReply(String prefix, String postfix, String channel, params String[] names)
        {
            var arr = new String[names.Length + 2];
            arr[0] = postfix;
            arr[1] = channel;
            for (int i = 2; i < names.Length + 2; i++)
            {
                arr[i] = names[i - 2];
            }
            return new IrcMessage(IrcDefinition.Response.NameReply, prefix, arr);
        }

        internal IrcMessage ChannelCreation(String prefix, String postfix, String channel, String creator, String creationString)
        {
            return new IrcMessage(IrcDefinition.Response.ChannelCreation, prefix, postfix, channel, creator, creationString);
        }

        internal IrcMessage NoSuchChannel(String prefix, String postfix, String channel, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorNoSuchChannel, prefix, postfix, channel, message);
        }

        internal IrcMessage TooManyChannels(String prefix, String postfix, String channel, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorTooManyChannels, prefix, postfix, channel, message);
        }

        internal IrcMessage BannedFromChannel(String prefix, String postfix, String channel, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorBannedFromChannel, prefix, postfix, channel, message);
        }

        internal IrcMessage InviteOnlyChannel(String prefix, String postfix, String channel, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorInviteOnlyChannel, prefix, postfix, channel, message);
        }

        internal IrcMessage ChannelIsFull(String prefix, String postfix, String channel, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorChannelIsFull, prefix, postfix, channel, message);
        }

        internal IrcMessage BadChannelKey(String prefix, String postfix, String channel, String message)
        {
            return new IrcMessage(IrcDefinition.Response.ErrorBadChannelKey, prefix, postfix, channel, message);
        }
    }
}
