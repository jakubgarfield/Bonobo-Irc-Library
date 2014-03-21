using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Bonobo.Irc.Client.UI.Data
{
    public class RegexValidationRule : ValidationRule
    {
        private string _pattern;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);

            if (!string.IsNullOrEmpty(_pattern))
            {
                Regex regex = new Regex(_pattern);

                if (!regex.IsMatch((value ?? String.Empty).ToString()))
                {
                    result = new ValidationResult(false, ErrorMessage);
                }
            }

            return result;
        }

        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }

        public String ErrorMessage
        {
            get;
            set;
        }
    }
}
