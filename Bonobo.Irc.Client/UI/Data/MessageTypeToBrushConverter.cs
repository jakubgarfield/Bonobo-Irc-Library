using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Data;

namespace Bonobo.Irc.Client.UI.Data
{
    [ValueConversion(typeof(object), typeof(Brush))]
    public class MessageTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IrcMessageType)
            {
                var type = (IrcMessageType)value;

                switch (type)
                {
                    case IrcMessageType.Message:
                        return Brushes.DarkBlue;

                    case IrcMessageType.MyMessage:
                        return Brushes.Green;

                    case IrcMessageType.ChannelRequest:
                    case IrcMessageType.ServerRequest:
                        return Brushes.DarkGray;

                    case IrcMessageType.ServerResponse:
                    case IrcMessageType.ChannelResponse:
                        return Brushes.DarkRed;

                    case IrcMessageType.UnknownMessage:
                        return Brushes.Gray;

                    default:
                        break;
                }
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
