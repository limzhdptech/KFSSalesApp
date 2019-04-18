using System;
using System.Globalization;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    public class DecimalStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // AmountEntry.Text = string.Format("{0:0.00}", decimal.Parse(AmountEntry.Text));
            decimal number;
            if (value == null)
                return "0.00";

            // decimal thedecimal = (decimal)value;
            decimal.TryParse((string)value, out number);

            decimal thedecimal = number;
            string retval = thedecimal.ToString("G29");//G29
            if (number == 0) retval = "0.00";
            // string retval = string.Format("{0:0.00}", thedecimal);
            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
