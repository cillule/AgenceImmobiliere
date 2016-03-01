using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class ObjectToBooleanConverter : IValueConverter
    {
        public enum ObjectToBooleanParameter
        {
            Normal = 0,
            Invert = 1,
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            parameter = GetParameterValue(parameter);
            if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
            {
                if ((((ObjectToBooleanParameter)(parameter)) == ObjectToBooleanParameter.Invert))
                {
                    return true;
                }
                return false;
            }
            if ((((ObjectToBooleanParameter)(parameter)) == ObjectToBooleanParameter.Invert))
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }        

        private ObjectToBooleanParameter GetParameterValue(object parameter)
        {
            if (parameter != null)
            {
                return ((ObjectToBooleanParameter)(int.Parse((string)(parameter))));
            }
            return ObjectToBooleanParameter.Normal;
        }

    }
}
