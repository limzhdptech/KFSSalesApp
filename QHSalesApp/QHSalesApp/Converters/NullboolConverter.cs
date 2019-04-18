using System;
using System.Globalization;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    class NullboolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != string.Empty ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Not used.
            throw new NotImplementedException();
        }
    }
}
