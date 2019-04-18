﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    class TitleChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string curstatus = (string)value.ToString().ToLower();
            string retvalue;
            if (curstatus == "loaded")
                retvalue ="Loaded Qty";
            else
                retvalue = "Pick Qty";
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
