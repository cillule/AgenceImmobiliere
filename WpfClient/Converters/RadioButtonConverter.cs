using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null) return value;
            if (string.IsNullOrEmpty(parameter.ToString())) return value;

            string sParameter = parameter.ToString().ToLower();
            if (sParameter != "oui" && sParameter != "non" && sParameter != "null") return value;

            if (sParameter == "oui" && (bool?)value == true) return true;
            if (sParameter == "non" && (bool?)value == false) return true;
            if (sParameter == "null" && (bool?)value == null) return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null) return null;
            if (string.IsNullOrEmpty(parameter.ToString())) return null;

            string sParameter = parameter.ToString().ToLower();
            if (sParameter != "oui" && sParameter != "non" && sParameter != "null") return null;

            if (sParameter == "oui" && (bool)value) return true;
            if (sParameter == "non" && (bool)value) return false;
            if (sParameter == "null" && (bool)value) return null;

            return null;
        }
    }
}
