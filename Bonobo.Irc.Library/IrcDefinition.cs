using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Bonobo.Irc
{
    /// <summary>
    /// Class with protocol definitions
    /// </summary>
    public static class IrcDefinition
    {
        public const String NewLine = "\r\n";
        public static Encoding Encoding = Encoding.UTF8;

        public class Request
        {
            public const String Ping = "PING";
            public const String Pass = "PASS";
            public const String User = "USER";
            public const String Nick = "NICK";
            public const String Pong = "PONG";
            public const String Quit = "QUIT";
            public const String List = "LIST";
            public const String Join = "JOIN";
            public const String Part = "PART";
            public const String Names = "NAMES";
            public const String Topic = "TOPIC";
            public const String Mode = "MODE";
            public const String PrivateMessage = "PRIVMSG";
        }

        public static IEnumerable<String> ServerConversationsResponses = new List<String>()
        { 
            Response.ErrorAlreadyRegistred,
            Response.ErrorErrorneusNickname,
            Response.ErrorNeedMoreParams,
            Response.ErrorNicknameCollision,
            Response.ErrorNicknameInUse,
            Response.ErrorNoNicknameGiven,
            Response.ErrorNoSuchNick,
            Response.ErrorNoSuchServer,
            Response.ErrorNotRegistred,
            Response.Unknown
        };

        public class Response
        {
            public const String Unknown = "0";
            public const String ErrorNoSuchNick = "401";
            public const String ErrorNoSuchServer = "402";
            public const String ErrorNoNicknameGiven = "431";
            public const String ErrorErrorneusNickname = "432";
            public const String ErrorNicknameInUse = "433";
            public const String ErrorNicknameCollision = "436";
            public const String ErrorNotRegistred = "451";
            public const String ErrorNeedMoreParams = "461";
            public const String ErrorAlreadyRegistred = "462";
            public const String ListStart = "321";
            public const String List = "322";
            public const String ListEnd = "323";
            public const String EndOfNames = "366";
            public const String NoTopic = "331";
            public const String Topic = "332";
            public const String NameReply = "353";
            public const String ChannelCreation = "333";
            public const String ErrorNoSuchChannel = "403";
            public const String ErrorTooManyChannels = "405";
            public const String ErrorBannedFromChannel = "474";
            public const String ErrorBadChannelKey = "475";
            public const String ErrorChannelIsFull = "471";
            public const String ErrorInviteOnlyChannel = "473";

        }
    }
}
