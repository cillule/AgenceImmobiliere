using System;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Converters
{
    class ConnectionParametersConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 3) return values;

            Commands.OpenWindowCommand.OpenMode mode;
            if (!Enum.TryParse(values[2].ToString(), out mode)) mode = Commands.OpenWindowCommand.OpenMode.Modal;

            return new Commands.OpenWindowCommand((Window)values[0], (Type)values[1], mode);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
