using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp.Converters
{
    class ByteToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource imageSource = null;
            if (value != null)
            {

                // byte[] image = (byte[])value;
                byte[] image = System.Convert.FromBase64String((string)value);
                imageSource = ImageSource.FromStream(() => new MemoryStream(image));
            }
            return imageSource;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
