using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace Bonobo.Irc.Client.UI.Data
{
    public class IntegerRangeValidationRule : ValidationRule
    {
        private int _minValue;
        private int _maxValue;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            int number;
            if (Int32.TryParse((value ?? String.Empty).ToString(), out number))
            {
                var comparableValue = value;

                if (number < MinValue || number > MaxValue)
                {
                    result = new ValidationResult(false, ErrorMessage);
                }

                return result;
            }
            else
            {
                return new ValidationResult(false, ErrorMessage); 
            }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public String ErrorMessage
        {
            get;
            set;
        }

    }
}
