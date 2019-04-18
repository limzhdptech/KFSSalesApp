using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    public class LoadVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string curstatus = (string)value.ToString().ToLower();
            bool retvalue = false;
            if (curstatus == "loaded")
                retvalue = true;
            else
                retvalue = false;
            return retvalue;
            //return (string)value.ToString().ToLower() == "load" ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Not used.
            throw new NotImplementedException();
        }
    }
}
