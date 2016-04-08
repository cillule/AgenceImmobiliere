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
        public enum ObjectToBooleanStringParameter
        {
            Normal = 0,
            Invert = 1,
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObjectToBooleanStringParameter param = GetParameterValue(parameter);
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

            
            if (bValue && param == ObjectToBooleanStringParameter.Normal) return "Oui"; 
            if (!bValue && param == ObjectToBooleanStringParameter.Invert) return "Oui";
            return "Non";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private ObjectToBooleanStringParameter GetParameterValue(object parameter)
        {
            if (parameter != null)
            {
                return ((ObjectToBooleanStringParameter)(int.Parse((string)(parameter))));
            }
            return ObjectToBooleanStringParameter.Normal;
        }
    }
}
