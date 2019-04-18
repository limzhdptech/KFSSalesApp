using System;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    public class ColorConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            { 
                if ((Boolean)value)
                    return Color.Orange;
                else
                    return Color.BurlyWood;
            }
            return Color.BurlyWood;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
