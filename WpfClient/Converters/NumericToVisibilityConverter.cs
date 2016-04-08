using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class NumericToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //NumericComparatorToBooleanConverter conv1 = new NumericComparatorToBooleanConverter();
            //bool bValue = (bool)conv1.Convert(new object[] {value, 1}, targetType, 5, culture);

            int num;
            bool bValue = false;
            if (value != null && int.TryParse(value.ToString(), out num)) bValue = num > 0;

            BoolToVisibilityConverter conv2 = new BoolToVisibilityConverter();
            return conv2.Convert(bValue, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
