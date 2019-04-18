using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strImg;
            if (value==null)
                strImg = Constants.BlankImgStr;
            else
                strImg =(string)value.ToString();
            string strImage = Regex.Replace(strImg, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
            return  Xamarin.Forms.ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(strImage)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
