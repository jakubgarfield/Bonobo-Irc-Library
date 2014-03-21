using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bonobo.Irc.Client.UI.Data;
using System.Reflection;
using System.Windows;

namespace Bonobo.Irc.Test
{
    [TestClass]
    public class IrcClientTest
    {

        [TestMethod]
        public void BoolToVisibilityConverter_Visible()
        {
            var converter = new BoolToVisibilityConverter();
            var value = converter.Convert(true, typeof(Boolean), null, System.Globalization.CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Visible, value);
        }

        [TestMethod]
        public void BoolToVisibilityConverter_Collapsed()
        {
            var converter = new BoolToVisibilityConverter();
            var value = converter.Convert(false, typeof(Boolean), null, System.Globalization.CultureInfo.CurrentCulture);
            Assert.AreEqual(Visibility.Collapsed, value);
        }

        [TestMethod]
        public void IntegerRangeValidationRule_Max_True()
        {
            var rule = new IntegerRangeValidationRule();
            rule.MaxValue = 10;
            Assert.IsTrue(rule.Validate(9, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void IntegerRangeValidationRule_Max_False()
        {
            var rule = new IntegerRangeValidationRule();
            rule.MaxValue = 10;
            Assert.IsFalse(rule.Validate(11, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void IntegerRangeValidationRule_Min_True()
        {
            var rule = new IntegerRangeValidationRule();
            rule.MinValue = 10;
            Assert.IsFalse(rule.Validate(11, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void IntegerRangeValidationRule_Min_False()
        {
            var rule = new IntegerRangeValidationRule();
            rule.MinValue = 10;
            Assert.IsFalse(rule.Validate(9, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void IntegerRangeValidationRule_MinMax()
        {
            var rule = new IntegerRangeValidationRule();
            rule.MinValue = 10;
            rule.MaxValue = 11;
            Assert.IsTrue(rule.Validate(10, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void StringRequiredRule_True()
        {
            var rule = new StringRequiredValidationRule();
            Assert.IsTrue(rule.Validate("Test", System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void StringRequiredRule_False()
        {
            var rule = new StringRequiredValidationRule();
            Assert.IsFalse(rule.Validate(String.Empty, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void StringRequiredRule_Null()
        {
            var rule = new StringRequiredValidationRule();
            Assert.IsFalse(rule.Validate(null, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void RegexValidationRule_True()
        {
            var rule = new RegexValidationRule();
            rule.Pattern = "[1-9][0-9]";
            Assert.IsTrue(rule.Validate(11, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void RegexValidationRule_False()
        {
            var rule = new RegexValidationRule();
            rule.Pattern = "[1-9][0-9]";
            Assert.IsFalse(rule.Validate(01, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }

        [TestMethod]
        public void RegexValidationRule_Null()
        {
            var rule = new RegexValidationRule();
            rule.Pattern = "[1-9][0-9]";
            Assert.IsFalse(rule.Validate(null, System.Globalization.CultureInfo.CurrentCulture).IsValid);
        }
    }
}
