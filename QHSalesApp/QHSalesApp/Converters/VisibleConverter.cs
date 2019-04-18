using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    class VisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string curstatus = (string)value.ToString().ToLower();
            bool retvalue = false;
            if (curstatus != "request")
                retvalue = false;
            else
                retvalue = true;
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
