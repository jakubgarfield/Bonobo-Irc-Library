using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Threading;

namespace Bonobo.Irc
{
    /// <summary>
    /// This class represents information about user and IRC server to connect
    /// </summary>
    public class IrcConnectionInfo : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler _propertyChanged;
        private String _username;
        private int _port;
        private IPAddress _address;
        private String _realname;
        private String _connectionPassword;

        public String Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public int Port
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged("Port");
                }
            }
        }

        public IPAddress Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        public String Realname
        {
            get { return _realname; }
            set
            {
                if (_realname != value)
                {
                    _realname = value;
                    OnPropertyChanged("Realname");
                }
            }
        }

        public String ConnectionPassword
        {
            get { return _connectionPassword; }
            set
            {
                if (_connectionPassword != value)
                {
                    _connectionPassword = value;
                    OnPropertyChanged("ConnectionPassword");
                }
            }
        }

        public IrcConnectionInfo()
        {
            _address = IPAddress.Parse("147.32.80.79");
            _port = 6667;
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        private void OnPropertyChanged(String propertyName)
        {
            var handler = _propertyChanged;

            if (handler != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);

                SynchronizationContext.Current.Post(() => handler(this, args));
            }
        }
    }
}
