using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class ObjectToBooleanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bValue;

            if (value != null && value.GetType() == typeof(bool))
            {
                bValue = (bool)value;
            }
            else
            {
                ObjectToBooleanConverter conv = new ObjectToBooleanConverter();
                bValue = (bool)conv.Convert(value, targetType, parameter, culture);
            }

            
            if (bValue) return "Oui"; 
            else return "Non";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
