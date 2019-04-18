using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    public class VoidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string curstatus = (string)value.ToString().ToLower();
                bool retvalue = true;
                if (curstatus != "false")
                    retvalue = true;
                else
                    retvalue = false;
                return retvalue;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Not used.
            throw new NotImplementedException();
        }
    }
}
