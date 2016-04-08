using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class TextBoxNumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null) return value;
            if (string.IsNullOrEmpty(parameter.ToString())) return value;

            string sParameter = parameter.ToString().ToLower();
            if (sParameter != "int" && sParameter != "double") return value;

            if (sParameter == "int" && (int)value < 0) return "";
            if (sParameter == "double" && (double)value < 0) return "";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null) return -1;
            if (string.IsNullOrEmpty(parameter.ToString())) return -1;

            string sParameter = parameter.ToString().ToLower();
            if (sParameter != "int" && sParameter != "double" &&
                sParameter != "int0" && sParameter != "double0") return -1;

            if (sParameter == "int")
            {
                int i;
                if (int.TryParse(value.ToString(), out i))
                    return i;
                else
                    return -1;
            }

            if (sParameter == "double")
            {
                double d;
                if (double.TryParse(value.ToString(), out d))
                    return d;
                else if (double.TryParse(value.ToString().Replace(",", "."), out d))
                    return d;
                else if (double.TryParse(value.ToString().Replace(".", ","), out d))
                    return d;
                else
                    return -1;
            }

            if (sParameter == "int0")
            {
                int i;
                if (int.TryParse(value.ToString(), out i))
                    return i;
                else
                    return 0;
            }

            if (sParameter == "double0")
            {
                double d;
                if (double.TryParse(value.ToString(), out d))
                    return d;
                else if (double.TryParse(value.ToString().Replace(",", "."), out d))
                    return d;
                else if (double.TryParse(value.ToString().Replace(".", ","), out d))
                    return d;
                else
                    return 0;
            }

            return -1;
        }
    }
}
