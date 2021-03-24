using System;
using System.Globalization;

namespace Ynab_Sms
{
    public static class Utils
    {
        /// <summary>
        /// YNAB long values that represent a dollar amount combine the decimnal and integer values together into a single long.
        /// The last 3 digits are the decimal value.
        /// This function converts the long into a traditional float
        /// /// </summary>
        public static double YnabLongToDouble(long num)
        {
            return (double)num / 1000;
        }

        /// <summary>
        /// Convert the YNAB long to a formatted string with currency formatting.
        /// </summary>
        public static string YnabLongToFormattedString(long num)
        {
            double n = YnabLongToDouble(num);

            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-US");
            ci.NumberFormat.CurrencyNegativePattern = 1; 

            return String.Format(ci, "{0:C}", n);
        }
    }
}