using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class ColorToSolidColorBrushConverter : IValueConverter
    {
        private static ColorToSolidColorBrushConverter _instance = null;
        public static ColorToSolidColorBrushConverter Instance
        {
            get
            {
                if (_instance == null) _instance = new ColorToSolidColorBrushConverter();
                return _instance;
            }
        }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }
}
