using System;
using Xunit;

using Ynab_Sms;

namespace Ynab_Sms.Tests
{
    public class YnabLongToDoubleTests
    {
        [Fact]
        public void NumberGreaterThan1Dollar()
        {
            long num = 123456;
            double ret = Utils.YnabLongToDouble(num);
            Assert.Equal(123.456, ret);
        }

        [Fact]
        public void Zero()
        {
            long num = 0000;
            double ret = Utils.YnabLongToDouble(num);
            Assert.Equal(0.000, ret);
        }

        [Fact]
        public void NumberLessThan1Dollar()
        {
            long num = 010;
            double ret = Utils.YnabLongToDouble(num);
            Assert.Equal(0.010, ret);
        }
    }

    public class YnabLongToFormattedStringTests
    {
        [Fact]
        public void DollarValueInHundreds()
        {
            long num = 123456;
            string ret = Utils.YnabLongToFormattedString(num);
            Assert.Equal("$123.46", ret);
        }

        [Fact]
        public void DollarValueInThousands()
        {
            long num = 123456789;
            string ret = Utils.YnabLongToFormattedString(num);
            Assert.Equal("$123,456.79", ret);
        }

        [Fact]
        public void NegativeDollarValueInHundreds()
        {
            long num = -123456;
            string ret = Utils.YnabLongToFormattedString(num);
            Assert.Equal("-$123.46", ret);
        }

        [Fact]
        public void NegativeDollarValueInThousands()
        {
            long num = -123456789;
            string ret = Utils.YnabLongToFormattedString(num);
            Assert.Equal("-$123,456.79", ret);
        }
    }
}
