using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObjectToBooleanConverter conv1 = new ObjectToBooleanConverter();
            bool bValue = (bool)conv1.Convert(value, targetType, parameter, culture);

            BoolToVisibilityConverter conv2 = new BoolToVisibilityConverter();
            return conv2.Convert(bValue, targetType, null, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
