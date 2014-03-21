using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Net;

namespace Bonobo.Irc.Client.UI.Data
{

    [ValueConversion(typeof(object), typeof(IPAddress))]
    public class IPAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value ?? String.Empty).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return IPAddress.Parse((value ?? String.Empty).ToString());
        }
    }
}
