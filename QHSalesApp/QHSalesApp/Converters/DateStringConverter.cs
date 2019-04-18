using System;
using System.Globalization;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    public class DateStringConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime ondate;
            if (value == null)
                return DateTime.Today.ToString("d", DateTimeFormatInfo.InvariantInfo);
            ondate = (DateTime)value;
            string retval = ondate.ToString("d", DateTimeFormatInfo.InvariantInfo);
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

        #endregion
    }
}
