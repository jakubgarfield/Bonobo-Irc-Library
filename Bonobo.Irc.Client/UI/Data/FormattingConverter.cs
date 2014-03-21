using System;
using System.Windows.Data;

namespace Bonobo.Irc.Client.UI.Data
{
    [ValueConversion(typeof(object),typeof(string))]
    public class FormattingConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string formatString = parameter as string;
            if (formatString != null)
            {
                culture = System.Globalization.CultureInfo.CurrentCulture;
                return string.Format(culture, formatString, value);
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}