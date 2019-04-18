using System;
using System.Globalization;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal number;
            if (value == null)
                return "0";

            // decimal thedecimal = (decimal)value;
            Decimal.TryParse((string)value, out number);

            decimal thedecimal = number;
            string retval = thedecimal.ToString("G29");//G29
            if (number == 0) retval = "0";
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (string.IsNullOrEmpty(strValue))
                strValue = "0";
            decimal resultdecimal;
            if (decimal.TryParse(strValue, out resultdecimal))
            {
                return resultdecimal;
            }
            return 0;
        }
    }
}
