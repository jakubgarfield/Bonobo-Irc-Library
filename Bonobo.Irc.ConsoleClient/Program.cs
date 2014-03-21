using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

namespace Bonobo.Irc.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestConnection(666);
        }

        //static void SessionStateChanged(object sender, IrcStateChangedEventArgs e)
        //{
        //    var session = (IrcSession) sender;
        //    if (session.State == IrcSessionState.Error)
        //    {
        //        Console.WriteLine(e.GetType());
        //        Console.WriteLine(e.Reason.Message);
        //    }
        //    else if (session.State == IrcSessionState.Closed)
        //    {
        //        Console.WriteLine("Not connected!");
        //        Console.WriteLine(e.Reason.Message);
        //        TestConnection(6667);
        //    }
        //}

        //static void TestConnection(int port)
        //{
        //    var endPoint = new IPEndPoint(IPAddress.Loopback, port);
        //    var user = new IrcConnectionInfo("ahoj", "jakubgarfield", "Jakub Chodounsky");

        //    using (var session = new IrcSession())
        //    {
        //        session.Open(user, endPoint);
        //        session.StateChanged += new EventHandler<IrcStateChangedEventArgs>(SessionStateChanged);
        //        Console.ReadLine();
        //        if (session.State != IrcSessionState.Closed)
        //        {
        //            Console.WriteLine("Quiting...");
        //            session.Quit();                    
        //        }
        //        Console.ReadLine();
        //    }
        //}
    }
}
